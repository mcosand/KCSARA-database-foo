/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Web.Api
{
  using System.Net;
  using System.Net.Http;
  using System.Web.Http;
  using System.Web.Http.Filters;

  /// <summary>Logs exceptions triggered from within an Api action method.</summary>
  public class ExceptionLoggingFilter : ExceptionFilterAttribute
  {
    /// <summary>Raises the exception event.</summary>
    /// <param name="context">The context for the action.</param>
    public override void OnException(HttpActionExecutedContext context)
    {
      // Assuming the API controller derives from TcaApiController, log the error using its logger.
      DatabaseApiController controller = context.ActionContext.ControllerContext.Controller as DatabaseApiController;
      if (controller != null)
      {
        controller.log.Error("API Unhandled Exception", context.Exception);
      }

      // Hide some details from the caller.
      throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
      {
        Content = new StringContent(WebStrings.ServerError_Api),
        ReasonPhrase = WebStrings.ServerError_Reason
      });
    }
  }
}