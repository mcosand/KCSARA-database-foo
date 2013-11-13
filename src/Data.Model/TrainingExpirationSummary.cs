using System;
using System.ComponentModel.DataAnnotations;

namespace Kcsar.Database.Model
{
    public class TrainingExpirationSummary
    {
        [Key]
        public Guid CourseId { get; set; }
        public int Expired { get; set; }
        public int Recent { get; set; }
        public int Almost { get; set; }
        public int Good { get; set; }
    }
}
