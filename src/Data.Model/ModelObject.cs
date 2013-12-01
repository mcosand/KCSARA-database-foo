/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Model
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;

  public abstract class ModelObject : IModelObject
  {
    public DateTime LastChanged { get; set; }
    public string ChangedBy { get; set; }
    public Guid Id { get; set; }

    public ModelObject()
    {
      this.Id = Guid.NewGuid();
    }

    public abstract string GetReportHtml();

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      return new ValidationResult[0];
    }
  }
}
