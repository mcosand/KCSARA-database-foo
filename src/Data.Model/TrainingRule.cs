/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Model
{
  using System;
  using System.Collections.Generic;

  public class TrainingRule : ModelObject
  {
    public string RuleText { get; set; }
    public DateTime? OfferedFrom { get; set; }
    public DateTime? OfferedUntil { get; set; }
    public virtual ICollection<TrainingAward> Results { get; set; }

    public TrainingRule()
    {
      this.Results = new List<TrainingAward>();
    }

    public override string GetReportHtml()
    {
      return RuleText;
    }
  }
}
