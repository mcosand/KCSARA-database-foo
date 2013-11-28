/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Services
{
  using System.Net.Mail;

  public interface IEmailService
  {
    void SendMail(MailMessage msg);
    void SendMail(string toAddresses, string subject, string body);
  }
}
