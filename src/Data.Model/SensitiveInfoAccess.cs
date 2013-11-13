using System;
using System.ComponentModel.DataAnnotations;

namespace Kcsar.Database.Model
{
    public class SensitiveInfoAccess
    {
        public SensitiveInfoAccess()
        {
            this.Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        public string Actor { get; set; }
        public DateTime Timestamp { get; set; }
        public Member Owner { get; set; }
        public string Action { get; set; }
        public string Reason { get; set; }
    }
}
