namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "StoreId", c => c.Int(nullable: false));
            AddColumn("dbo.Store", "Order_Id", c => c.Int());
            CreateIndex("dbo.Store", "Order_Id");
            AddForeignKey("dbo.Store", "Order_Id", "dbo.Order", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Store", "Order_Id", "dbo.Order");
            DropIndex("dbo.Store", new[] { "Order_Id" });
            DropColumn("dbo.Store", "Order_Id");
            DropColumn("dbo.Order", "StoreId");
        }
    }
}
