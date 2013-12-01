/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Model
{
  using System;

  public interface IDocument
  {
    Guid Id { get; }
    string Type { get; set; }
    Guid ReferenceId { get; set; }
    string FileName { get; set; }
    string MimeType { get; set; }
  }
}
