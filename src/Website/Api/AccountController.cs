/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Web.Api
{
  using System;
  using System.Configuration;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Web.Http;
  using System.Web.Profile;
  using Kcsar.Database.Model;
  using Kcsar.Database.Services;
  using Kcsar.Database.Web.Api.Models;
  using Kcsar.Membership;
  using log4net;

  [ModelValidationFilter]
  public class AccountController : DatabaseApiController
  {
    public static readonly string APPLICANT_ROLE = "cdb.applicants";

    public static readonly int USERNAME_MIN_LENGTH = 3;
    public static readonly int USERNAME_MAX_LENGTH = 200;
    public static readonly int PASSWORD_MIN_LENGTH = 6;
    public static readonly int PASSWORD_MAX_LENGTH = 64;
    public static readonly int APPLICANT_MIN_AGE = 14;
    public static readonly string USERNAME_STATUS_AVAILABLE = "Available";
    public static readonly string USERNAME_STATUS_NOTAVAILABLE = "Not Available";

    public static readonly string LOG_ERROR_CREATING_ACCOUNT = "Error creating account";
    public static readonly string LOG_ERROR_CREATING_ACCOUNT_EXTERNAL = "An error occured while creating your user account";

    public static readonly string MAIL_SUBJECT_TEMPLATE = "{0} account verification";

    readonly IEmailService email;

    public AccountController(
      IEmailService email,
      IKcsarContext db,
      IPermissionsService permissions,
      IWebHostingService hosting,
      ILog log)
      : base(db, permissions, hosting, log)
    {
      this.email = email;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">Username to check.</param>
    /// <returns></returns>
    [HttpPost]
    public string CheckUsername(string id)
    {
      if (id.Length < USERNAME_MIN_LENGTH)
        return string.Format(WebStrings.Validation_MinCharacters, WebStrings.Property_Username, USERNAME_MIN_LENGTH);
      if (id.Length > USERNAME_MAX_LENGTH)
        return string.Format(WebStrings.Validation_MaxCharacters, WebStrings.Property_Username, USERNAME_MAX_LENGTH);
      if (!Regex.IsMatch(id, @"^[a-zA-Z0-9\.\-_]+$"))
        return WebStrings.Validation_UsernameFormat;

      var existing = System.Web.Security.Membership.GetUser(id);
      return (existing == null) ? USERNAME_STATUS_AVAILABLE : USERNAME_STATUS_NOTAVAILABLE;
    }

    [HttpPost]
    public string Signup(AccountSignup data)
    {
      if (string.IsNullOrWhiteSpace(data.Firstname))
        return string.Format(WebStrings.Validation_Required, WebStrings.Property_Firstname);
      if (string.IsNullOrWhiteSpace(data.Lastname))
        return string.Format(WebStrings.Validation_Required, WebStrings.Property_Lastname);

      if (string.IsNullOrWhiteSpace(data.Email))
        return string.Format(WebStrings.Validation_Required, WebStrings.Property_Email);
      if (!Regex.IsMatch(data.Email, @"^\S+@\S+(\.\S+)+$"))
        return WebStrings.Validation_BadEmail;

      if (data.BirthDate.HasValue == false)
        return string.Format(WebStrings.Validation_Required, WebStrings.Property_BirthDate);
      if (data.BirthDate > DateTime.Today.AddYears(-APPLICANT_MIN_AGE))
        return string.Format(WebStrings.Validation_ApplicantYoung, APPLICANT_MIN_AGE);
      if (data.BirthDate < DateTime.Today.AddYears(-120))
        return string.Format(WebStrings.Validation_Invalid, WebStrings.Property_BirthDate.ToLower());

      if (!(new[] { "m", "f", null }.Contains(data.Gender)))
        return string.Format(WebStrings.Validation_Invalid, WebStrings.Property_Gender.ToLower());

      if (data.Units.Length == 0)
        return string.Format(WebStrings.Validation_AtLeastOne, WebStrings.Object_Unit.ToLower());

      if (string.IsNullOrWhiteSpace(data.Username))
        return string.Format(WebStrings.Validation_Required, WebStrings.Property_Username);
      if (data.Username.Length < USERNAME_MIN_LENGTH)
        return string.Format(WebStrings.Validation_MinCharacters, WebStrings.Property_Username, USERNAME_MIN_LENGTH);
      if (data.Username.Length > USERNAME_MAX_LENGTH)
        return string.Format(WebStrings.Validation_MaxCharacters, WebStrings.Property_Username, USERNAME_MAX_LENGTH);
      if (!Regex.IsMatch(data.Username, @"^[a-zA-Z0-9\.\-_]+$"))
        return WebStrings.Validation_UsernameFormat;
      if (this.permissions.GetUser(data.Username) != null)
        return WebStrings.Validation_UsernameTaken;


      if (string.IsNullOrWhiteSpace(data.Password))
        return string.Format(WebStrings.Validation_Required, WebStrings.Property_Password);
      if (data.Password.Length < PASSWORD_MIN_LENGTH)
        return string.Format(WebStrings.Validation_MinCharacters, WebStrings.Property_Password, PASSWORD_MIN_LENGTH);
      if (data.Password.Length > PASSWORD_MAX_LENGTH)
        return string.Format(WebStrings.Validation_MaxCharacters, WebStrings.Property_Password, PASSWORD_MAX_LENGTH);


      var user = this.permissions.CreateUser(data.Username, data.Password, data.Email);

      try
      {
        user.IsApproved = false;
        this.permissions.UpdateUser(user);

        this.permissions.SetCurrentUser(data.Username);

        Member newMember = new Member
        {
          FirstName = data.Firstname,
          MiddleName = data.Middlename,
          LastName = data.Lastname,
          BirthDate = data.BirthDate,
          InternalGender = data.Gender,
          Status = MemberStatus.Applicant,
          Username = data.Username
        };
        db.Members.Add(newMember);

        PersonContact email = new PersonContact
        {
          Person = newMember,
          Type = "email",
          Value = data.Email,
          Priority = 0
        };
        db.PersonContact.Add(email);

        foreach (Guid unitId in data.Units)
        {
          UnitsController.RegisterApplication(db, unitId, newMember);
        }

        KcsarUserProfile profile = this.permissions.GetProfile(data.Username);
        if (profile != null)
        {
          profile.FirstName = data.Firstname;
          profile.LastName = data.Lastname;
          profile.LinkKey = newMember.Id.ToString();
          profile.Save();
        }

        if (!this.permissions.RoleExists(APPLICANT_ROLE))
        {
          this.permissions.CreateRole(APPLICANT_ROLE);
        }
        this.permissions.AddUserToRole(data.Username, APPLICANT_ROLE);

        string mailSubject = string.Format(MAIL_SUBJECT_TEMPLATE, WebStrings.DatabaseName);
        string mailTemplate = this.hosting.ReadFile("EmailTemplates\\new-account-verification.html");
        string mailBody = mailTemplate
            .Replace("%Username%", data.Username)
            .Replace("%VerifyLink%", this.hosting.GetApiUrl("Account", "Verify", data.Username, true)  + "?key=" + user.ProviderUserKey.ToString())
            .Replace("%WebsiteContact%", this.hosting.FeedbackAddress);

        db.SaveChanges();
        this.email.SendMail(data.Email, mailSubject, mailBody);
      }
      catch (Exception ex)
      {
        log.Error(LOG_ERROR_CREATING_ACCOUNT, ex);
        this.permissions.DeleteUser(data.Username);
        return LOG_ERROR_CREATING_ACCOUNT_EXTERNAL;
      }

      return "OK";
    }

    [HttpPost]
    public bool Verify(AccountVerify data)
    {
      if (data == null || string.IsNullOrWhiteSpace(data.Username) || string.IsNullOrWhiteSpace(data.Key))
        return false;

      var user = this.permissions.GetUser(data.Username);
      if (user != null && data.Key.Equals((user.ProviderUserKey ?? "").ToString(), StringComparison.OrdinalIgnoreCase))
      {
        user.IsApproved = true;
        this.permissions.UpdateUser(user);

        return true;
      }

      return false;
    }
  }
}