using System;
using System.Collections.Generic;

namespace Kcsar.Database.Model
{
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
