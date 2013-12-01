/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Membership
{
  using System;
  using System.Configuration;
  using System.Net.Mail;
  using System.Web.Profile;
  using System.Web.Security;
  using Kcsar.Database.Services;

  public class MembershipProvider : SqlMembershipProvider
  {
    protected readonly IEmailService email;
    protected readonly IHostingService hosting;

    public MembershipProvider(IEmailService email, IHostingService hosting)
      : base()
    {
      this.email = email;
      this.hosting = hosting;
    }

    public void SetPassword(string username, string newPassword, bool sendMail)
    {
      // There's enough stuff going on here that it would be hard to return false.
      // What would false mean? All passwords failed? At least one password failed?
      // Therefore, we throw on failure.

      string tempPassword = base.ResetPassword(username, null);
      base.ChangePassword(username, tempPassword, newPassword);

      if (sendMail)
      {
        MembershipUser user = this.GetUser(username, false);

        MailMessage msg = new MailMessage();
        msg.To.Add(user.Email);
        msg.From = new MailAddress(this.hosting.FromAddress, ServiceStrings.DatabaseName);
        msg.Subject = ServiceStrings.DatabaseName + " Password Changed";
        msg.Body = string.Format("You password has been changed.\n\nUsername: {0}\nPassword: {1}", username, newPassword);

        this.email.SendMail(msg);
      }
    }
  }
}
