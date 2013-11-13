using System.ComponentModel.DataAnnotations;
namespace Kcsar.Database.Model
{

  public class AnimalMission : ModelObject
  {
    [Required]
    public virtual Animal Animal { get; set; }
    
    [Required]
    public virtual MissionRoster MissionRoster { get; set; }

    public override string GetReportHtml()
    {
      return string.Format("<b>[{0}] [{1}]</b> In:{2} Out:{3}", this.MissionRoster.Mission.Title, this.Animal.Name, this.MissionRoster.TimeIn, this.MissionRoster.TimeOut);
    }
  }
}
