/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Model
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.Linq;
  using System.Text.RegularExpressions;

  public class Mission : ModelObject, IRosterEvent<Mission, MissionRoster>
  {
    public string Title { get; set; }
    public string County { get; set; }
    public string StateNumber { get; set; }
    public string CountyNumber { get; set; }
    public string MissionType { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? StopTime { get; set; }
    public string Comments { get; set; }
    public string Location { get; set; }
    public bool ReportCompleted { get; set; }

    public virtual ICollection<Mission> Followups { get; set; }
    public virtual Mission Previous { get; set; }

    public virtual ICollection<MissionLog> Log { get; set; }
    public virtual ICollection<MissionRoster> Roster { get; set; }

    public virtual MissionDetails Details { get; set; }
    public virtual ICollection<SubjectGroup> SubjectGroups { get; set; }
    public virtual ICollection<MissionGeography> MissionGeography { get; set; }

    public Mission()
      : base()
    {
      this.StartTime = DateTime.Now.Date;
      this.County = "King";

      this.Followups = new List<Mission>();
      this.Log = new List<MissionLog>();
      this.Roster = new List<MissionRoster>();
      this.SubjectGroups = new List<SubjectGroup>();
      this.MissionGeography = new List<MissionGeography>();
    }

    public override string ToString()
    {
      return string.Format("{0} {1}", this.StateNumber, this.Title);
    }

    public override string GetReportHtml()
    {
      return string.Format("{0} <b>{1}</b> Start:{2}, Type:{3}, County:{4}, Completed:{5}", this.StateNumber, this.Title, this.StartTime, this.MissionType, this.County, this.StopTime);
    }

    IEnumerable<MissionRoster> IRosterEvent<Mission, MissionRoster>.Roster
    {
      get { return (IEnumerable<MissionRoster>)this.Roster; }
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      if (string.IsNullOrEmpty(this.Title))
      {
        yield return new ValidationResult("Required", new[] { "Title" });
      }

      if (string.IsNullOrEmpty(this.Location))
      {
        yield return new ValidationResult("Required", new[] { "Location" });
      }

      if (string.IsNullOrEmpty(this.County))
      {
        yield return new ValidationResult("Required", new[] { "County" });
      }
      else if (!Utils.CountyNames.Contains(this.County, new Utils.CaseInsensitiveComparer()))
      {
        yield return new ValidationResult("Not a county in Washington", new[] { "County" });
      }


      if (this.StopTime.HasValue && this.StopTime <= this.StartTime)
      {
        yield return new ValidationResult("Start must be before Stop", new[] { "StartTime", "StopTime" });
      }
  
      if (!string.IsNullOrEmpty(this.StateNumber) && !Regex.IsMatch(this.StateNumber, @"^\d{2}\-\d{4}$") && !Regex.IsMatch(this.StateNumber, @"^\d{2}\-ES\-\d{3}$", RegexOptions.IgnoreCase))
      {
        yield return new ValidationResult("Must be in form 00-0000 or 00-ES-000", new[] { "StateNumber" });
      }

      if (!string.IsNullOrEmpty(this.CountyNumber) && this.County.Equals("King", StringComparison.OrdinalIgnoreCase) && !Regex.IsMatch(this.CountyNumber, @"^\d{2}\-\d{6}$"))
      {
        yield return new ValidationResult("Must be in form 00-000000", new[] { "CountyNumber" });
      }

      if (this.Previous != null && this.Previous.Id == this.Id)
      {
        yield return new ValidationResult("Can't link a mission to itself", new[] { "Previous" });
      }

    }
  }
}
