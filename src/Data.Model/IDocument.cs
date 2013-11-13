using System;

namespace Kcsar.Database.Model
{
    public interface IDocument
    {
        Guid Id { get; }
        string Type { get; set; }
        Guid ReferenceId { get; set; }
        string FileName { get; set; }
        string MimeType { get; set; }
    }
}
