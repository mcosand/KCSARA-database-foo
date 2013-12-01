/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Model
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;

  public class AnimalOwner : ModelObject
  {
    public bool IsPrimary { get; set; }
    public DateTime Starting { get; set; }
    public DateTime? Ending { get; set; }
    [Required]
    public virtual Animal Animal { get; set; }
    public virtual Member Owner { get; set; }

    public override string GetReportHtml()
    {
      return string.Format("<b>{0}</b> Suffix:{1} Type:{2} Comments:{3}", this.Animal.Name, this.Owner.FullName, this.IsPrimary, this.Starting, this.Ending);
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      if (this.Ending.HasValue && this.Ending <= this.Starting)
      {
        yield return new ValidationResult("Must be after start date: " + this.Starting.ToString(), new[] { "Ending" });
      }
    }
  }
}
