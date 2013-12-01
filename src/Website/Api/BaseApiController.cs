/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Web.Api
{
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.Net;
  using System.Net.Http;
  using System.Web.Http;
  using Kcsar.Database.Services;
  using log4net;
  using Newtonsoft.Json;
  
  public abstract class BaseApiController : ApiController
    {
    public readonly ILog log;
    protected readonly IPermissionsService permissions;
    protected readonly IWebHostingService hosting;

    public BaseApiController(IPermissionsService permissions, IWebHostingService hosting, ILog log)
    {
      this.log = log;
      this.permissions = permissions;
      this.hosting = hosting;
    }

    protected void ThrowAuthError()
    {
      log.WarnFormat("AUTH ERR: {0} {1}", User.Identity.Name, Request.RequestUri);
      throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
    }

    protected void ThrowSubmitErrors(IEnumerable<ValidationResult> errors)
    {
      log.WarnFormat("{0} {1} {2}", Request.RequestUri, User.Identity.Name, JsonConvert.SerializeObject(errors));
      // TODO: Copy validation errors to the client.
      throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "NYI"));
    }

    protected string GetDateFormat()
    {
      return "{0:yyyy-MM-dd}";
    }
  }
}