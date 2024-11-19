namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class beverage_v10 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderDetailExtra", "ExtraId", "dbo.Extra");
            DropForeignKey("dbo.OrderDetailExtra", "OrderDetailId", "dbo.OrderDetail");
            DropForeignKey("dbo.OrderDetailBeverage", "OrderDetailId", "dbo.OrderDetail");
            DropForeignKey("dbo.OrderDetailBeverage", "ProductId", "dbo.Product");
            DropForeignKey("dbo.BeverageDetails", "OrderDetailBeverageId", "dbo.OrderDetailBeverage");
            DropForeignKey("dbo.BeverageDetails", "SizeId", "dbo.Size");
            DropForeignKey("dbo.BeverageDetails", "ToppingId", "dbo.Topping");
            DropIndex("dbo.BeverageDetails", new[] { "OrderDetailBeverageId" });
            DropIndex("dbo.BeverageDetails", new[] { "SizeId" });
            DropIndex("dbo.BeverageDetails", new[] { "ToppingId" });
            DropIndex("dbo.OrderDetailBeverage", new[] { "OrderDetailId" });
            DropIndex("dbo.OrderDetailBeverage", new[] { "ProductId" });
            DropIndex("dbo.OrderDetailExtra", new[] { "OrderDetailId" });
            DropIndex("dbo.OrderDetailExtra", new[] { "ExtraId" });
            DropTable("dbo.BeverageDetails");
            DropTable("dbo.OrderDetailBeverage");
            DropTable("dbo.OrderDetailExtra");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.OrderDetailExtra",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderDetailId = c.Int(nullable: false),
                        ExtraId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderDetailBeverage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderDetailId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BeverageDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderDetailBeverageId = c.Int(nullable: false),
                        SizeId = c.Int(),
                        ToppingId = c.Int(),
                        IceLevel = c.String(),
                        SweetnessLevel = c.String(),
                        Temperature = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.OrderDetailExtra", "ExtraId");
            CreateIndex("dbo.OrderDetailExtra", "OrderDetailId");
            CreateIndex("dbo.OrderDetailBeverage", "ProductId");
            CreateIndex("dbo.OrderDetailBeverage", "OrderDetailId");
            CreateIndex("dbo.BeverageDetails", "ToppingId");
            CreateIndex("dbo.BeverageDetails", "SizeId");
            CreateIndex("dbo.BeverageDetails", "OrderDetailBeverageId");
            AddForeignKey("dbo.BeverageDetails", "ToppingId", "dbo.Topping", "Id");
            AddForeignKey("dbo.BeverageDetails", "SizeId", "dbo.Size", "Id");
            AddForeignKey("dbo.BeverageDetails", "OrderDetailBeverageId", "dbo.OrderDetailBeverage", "Id", cascadeDelete: true);
            AddForeignKey("dbo.OrderDetailBeverage", "ProductId", "dbo.Product", "Id");
            AddForeignKey("dbo.OrderDetailBeverage", "OrderDetailId", "dbo.OrderDetail", "Id");
            AddForeignKey("dbo.OrderDetailExtra", "OrderDetailId", "dbo.OrderDetail", "Id", cascadeDelete: true);
            AddForeignKey("dbo.OrderDetailExtra", "ExtraId", "dbo.Extra", "Id", cascadeDelete: true);
        }
    }
}
