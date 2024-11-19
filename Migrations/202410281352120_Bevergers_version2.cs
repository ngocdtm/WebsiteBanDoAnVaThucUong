namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Bevergers_version2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Size", "OrderDetailBeverage_Id", "dbo.OrderDetailBeverage");
            DropForeignKey("dbo.Topping", "OrderDetailBeverage_Id", "dbo.OrderDetailBeverage");
            DropIndex("dbo.Size", new[] { "OrderDetailBeverage_Id" });
            DropIndex("dbo.Topping", new[] { "OrderDetailBeverage_Id" });
            DropColumn("dbo.Size", "OrderDetailBeverage_Id");
            DropColumn("dbo.Topping", "OrderDetailBeverage_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Topping", "OrderDetailBeverage_Id", c => c.Int());
            AddColumn("dbo.Size", "OrderDetailBeverage_Id", c => c.Int());
            CreateIndex("dbo.Topping", "OrderDetailBeverage_Id");
            CreateIndex("dbo.Size", "OrderDetailBeverage_Id");
            AddForeignKey("dbo.Topping", "OrderDetailBeverage_Id", "dbo.OrderDetailBeverage", "Id");
            AddForeignKey("dbo.Size", "OrderDetailBeverage_Id", "dbo.OrderDetailBeverage", "Id");
        }
    }
}
