/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Web.Api.Models
{
  using System;

  public class MemberContact
  {
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public string Type { get; set; }
    public string SubType { get; set; }
    public string Value { get; set; }
    public int Priority { get; set; }
  }
}
