namespace Kcsar.Database.Web
{
  using System.Web.Http;
  using Kcsar.Database.Web.Api;
  using Newtonsoft.Json.Converters;
  using Ninject;

  /// <summary>Configure API settings for the basic application</summary>
  public static class WebApiConfig
  {
    /// <summary>Configures API settings for the basic application.</summary>
    /// <param name="config">Configuration of the application.</param>
    /// <param name="kernel">Composition root.</param>
    public static void Register(HttpConfiguration config, IKernel kernel)
    {
      config.DependencyResolver = new NinjectResolver(kernel);

      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{action}/{value}",
          defaults: new { value = RouteParameter.Optional }
      );

      config.Filters.Add(new ExceptionLoggingFilter());
      config.Filters.Add(new AuthorizeAttribute());

      config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
    }
  }
}
