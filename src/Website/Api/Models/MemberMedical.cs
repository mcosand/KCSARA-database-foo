/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Web.Api.Models
{
  using System.Collections.Generic;

  public class MemberMedical
  {
    public NameIdPair Member { get; set; }
    public string Allergies { get; set; }
    public string Medications { get; set; }
    public string Disclosure { get; set; }
    public IEnumerable<EmergencyContact> Contacts { get; set; }

    public bool IsSensitive { get; set; }
  }
}