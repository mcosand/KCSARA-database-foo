/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Model
{
  using System;
  using System.ComponentModel.DataAnnotations;

  public class SensitiveInfoAccess
  {
    public SensitiveInfoAccess()
    {
      this.Id = Guid.NewGuid();
    }

    [Key]
    public Guid Id { get; set; }
    public string Actor { get; set; }
    public DateTime Timestamp { get; set; }
    public Member Owner { get; set; }
    public string Action { get; set; }
    public string Reason { get; set; }
  }
}
