namespace LoveMKERegistration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class settings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SettingsModels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        HasTShirts = c.Boolean(nullable: false),
                        Logo = c.Binary(),
                        Background = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SettingsModels");
        }
    }
}
