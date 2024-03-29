﻿/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Web.Api
{
  using System;
  using System.Collections.Generic;
  using System.Drawing;
  using System.Drawing.Imaging;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Net.Http;
  using System.Net.Http.Formatting;
  using System.Net.Http.Headers;
  using System.Threading.Tasks;
  using System.Web;
  using System.Web.Http;
  using Kcsar.Database.Services;
  using Kcsar.Database.Website.Api.Models;
  using log4net;
  using Data = Kcsar.Database.Model;
  
  public abstract class DocumentsController : DatabaseApiController
  {
    protected readonly IDocumentsService documents;
    public DocumentsController(
      IDocumentsService documents,
      Data.IKcsarContext db,
      IPermissionsService permissions,
      IWebHostingService hosting,
      ILog log)
      : base(db, permissions, hosting, log)
    {
      this.documents = documents;
    }

    [HttpGet]
    public IEnumerable<Document> GetList(Guid id)
    {

      var model = db.Documents.Where(f => f.ReferenceId == id).OrderBy(f => f.FileName).AsEnumerable().Select(d => GetView(d, id));
      return model;
    }

    private Document GetView(Data.Document d, Guid reference)
    {
      return new Document
          {
            Id = d.Id,
            Reference = reference,
            Title = d.FileName,
            Size = d.Size,
            Type = d.Type,
            Mime = d.MimeType,
            Url = Url.Route("defaultApi", new { controller = this.ControllerContext.ControllerDescriptor.ControllerName, action = "Get", id = d.Id }),
            Thumbnail = Url.Route("defaultApi", new { controller = this.ControllerContext.ControllerDescriptor.ControllerName, action = "Thumbnail", id = d.Id })
          };
    }

    public HttpResponseMessage Get(Guid id)
    {
      string name;
      string mime;

      var doc = GetObjectOrNotFound(() => (from d in db.Documents where d.Id == id select d).FirstOrDefault());

      name = doc.FileName.Replace(" ", "_");
      mime = string.IsNullOrWhiteSpace(doc.MimeType) ? this.documents.GuessMime(name) : doc.MimeType;

      var response = new HttpResponseMessage();
      response.Content = new StreamContent(this.documents.GetDocumentStream(doc));
      response.Content.Headers.ContentType = new MediaTypeHeaderValue(mime);
      response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = name };

      return response;
    }


    [HttpGet]
    public HttpResponseMessage Thumbnail(Guid id, int? maxSize)
    {
      HttpResponseMessage response = new HttpResponseMessage();

      var thumb = (from d in db.Documents where d.Id == id select new { file = d.FileName, store = d.StorePath, type = d.MimeType }).First();

      string file = Data.Document.StorageRoot + thumb.store;
      string mime = thumb.type ?? this.documents.GuessMime(thumb.file);
      if (mime.StartsWith("image", StringComparison.OrdinalIgnoreCase))
      {
        if (!File.Exists(file))
        {
          file = HttpContext.Current.Server.MapPath("~/Content/images/deleted.png");
        }
      }
      else
      {
        file = HttpContext.Current.Server.MapPath("~/Content/images/mime/" + mime.Replace("/", "_") + ".png");
        if (!File.Exists(file))
        {
          file = HttpContext.Current.Server.MapPath("~/Content/images/mime/unknown.png");
        }
      }

      using (Image img = Image.FromFile(file))
      {
        double s = (double)(maxSize ?? 100);
        int h = (int)s;
        int w = (int)((double)img.Width / (double)img.Height * s);
        if (w > h)
        {
          h = (int)(s / w * s);
          w = (int)s;
        }

        string tempFile = System.IO.Path.GetTempFileName();
        var thumbnail = img.GetThumbnailImage(w, h, null, new IntPtr());
        MemoryStream stream = new MemoryStream();
        thumbnail.Save(stream, ImageFormat.Jpeg);
        stream.Position = 0;

        response.Content = new StreamContent(stream);
        response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
      }

      return response;
    }

    protected abstract bool CanAddDocuments(Guid id);

    protected abstract string DocumentType { get; }

    [HttpPost]
    public Task<HttpResponseMessage> PutFiles(Guid id)
    {
      if (!CanAddDocuments(id))
      {
        base.ThrowAuthError();
      }

      if (!Request.Content.IsMimeMultipartContent())
      {
        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
      }

      var task = Request.Content.ReadAsMultipartAsync().ContinueWith<HttpResponseMessage>(f =>
      {
        List<Document> results = new List<Document>();

        foreach (var file in f.Result.Contents.Cast<StreamContent>().Where(g =>
            g.Headers.ContentDisposition.Name.Trim('"') == "files[]"))
        {
          foreach (var d in this.documents.ReceiveDocument(
              file.ReadAsStreamAsync().Result,
              file.Headers.ContentDisposition.FileName.Trim('"'),
              (int)file.Headers.ContentLength.Value,
              db,
              id,
              this.DocumentType
              ))
          {
            results.Add(GetView(d, id));
          }

        }

        IContentNegotiator negotiator = Configuration.Services.GetContentNegotiator();
        ContentNegotiationResult result = negotiator.Negotiate(typeof(Document), Request, Configuration.Formatters);

        db.SaveChanges();
        return new HttpResponseMessage
        {
          Content = new ObjectContent<IEnumerable<Document>>(results.ToArray(), result.Formatter, "text/plain")
        };
      });

      return task;
      //var data = Request.Content.ReadAsMultipartAsync();

      //List<Document> results = new List<Document>();

      //foreach (var file in data.Result.Contents.Cast<StreamContent>().Where(f => f.Headers.ContentDisposition.Name.Trim('"') == "files[]"))
      //{
      //    foreach (var d in Documents.ReceiveDocument(
      //        file.ReadAsStreamAsync().Result,
      //        file.Headers.ContentDisposition.FileName.Trim('"'),
      //        (int)file.Headers.ContentLength.Value,
      //        db,
      //        id,
      //        this.DocumentType
      //        ))
      //    {
      //        results.Add(GetView(d, id));
      //    }

      //}

      //IContentNegotiator negotiator = Configuration.Services.GetContentNegotiator();
      //ContentNegotiationResult result = negotiator.Negotiate(typeof(Document), Request, Configuration.Formatters);

      //db.SaveChanges();
      //return new HttpResponseMessage()
      //{
      //    Content = new ObjectContent<IEnumerable<Document>>(
      //        results.ToArray(), result.Formatter, "text/plain"
      //         )
      //};
    }

    [HttpPost]
    public void Delete(Guid id)
    {
      var doc = GetObjectOrNotFound(() => db.Documents.FirstOrDefault(f => f.Id == id && f.Type == "award"));

      if (!CanAddDocuments(doc.ReferenceId))
      {
        base.ThrowAuthError();
      }

      db.Documents.Remove(doc);
      db.SaveChanges();
    }
  }
}