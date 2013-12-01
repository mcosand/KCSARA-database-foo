﻿/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Model
{
  public class MemberMedical : ModelObject
  {
    public Member Member { get; set; }
    public string EncryptedAllergies { get; set; }
    public string EncryptedMedications { get; set; }
    public string EncryptedDisclosures { get; set; }

    public override string GetReportHtml()
    {
      return "Medical information for <b>" + this.Member.FullName + "</b>";
    }

  }
}
