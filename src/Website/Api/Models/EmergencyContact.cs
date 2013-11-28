/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Web.Api.Models
{
  using System;

  public class EmergencyContact
  {
    public Guid Id { get; set; }
    public bool IsSensitive { get; set; }

    public string Name { get; set; }
    public string Relation { get; set; }
    public string Type { get; set; }
    public string Number { get; set; }

  }
}