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

  public interface IHostingService
  {
    /// <summary>
    /// Read the contents of a file.
    /// </summary>
    /// <param name="path">The path to the file, relative to the site root.</param>
    /// <returns>The string contents of the file.</returns>
    string ReadFile(string path);

    /// <summary>Gets the SMTP address someone should use to contact a site administrator.</summary>
    string FeedbackAddress { get; }

    /// <summary>Gets the SMTP address of the account that will send mail from the database.</summary>
    string FromAddress { get; }
  }
}
