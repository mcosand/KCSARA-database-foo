/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Web.Api
{
  using System.Web.Http;
  using Kcsar.Database.Services;
  using Kcsar.Database.Web.Api.Models;
  using log4net;
  
  /// <summary>Provides telemetry back to server. Not for general use.</summary>
  public class TelemetryController : BaseApiController
  {
        public TelemetryController(IPermissionsService permissions, IWebHostingService hosting, ILog log)
      : base(permissions, hosting, log)
    {
    }

    /// <summary>Client trapped a generic unhandled error.</summary>
    /// <param name="error">Error details.</param>
    /// <returns>true</returns>
    public bool Error([FromBody]TelemetryError error)
    {
      log.DebugFormat("{3} CLIENT ERROR: {0} // {1} // {2} - {4}", error.Error, error.Message, error.Location, User.Identity.Name, Request.Headers.UserAgent);
      return true;
    }
  }
}