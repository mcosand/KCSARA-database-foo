﻿/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Model
{
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.Linq;

  public class Animal : ModelObject
  {
    public static readonly string[] AllowedTypes = new string[] { "horse", "dog" };

    public string DemSuffix { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Comments { get; set; }
    public virtual ICollection<AnimalOwner> Owners { get; set; }
    public virtual ICollection<AnimalMission> MissionRosters { get; set; }
    public string PhotoFile { get; set; }

    public Animal()
      : base()
    {
      this.Type = Animal.AllowedTypes[0];
      this.Owners = new List<AnimalOwner>();
      this.MissionRosters = new List<AnimalMission>();
    }

    public Member GetPrimaryOwner()
    {
      foreach (AnimalOwner link in this.Owners)
      {
        if (link.IsPrimary)
        {
          return link.Owner;
        }
      }
      return null;
    }

    public override string GetReportHtml()
    {
      return string.Format("<b>{0}</b> Suffix:{1} Type:{2} Comments:{3}", this.Name, this.DemSuffix, this.Type, this.Comments);
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {

      if (string.IsNullOrWhiteSpace(this.Name))
      {
        yield return new ValidationResult("Required", new[] { "Name" });
      }

      if (string.IsNullOrWhiteSpace(this.DemSuffix))
      {
        yield return new ValidationResult("Required", new[] { "DemSuffix" });
      }

      if (string.IsNullOrEmpty(this.Type))
      {
        yield return new ValidationResult("Required", new[] { "Type" });
      }
      else if (!Animal.AllowedTypes.Contains(this.Type.ToLower()))
      {
        yield return new ValidationResult("Must be one of: " + string.Join(", ", Animal.AllowedTypes));
      }
    }
  }
}
