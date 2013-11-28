using System;
using System.Reflection;
using System.Web.Security;
using Kcsar.Database.Services;
using Kcsar.Database.Web;
using Kcsar.Database.Web.Api;
using Kcsar.Database.Web.Api.Models;
using Moq;
using NUnit.Framework;

namespace Internal.Website.Tests.Api
{
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

      AccountController c = new AccountController(null, permissionsMock.Object, null, null);

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
