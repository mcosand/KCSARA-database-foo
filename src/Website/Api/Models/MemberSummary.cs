/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Web.Api.Models
{
  using System;
  using System.Collections.Generic;
  using Kcsar.Database.Model;

  public class MemberSummary
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string WorkerNumber { get; set; }
    public Dictionary<Guid, string> Units { get; set; }

    public MemberSummary()
    {
    }

    public MemberSummary(Member member)
    {
      this.Id = member.Id;
      this.Name = member.ReverseName;
      this.WorkerNumber = member.DEM;
    }
  }
}
