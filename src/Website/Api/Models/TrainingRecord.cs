/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Web.Api.Models
{
  using System;

  public class TrainingRecord
  {
    public MemberSummary Member { get; set; }
    public TrainingCourse Course { get; set; }
    public string Completed { get; set; }
    public string Expires { get; set; }
    public string ExpirySrc { get; set; }
    public string Source { get; set; }
    public Guid ReferenceId { get; set; }
    public string Comments { get; set; }
    public bool? Required { get; set; }
    public int PendingUploads { get; set; }
  }

  public class TrainingExpiration : TrainingRecord
  {
    public string Status { get; set; }
    public string ExpiryText { get; set; }
  }

  public class CompositeExpiration
  {
    public bool? Goodness { get; set; }
    public TrainingExpiration[] Expirations { get; set; }
  }
}