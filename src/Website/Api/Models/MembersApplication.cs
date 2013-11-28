/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Web.Api.Models
{
  using System;

  public class MembersApplication
  {
    public Guid Id { get; set; }
    public NameIdPair Unit { get; set; }
    public DateTime Started { get; set; }
    public bool IsActive { get; set; }
    public bool CanEdit { get; set; }
  }
}