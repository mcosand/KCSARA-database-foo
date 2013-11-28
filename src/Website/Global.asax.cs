

namespace Kcsar.Database.Web
{
  using System.Web.Http;
  using System.Web.Mvc;
  using System.Web.Routing;
  using Ninject;

  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801
  public class MvcApplication : Ninject.Web.Common.NinjectHttpApplication
  {
    private static IKernel defaultKernel;
    static MvcApplication()
    {
      MvcApplication.defaultKernel = new StandardKernel();
    }

    protected readonly IKernel kernel;
    public MvcApplication(IKernel kernel) : base()
    {
      this.kernel = kernel;
    }

    public MvcApplication() : this(MvcApplication.defaultKernel)
    {
    }

    protected override void OnApplicationStarted()
    {
      AreaRegistration.RegisterAllAreas();

      WebApiConfig.Register(GlobalConfiguration.Configuration, this.kernel);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
    }

    protected override IKernel CreateKernel()
    {
      return this.kernel;
    }
  }
}