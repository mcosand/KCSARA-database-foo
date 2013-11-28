/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Web.Api.Models
{
  using System;
  using System.Linq.Expressions;
  using Data = Kcsar.Database.Model;

  public class TrainingCourse
  {
    public Guid Id { get; set; }

    public string Title { get; set; }

    public int Required { get; set; }

    public bool? Offered { get; set; }

    public static Expression<Func<Data.TrainingCourse, TrainingCourse>> GetDataModelConversion(DateTime? when)
    {
      return f => new TrainingCourse
        {
          Id = f.Id,
          Title = f.DisplayName,
          Required = f.WacRequired,
          Offered = (when == null) ? (bool?)null : ((f.OfferedFrom ?? DateTime.MinValue) <= when) && ((f.OfferedUntil ?? DateTime.MaxValue) > when)
        };
    }
  }
}