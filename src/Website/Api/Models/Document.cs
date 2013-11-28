/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Website.Api.Models
{
  using System;

  public class Document
  {
    public Guid Id { get; set; }

    public string Type { get; set; }

    public Guid Reference { get; set; }

    public string Title { get; set; }

    public int Size { get; set; }

    public string Mime { get; set; }

    public DateTime? Changed { get; set; }

    public string Url { get; set; }

    public string Thumbnail { get; set; }
  }
}
