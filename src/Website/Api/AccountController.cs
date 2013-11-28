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
    public const string APPLICANT_ROLE = "cdb.applicants";

    public const int USERNAME_MIN_LENGTH = 3;
    public const int USERNAME_MAX_LENGTH = 200;
    public const int PASSWORD_MIN_LENGTH = 6;
    public const int PASSWORD_MAX_LENGTH = 64;
    public const int APPLICANT_MIN_AGE = 14;
    public const string USERNAME_STATUS_AVAILABLE = "Available";
    public const string USERNAME_STATUS_NOTAVAILABLE = "Not Available";

    readonly IEmailService email;

    public AccountController(IEmailService email, IPermissionsService permissions, IKcsarContext db, ILog log)
      : base(db, permissions, log)
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


      var user = System.Web.Security.Membership.CreateUser(data.Username, data.Password, data.Email);

      try
      {
        user.IsApproved = false;
        System.Web.Security.Membership.UpdateUser(user);

        System.Web.Security.FormsAuthenticationTicket ticket = new System.Web.Security.FormsAuthenticationTicket(data.Username, false, 5);
        Thread.CurrentPrincipal = new System.Web.Security.RolePrincipal(new System.Web.Security.FormsIdentity(ticket));

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

        KcsarUserProfile profile = ProfileBase.Create(data.Username) as KcsarUserProfile;
        if (profile != null)
        {
          profile.FirstName = data.Firstname;
          profile.LastName = data.Lastname;
          profile.LinkKey = newMember.Id.ToString();
          profile.Save();
        }

        if (!System.Web.Security.Roles.RoleExists(APPLICANT_ROLE))
        {
          System.Web.Security.Roles.CreateRole(APPLICANT_ROLE);
        }
        System.Web.Security.Roles.AddUserToRole(data.Username, APPLICANT_ROLE);

        string mailSubject = string.Format("{0} account verification", ConfigurationManager.AppSettings["dbNameShort"] ?? "KCSARA");
        string mailTemplate = File.ReadAllText(Path.Combine(Path.GetDirectoryName(new Uri(typeof(AccountController).Assembly.CodeBase).LocalPath), "EmailTemplates", "new-account-verification.html"));
        string mailBody = mailTemplate
            .Replace("%Username%", data.Username)
            .Replace("%VerifyLink%", new Uri(this.Request.RequestUri, Url.Route("Default", new { httproute = "", controller = "Account", action = "Verify", id = data.Username })).AbsoluteUri + "?key=" + user.ProviderUserKey.ToString())
            .Replace("%WebsiteContact%", "webpage@kingcountysar.org");

        db.SaveChanges();
        this.email.SendMail(data.Email, mailSubject, mailBody);
      }
      catch (Exception ex)
      {
        log.Error(ex.ToString());
        System.Web.Security.Membership.DeleteUser(data.Username);
        return "An error occured while creating your user account";
      }

      return "OK";
    }

    [HttpPost]
    public bool Verify(AccountVerify data)
    {
      if (data == null || string.IsNullOrWhiteSpace(data.Username) || string.IsNullOrWhiteSpace(data.Key))
        return false;

      var user = System.Web.Security.Membership.GetUser(data.Username);
      if (user != null && data.Key.Equals((user.ProviderUserKey ?? "").ToString(), StringComparison.OrdinalIgnoreCase))
      {
        user.IsApproved = true;
        System.Web.Security.Membership.UpdateUser(user);

        return true;
      }

      return false;
    }
  }
}