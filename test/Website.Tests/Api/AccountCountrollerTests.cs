/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Internal.Website.Tests.Api
{
  using System;
  using System.Collections;
  using System.Configuration.Provider;
  using System.Linq;
  using System.Reflection;
  using System.Web.Security;
  using Internal.Common;
  using Kcsar.Database.Model;
  using Kcsar.Database.Services;
  using Kcsar.Database.Web;
  using Kcsar.Database.Web.Api;
  using Kcsar.Database.Web.Api.Models;
  using Kcsar.Membership;
  using Moq;
  using NUnit.Framework;

  [TestFixture]
  public class AccountCountrollerTests
  {
    [TestCase("Firstname")]
    [TestCase("Lastname")]
    [TestCase("Email")]
    [TestCase("BirthDate")]
    [TestCase("Username")]
    [TestCase("Password")]
    public void Signup_RequiredFields(string propertyName)
    {
      Mock<IPermissionsService> permissionsMock = new Mock<IPermissionsService>(MockBehavior.Strict);
      permissionsMock.Setup(f => f.GetUser(It.IsAny<string>())).Returns((MembershipUser)null);

      AccountController c = new AccountController(null, null, permissionsMock.Object, null, new ConsoleLogger());

      var d = CreateData();
      PropertyInfo property = d.GetType().GetProperty(propertyName);
      PropertyInfo stringProperty = typeof(WebStrings).GetProperty("Property_" + propertyName, BindingFlags.Static | BindingFlags.Public);

      object[] options = property.PropertyType == typeof(string) ?
        new object[] { string.Empty, null } :
        new object[] { (DateTime?)null };

      foreach (object value in options)
      {
        property.SetValue(d, value);

        var expected = string.Format(WebStrings.Validation_Required, stringProperty.GetValue(null));

        Assert.AreEqual(expected, c.Signup(d), value == null ? "<null>" : value.ToString());
      }
    }

    [Test]
    public void Signup()
    {
      // SETUP
      var d = CreateData();

      string providerName = ((ICollection)Membership.Providers).Cast<ProviderBase>().First().Name;
      MembershipUser user = new MembershipUser(providerName, d.Username, string.Empty, d.Email, string.Empty, string.Empty, false, false, DateTime.Now, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
      TestProfile profile = new TestProfile();

      Mock<IPermissionsService> permissionMock = new Mock<IPermissionsService>(MockBehavior.Strict);
      permissionMock.Setup(f => f.GetUser(d.Username)).Returns<MembershipUser>(null);
      permissionMock.Setup(f => f.CreateUser(d.Username, d.Password, d.Email)).Returns(user).Verifiable();
      permissionMock.Setup(f => f.SetCurrentUser(d.Username)).Verifiable();
      permissionMock.Setup(f => f.UpdateUser(user));
      permissionMock.Setup(f => f.GetProfile(d.Username)).Returns(profile);
      permissionMock.Setup(f => f.RoleExists(AccountController.APPLICANT_ROLE)).Returns(true);
      permissionMock.Setup(f => f.AddUserToRole(d.Username, AccountController.APPLICANT_ROLE)).Verifiable();

      InMemoryDbSet<Member> members = new InMemoryDbSet<Member>();
      InMemoryDbSet<PersonContact> contacts = new InMemoryDbSet<PersonContact>();
      InMemoryDbSet<SarUnit> units = new InMemoryDbSet<SarUnit>(new[] { new SarUnit { Id = d.Units.First() } });
      InMemoryDbSet<Kcsar.Database.Model.UnitApplicant> applicants = new InMemoryDbSet<Kcsar.Database.Model.UnitApplicant>();

      Mock<IKcsarContext> dbMock = new Mock<IKcsarContext>(MockBehavior.Strict);
      dbMock.SetupGet(f => f.Members).Returns(members);
      dbMock.SetupGet(f => f.PersonContact).Returns(contacts);
      dbMock.SetupGet(f => f.Units).Returns(units);
      dbMock.SetupGet(f => f.UnitApplicants).Returns(applicants);
      dbMock.Setup(f => f.SaveChanges()).Returns(4);

      Mock<IWebHostingService> hostMock = new Mock<IWebHostingService>(MockBehavior.Strict);
      hostMock.Setup(f => f.ReadFile("EmailTemplates\\new-account-verification.html")).Returns("%Username%\n%VerifyLink%\n%WebsiteContact%");
      hostMock.Setup(f => f.GetApiUrl("Account", "Verify", d.Username, true)).Returns("THE-URL");
      hostMock.SetupGet(f => f.FeedbackAddress).Returns("feedback");

      Mock<IEmailService> emailMock = new Mock<IEmailService>(MockBehavior.Strict);
      string mailBody = null;
      emailMock
        .Setup(f => f.SendMail(d.Email, string.Format(AccountController.MAIL_SUBJECT_TEMPLATE, WebStrings.DatabaseName), It.IsAny<string>()))
        .Callback((Action<string, string, string>)((e, s, b) => mailBody = b))
        .Verifiable();

      // TEST
      AccountController c = new AccountController(emailMock.Object, dbMock.Object, permissionMock.Object, hostMock.Object, new ConsoleLogger());
      var result = c.Signup(d);

      // VERIFY
      Assert.AreEqual("OK", result);

      permissionMock.VerifyAll();
      emailMock.VerifyAll();

      var newMember = members.Single();
      Assert.AreEqual(d.Firstname, newMember.FirstName, "firstname");
      Assert.AreEqual(d.Lastname, newMember.LastName, "lastname");
      Assert.AreEqual(d.BirthDate, newMember.BirthDate, "dob");

      var applicant = applicants.Single();
    }

    [Test]
    public void Verify_Null()
    {
      var c = new AccountController(null, null, null, null, null);

      var result = c.Verify(null);

      Assert.AreEqual(false, result);
    }

    [TestCase(null)]
    [TestCase("")]
    public void Verify_UserBlank(string val)
    {
      var c = new AccountController(null, null, null, null, null);

      var result = c.Verify(new AccountVerify { Username = val, Key = "foo" });

      Assert.AreEqual(false, result);
    }

    [TestCase(null)]
    [TestCase("")]
    public void Verify_KeyBlank(string val)
    {
      var c = new AccountController(null, null, null, null, null);

      var result = c.Verify(new AccountVerify { Username = "user", Key = val });

      Assert.AreEqual(false, result);
    }

    [Test]
    public void Verify_UserNotFound()
    {
      string user = "user";

      Mock<IPermissionsService> mock = new Mock<IPermissionsService>(MockBehavior.Strict);
      mock.Setup(f => f.GetUser(user)).Returns<MembershipUser>(null).Verifiable();

      var c = new AccountController(null, null, mock.Object, null, null);

      var result = c.Verify(new AccountVerify { Username = user, Key = "foo" });

      Assert.AreEqual(false, result);
      mock.VerifyAll();
    }

    [Test]
    public void Verify()
    {
      var d = CreateData();

      string providerName = ((ICollection)Membership.Providers).Cast<ProviderBase>().First().Name;
      MembershipUser user = new MembershipUser(providerName, d.Username, Guid.NewGuid().ToString(), d.Email, string.Empty, string.Empty, false, false, DateTime.Now, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);

      Mock<IPermissionsService> mock = new Mock<IPermissionsService>(MockBehavior.Strict);
      mock.Setup(f => f.GetUser(user.UserName)).Returns(user);
      mock.Setup(f => f.UpdateUser(user)).Verifiable();

      var c = new AccountController(null, null, mock.Object, null, null);

      var result = c.Verify(new AccountVerify { Key = user.ProviderUserKey.ToString().ToUpper(), Username = d.Username });

      Assert.IsTrue(result, "result");

      mock.VerifyAll();
    }

    private AccountSignup CreateData()
    {
      return new AccountSignup
      {
        Firstname = "Joe",
        Middlename = "Schmoe",
        Lastname = "Tester",
        Email = "joe@example.com",
        BirthDate = DateTime.Now.AddYears(-16),
        Gender = "m",
        Units = new[] { Guid.NewGuid() },
        Username = "joeTester",
        Password = "SuperSecret",
      };
    }
  }
}
