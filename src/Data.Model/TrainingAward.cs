/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Model
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Threading;

  public class TrainingAward : ModelObject, ITrainingAward
  {
    [Required]
    public DateTime Completed { get; set; }
    public DateTime? Expiry { get; set; }
    public string DocPath { get; set; }
    public string metadata { get; set; }
    [Required]
    public virtual Member Member { get; set; }
    [Required]
    public virtual TrainingCourse Course { get; set; }
    public virtual TrainingRoster Roster { get; set; }


    [NotMapped]
    public bool UploadsPending { get; set; }

    public TrainingAward()
      : base()
    {
      this.LastChanged = DateTime.Now;
      this.ChangedBy = Thread.CurrentPrincipal.Identity.Name;
      this.UploadsPending = false;
      this.Completed = DateTime.Today;
    }

    public override string GetReportHtml()
    {
      return string.Format("<b>{0}</b> awarded <b>{1}</b>, {2:d}. Expires:{3:d}, Have Roster={4}", this.Member.FullName, this.Course.DisplayName, this.Completed, this.Expiry, this.Roster != null);
    }

    public override string ToString()
    {
      return this.GetReportHtml();
    }


    public DateTime? NullableCompleted { get { return new DateTime?(this.Completed); } }

    public TrainingRule Rule { get { return null; } }

    public override System.Collections.Generic.IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      if (this.Completed > DateTime.Today.AddMonths(3))
      {
        yield return new ValidationResult("Too far in the future. Must be less than 3 months from today.", new[] { "Completed" });
      }

      if (this.Expiry.HasValue && this.Completed > this.Expiry)
      {
        yield return new ValidationResult("Expiration must be after time completed.", new[] { "Expiry" });
      }

      if (this.Roster != null)
      {
        if (this.Completed != this.Roster.TimeOut)
        {
          yield return new ValidationResult("When an award is via a roster, Completed must be the roster time out.", new[] { "Completed" });
        }
      }
      else
      {
        if (string.IsNullOrWhiteSpace(this.metadata) && !this.UploadsPending)
        {
          yield return new ValidationResult("When an award is given without a roster, documentation is required.", new[] { "metadata" });
        }
      }

      //foreach (TrainingAward row in this.Member.TrainingAwards)
      //{
      //    if (row.Id != this.Id && this.MemberReference.EntityKey == row.MemberReference.EntityKey && this.CourseReference.EntityKey == row.CourseReference.EntityKey && this.Completed == row.Completed)
      //    {
      //        errors.Add(new RuleViolation(this.Id, "Completed", this.Completed.ToString(),
      //            string.Format("Conflicts with another entry, where Course \"{0}\" was completed {1}", this.Course.DisplayName, this.Completed)));
      //    }
      //}
    }
  }
}
