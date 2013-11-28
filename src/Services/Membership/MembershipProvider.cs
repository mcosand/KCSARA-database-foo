﻿
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

    public MembershipProvider(IEmailService email)
      : base()
    {
      this.email = email;
    }

    public void SetPassword(string username, string newPassword, bool sendMail)
    {
      // There's enough stuff going on here that it would be hard to return false.
      // What would false mean? All passwords failed? At least one password failed?
      // Therefore, we throw on failure.

      //PasswordSyncSection syncSection = ConfigurationManager.GetSection("passwordSync") as PasswordSyncSection;
      //if (syncSection != null)
      //{
      //    foreach (SynchronizerElement config in syncSection.Items)
      //    {
      //        Type syncType = Type.GetType(config.Type);
      //        if (syncType != null)
      //        {
      //            IPasswordSynchronizer sync = Activator.CreateInstance(syncType, config.Option, LogManager.GetLogger(syncType.Name)) as IPasswordSynchronizer;
      //            sync.SetPassword(username, newPassword);
      //        }
      //    }
      //}

      string tempPassword = base.ResetPassword(username, null);
      base.ChangePassword(username, tempPassword, newPassword);

      if (sendMail)
      {
        MembershipUser user = this.GetUser(username, false);

        MailMessage msg = new MailMessage();
        msg.To.Add(user.Email);
        msg.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"] ?? "webpage@kcsar.local", "KCSARA Web Page");
        msg.Subject = "KCSARA Password Changed";
        msg.Body = string.Format("You password has been changed.\n\nUsername: {0}\nPassword: {1}", username, newPassword);

        this.email.SendMail(msg);    
      }
    }

    public static Guid? UsernameToMemberKey(string name)
    {
      KcsarUserProfile profile = ProfileBase.Create(name) as KcsarUserProfile;
      if (profile.UsesLink)
      {
        return new Guid(profile.LinkKey);
      }
      return null;
    }
  }
}