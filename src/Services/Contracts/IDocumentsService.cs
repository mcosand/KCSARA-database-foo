/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Services
{
  using System;
  using System.IO;
  using System.Web;
  using Kcsar.Database.Model;

  public interface IDocumentsService
  {
    Document ProcessImage(Stream source, IKcsarContext ctx, string filename, Guid trainingId);
    Document[] ReceiveDocument(Stream contentStream, string filename, int length, IKcsarContext ctx, Guid reference, string type);
    void ReceiveDocuments(HttpFileCollectionBase files, IKcsarContext ctx, Guid reference, string type);
    Stream GetDocumentStream(Document doc);
    string GuessMime(string filename);
  }
}
