
namespace Kcsar.Database.Model
{
  using Microsoft.SqlServer.Types;
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.SqlTypes;

    public class MissionGeography : ModelObject
    {
        public Guid? InstanceId { get; set; }
        public string Kind { get; set; }
        public DateTime? Time { get; set; }
        public string Description { get; set; }
        public string LocationBinary { get; set; }
        public string LocationText { get; set; }
        public virtual Mission Mission { get; set; }

        private SqlGeography geog = null;
        [NotMapped]
        public SqlGeography Geography
        {
            get
            {
                if (geog == null)
                {
                    geog = this.LocationBinary == null ? null : SqlGeography.STGeomFromText(new SqlChars(this.LocationBinary.ToCharArray()), 4326);
                }
                return geog;
            }
            set
            {
                geog = value;
                this.LocationBinary = geog.ToString();
            }
        }

        public override string GetReportHtml()
        {
            return "report";
        }
    }
}
