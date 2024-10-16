namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class getData : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StoreProduct",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    StoreId = c.Int(nullable: false),
                    ProductId = c.Int(nullable: false),
                    StockCount = c.Int(nullable: false),
                    SellCount = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Product", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Store", t => t.StoreId, cascadeDelete: true)
                .Index(t => t.StoreId)
                .Index(t => t.ProductId);

            AddColumn("dbo.Order", "StoreId", c => c.Int(nullable: false));
            AddColumn("dbo.Store", "Order_Id", c => c.Int());
            CreateIndex("dbo.Store", "Order_Id");
            AddForeignKey("dbo.Store", "Order_Id", "dbo.Order", "Id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Store", "Order_Id", "dbo.Order");
            DropForeignKey("dbo.StoreProduct", "StoreId", "dbo.Store");
            DropForeignKey("dbo.StoreProduct", "ProductId", "dbo.Product");
            DropIndex("dbo.Store", new[] { "Order_Id" });
            DropIndex("dbo.StoreProduct", new[] { "ProductId" });
            DropIndex("dbo.StoreProduct", new[] { "StoreId" });
            DropColumn("dbo.Store", "Order_Id");
            DropColumn("dbo.Order", "StoreId");
            DropTable("dbo.StoreProduct");
        }
    }
}

