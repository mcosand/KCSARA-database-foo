/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Web.Api.Models
{
  using System;

  public class UnitApplicant
  {
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public string NameReverse { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public int EmergencyContactCount { get; set; }
    public int RemainingDocCount { get; set; }

    public string Background { get; set; }
    public DateTime Started { get; set; }
    public bool Active { get; set; }
  }
}