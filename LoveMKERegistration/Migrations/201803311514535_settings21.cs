namespace LoveMKERegistration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class settings21 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.SettingsModels");
            AlterColumn("dbo.SettingsModels", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.SettingsModels", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.SettingsModels");
            AlterColumn("dbo.SettingsModels", "Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.SettingsModels", "Id");
        }
    }
}
