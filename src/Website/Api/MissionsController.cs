/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Web.Api
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web.Http;
  using Kcsar.Database.Model;
  using Kcsar.Database.Services;
  using Kcsar.Database.Web.Api.Models;
  using log4net;

  public class MissionsController : DatabaseApiController
  {
    public MissionsController(
      IKcsarContext db,
      IPermissionsService permissions,
      IWebHostingService hosting,
      ILog log)
      : base(db, permissions, hosting, log)
    {
    }

    [HttpGet]
    public IEnumerable<MemberDetail> GetResponderEmails(Guid id, Guid? unitId)
    {
      if (!User.IsInRole("cdb.users")) ThrowAuthError();
      string unit = null;

      var q = db.MissionRosters.Where(f => f.Mission.Id == id);
      if (unitId.HasValue)
      {
        q = q.Where(f => f.Unit.Id == unitId.Value);
        unit = db.Units.Single(f => f.Id == unitId).DisplayName;
      }

      var responders = q.Select(f => new
      {
        Id = f.Person.Id,
        First = f.Person.FirstName,
        Last = f.Person.LastName,
        Email = f.Person.ContactNumbers.Where(g => g.Type == "email").OrderBy(g => g.Priority).FirstOrDefault(),
        Unit = f.Unit.DisplayName
      }).Distinct().OrderBy(f => f.Last).ThenBy(f => f.First).ToArray();

      var model = responders.Select(f => new MemberDetail
      {
        Id = f.Id,
        FirstName = f.First,
        LastName = f.Last,
        Units = new[] { f.Unit },
        Contacts = new[] { f.Email == null ? null : new MemberContact { Id = f.Email.Id, Value = f.Email.Value } }
      });

      return model;
    }
  }
}