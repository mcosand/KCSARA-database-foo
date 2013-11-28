/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Web.Api
{
  using System;
  using Kcsar.Database.Model;
  using Kcsar.Database.Services;
  using log4net;

  public class TrainingDocumentsController : DocumentsController
  {
    public TrainingDocumentsController(IDocumentsService documents, IPermissionsService permissions, IKcsarContext db, ILog log)
      : base(documents, permissions, db, log)
    {
    }

    protected override bool CanAddDocuments(Guid id)
    {
      return User.IsInRole("cdb.trainingeditors");
    }

    protected override string DocumentType
    {
      get { return "award"; }
    }
  }
}