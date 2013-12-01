﻿/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Model
{
    using System;

  public class UnitApplicant : ModelObject
    {
        [ReportedReference]
        public Member Applicant { get; set; }
        [ReportedReference]
        public SarUnit Unit { get; set; }
        public DateTime Started { get; set; }
        public string Data { get; set; }
        public bool IsActive { get; set; }

        public UnitApplicant()
            : base()
        {
        }

        public override string ToString()
        {
            return this.GetReportHtml();
        }

        public override string GetReportHtml()
        {
            return string.Format("<b>{0}</b> applying to <b>{1}</b>, started: {2}, active: {3}", this.Applicant.FullName, this.Unit.DisplayName, this.Started, this.IsActive);
        }
    }
}
