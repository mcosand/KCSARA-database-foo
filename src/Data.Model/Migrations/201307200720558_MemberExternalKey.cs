namespace Kcsar.Database.Model.Migrations
{
  using System.Data.Entity.Migrations;
    
    public partial class MemberExternalKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Members", "ExternalKey1", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Members", "ExternalKey1");
        }
    }
}
