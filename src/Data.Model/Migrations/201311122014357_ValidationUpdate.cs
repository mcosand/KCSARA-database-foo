namespace Kcsar.Database.Model.Migrations
{
  using System.Data.Entity.Migrations;
    
    public partial class ValidationUpdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AnimalOwners", "Animal_Id", "dbo.Animals");
            DropForeignKey("dbo.AnimalMissions", "Animal_Id", "dbo.Animals");
            DropForeignKey("dbo.AnimalMissions", "MissionRoster_Id", "dbo.MissionRosters");
            DropIndex("dbo.AnimalOwners", new[] { "Animal_Id" });
            DropIndex("dbo.AnimalMissions", new[] { "Animal_Id" });
            DropIndex("dbo.AnimalMissions", new[] { "MissionRoster_Id" });
            AlterColumn("dbo.AnimalOwners", "Animal_Id", c => c.Guid(nullable: false));
            AlterColumn("dbo.TrainingCourses", "FullName", c => c.String(nullable: false));
            AlterColumn("dbo.UnitContacts", "Type", c => c.String(nullable: false));
            AlterColumn("dbo.UnitContacts", "Value", c => c.String(nullable: false));
            AlterColumn("dbo.AnimalMissions", "Animal_Id", c => c.Guid(nullable: false));
            AlterColumn("dbo.AnimalMissions", "MissionRoster_Id", c => c.Guid(nullable: false));
            AlterColumn("dbo.PersonContacts", "Type", c => c.String(nullable: false));
            AlterColumn("dbo.PersonContacts", "Value", c => c.String(nullable: false));
            CreateIndex("dbo.AnimalOwners", "Animal_Id");
            CreateIndex("dbo.AnimalMissions", "Animal_Id");
            CreateIndex("dbo.AnimalMissions", "MissionRoster_Id");
            AddForeignKey("dbo.AnimalOwners", "Animal_Id", "dbo.Animals", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AnimalMissions", "Animal_Id", "dbo.Animals", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AnimalMissions", "MissionRoster_Id", "dbo.MissionRosters", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AnimalMissions", "MissionRoster_Id", "dbo.MissionRosters");
            DropForeignKey("dbo.AnimalMissions", "Animal_Id", "dbo.Animals");
            DropForeignKey("dbo.AnimalOwners", "Animal_Id", "dbo.Animals");
            DropIndex("dbo.AnimalMissions", new[] { "MissionRoster_Id" });
            DropIndex("dbo.AnimalMissions", new[] { "Animal_Id" });
            DropIndex("dbo.AnimalOwners", new[] { "Animal_Id" });
            AlterColumn("dbo.PersonContacts", "Value", c => c.String());
            AlterColumn("dbo.PersonContacts", "Type", c => c.String());
            AlterColumn("dbo.AnimalMissions", "MissionRoster_Id", c => c.Guid());
            AlterColumn("dbo.AnimalMissions", "Animal_Id", c => c.Guid());
            AlterColumn("dbo.UnitContacts", "Value", c => c.String());
            AlterColumn("dbo.UnitContacts", "Type", c => c.String());
            AlterColumn("dbo.TrainingCourses", "FullName", c => c.String());
            AlterColumn("dbo.AnimalOwners", "Animal_Id", c => c.Guid());
            CreateIndex("dbo.AnimalMissions", "MissionRoster_Id");
            CreateIndex("dbo.AnimalMissions", "Animal_Id");
            CreateIndex("dbo.AnimalOwners", "Animal_Id");
            AddForeignKey("dbo.AnimalMissions", "MissionRoster_Id", "dbo.MissionRosters", "Id");
            AddForeignKey("dbo.AnimalMissions", "Animal_Id", "dbo.Animals", "Id");
            AddForeignKey("dbo.AnimalOwners", "Animal_Id", "dbo.Animals", "Id");
        }
    }
}
