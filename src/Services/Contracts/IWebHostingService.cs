/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Services
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public interface IWebHostingService : IHostingService
  {
    // new Uri(this.Request.RequestUri, Url.Route("Default", new { httproute = "", controller = "Account", action = "Verify", id = data.Username })).AbsoluteUri
    string GetApiUrl(string controller, string action, object id, bool asAbsolute);
  }
}
