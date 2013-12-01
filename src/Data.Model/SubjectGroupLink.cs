/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Model
{
  public class SubjectGroupLink : ModelObject
    {
        public int Number { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual SubjectGroup Group { get; set; }

        public override string GetReportHtml()
        {
            return string.Format("<b>{0}'s behavior on {1}</b> ", this.Subject.FirstName, this.Group.Mission.Title);
        }
    }
}
