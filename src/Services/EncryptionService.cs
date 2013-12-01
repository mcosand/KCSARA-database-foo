/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsara.Database.Services
{
  using System;
  using System.Text;
  using System.Web.Security;

  public static class EncryptionService
  {
    public const string MEMBER_SENSITIVE = "Member-Sensitive-Data";

    public static string Protect(string purpose, string unprotectedText)
    {
      var unprotectedBytes = Encoding.UTF8.GetBytes(unprotectedText);
      var protectedBytes = MachineKey.Protect(unprotectedBytes, purpose);
      var protectedText = Convert.ToBase64String(protectedBytes);
      return protectedText;
    }

    public static string Unprotect(string purpose, string protectedText)
    {
      var protectedBytes = Convert.FromBase64String(protectedText);
      var unprotectedBytes = MachineKey.Unprotect(protectedBytes, purpose);
      var unprotectedText = Encoding.UTF8.GetString(unprotectedBytes);
      return unprotectedText;
    }

  }
}