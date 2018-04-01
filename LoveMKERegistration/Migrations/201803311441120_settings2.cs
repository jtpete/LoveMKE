namespace LoveMKERegistration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class settings2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SettingsModels", "LogoName", c => c.String());
            AddColumn("dbo.SettingsModels", "BackgroundName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SettingsModels", "BackgroundName");
            DropColumn("dbo.SettingsModels", "LogoName");
        }
    }
}
