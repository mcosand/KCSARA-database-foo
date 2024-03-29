﻿/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Model
{
  using System;
  using System.ComponentModel.DataAnnotations;

  public interface IModelObject : IValidatableObject
  {
    DateTime LastChanged { get; set; }
    string ChangedBy { get; set; }
    Guid Id { get; }
    string GetReportHtml();
  }
}
