/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Web.Api
{
  using System;
  using System.Net;
  using System.Net.Http;
  using System.Web.Http;
  using Kcsar.Database.Model;
  using Kcsar.Database.Services;
  using log4net;

  public abstract class DatabaseApiController : BaseApiController, IDisposable
  {
    protected readonly IKcsarContext db;

    public DatabaseApiController(IKcsarContext db, IPermissionsService permissions, IWebHostingService hosting, ILog log)
      : base(permissions, hosting, log)
    {
      this.db = db;
    }

    //public const string ModelRootNodeName = "_root";

    protected override void Dispose(bool disposing)
    {
      IDisposable disposableDb = this.db as IDisposable;
      if (disposing && disposableDb != null) disposableDb.Dispose();
      base.Dispose(disposing);
    }

    protected T GetObjectOrNotFound<T>(Func<T> getter)
    {
      var result = getter();
      if (result == null)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
      }
      return result;
    }
  }
}