namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sizes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Size", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Size", "ApplicationUser_Id");
            AddForeignKey("dbo.Size", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Size", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Size", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Size", "ApplicationUser_Id");
        }
    }
}
