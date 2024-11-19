namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Bevergers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BeverageDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderDetailBeverageId = c.Int(nullable: false),
                        SizeId = c.Int(nullable: false),
                        ToppingId = c.Int(nullable: false),
                        IceLevel = c.String(),
                        SweetnessLevel = c.String(),
                        Temperature = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderDetailBeverage", t => t.OrderDetailBeverageId, cascadeDelete: true)
                .ForeignKey("dbo.Size", t => t.SizeId, cascadeDelete: true)
                .ForeignKey("dbo.Topping", t => t.ToppingId, cascadeDelete: true)
                .Index(t => t.OrderDetailBeverageId)
                .Index(t => t.SizeId)
                .Index(t => t.ToppingId);
            
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderDetail", t => t.OrderDetailId)
                .ForeignKey("dbo.Product", t => t.ProductId)
                .Index(t => t.OrderDetailId)
                .Index(t => t.ProductId);
            
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Extra", t => t.ExtraId, cascadeDelete: true)
                .ForeignKey("dbo.OrderDetail", t => t.OrderDetailId, cascadeDelete: true)
                .Index(t => t.OrderDetailId)
                .Index(t => t.ExtraId);
            
            CreateTable(
                "dbo.Extra",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductExtra",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        ExtraId = c.Int(nullable: false),
                        IsRecommended = c.Boolean(nullable: false),
                        SpecialPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Extra", t => t.ExtraId, cascadeDelete: true)
                .ForeignKey("dbo.Product", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.ExtraId);
            
            CreateTable(
                "dbo.ComboDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ComboId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        IsRequired = c.Boolean(nullable: false),
                        AdditionalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Combo", t => t.ComboId, cascadeDelete: true)
                .ForeignKey("dbo.Product", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ComboId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Combo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        Description = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        ProductCategoryId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductCategory", t => t.ProductCategoryId)
                .Index(t => t.ProductCategoryId);
            
            CreateTable(
                "dbo.ProductSize",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        SizeId = c.Int(nullable: false),
                        IsRecommended = c.Boolean(nullable: false),
                        SpecialPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Product", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Size", t => t.SizeId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.SizeId);
            
            CreateTable(
                "dbo.Size",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NameSize = c.String(nullable: false, maxLength: 50),
                        PriceSize = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderDetailBeverage_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderDetailBeverage", t => t.OrderDetailBeverage_Id)
                .Index(t => t.OrderDetailBeverage_Id);
            
            CreateTable(
                "dbo.ProductTopping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        ToppingId = c.Int(nullable: false),
                        IsRecommended = c.Boolean(nullable: false),
                        SpecialPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Product", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Topping", t => t.ToppingId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.ToppingId);
            
            CreateTable(
                "dbo.Topping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NameTopping = c.String(nullable: false, maxLength: 100),
                        PriceTopping = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderDetailBeverage_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderDetailBeverage", t => t.OrderDetailBeverage_Id)
                .Index(t => t.OrderDetailBeverage_Id);
            
            CreateTable(
                "dbo.ProductType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Alias = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BeverageDetails", "ToppingId", "dbo.Topping");
            DropForeignKey("dbo.BeverageDetails", "SizeId", "dbo.Size");
            DropForeignKey("dbo.BeverageDetails", "OrderDetailBeverageId", "dbo.OrderDetailBeverage");
            DropForeignKey("dbo.Topping", "OrderDetailBeverage_Id", "dbo.OrderDetailBeverage");
            DropForeignKey("dbo.Size", "OrderDetailBeverage_Id", "dbo.OrderDetailBeverage");
            DropForeignKey("dbo.OrderDetailBeverage", "ProductId", "dbo.Product");
            DropForeignKey("dbo.OrderDetailBeverage", "OrderDetailId", "dbo.OrderDetail");
            DropForeignKey("dbo.OrderDetailExtra", "OrderDetailId", "dbo.OrderDetail");
            DropForeignKey("dbo.OrderDetailExtra", "ExtraId", "dbo.Extra");
            DropForeignKey("dbo.ProductExtra", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ProductTopping", "ToppingId", "dbo.Topping");
            DropForeignKey("dbo.ProductTopping", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ProductSize", "SizeId", "dbo.Size");
            DropForeignKey("dbo.ProductSize", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ComboDetail", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ComboDetail", "ComboId", "dbo.Combo");
            DropForeignKey("dbo.Combo", "ProductCategoryId", "dbo.ProductCategory");
            DropForeignKey("dbo.ProductExtra", "ExtraId", "dbo.Extra");
            DropIndex("dbo.Topping", new[] { "OrderDetailBeverage_Id" });
            DropIndex("dbo.ProductTopping", new[] { "ToppingId" });
            DropIndex("dbo.ProductTopping", new[] { "ProductId" });
            DropIndex("dbo.Size", new[] { "OrderDetailBeverage_Id" });
            DropIndex("dbo.ProductSize", new[] { "SizeId" });
            DropIndex("dbo.ProductSize", new[] { "ProductId" });
            DropIndex("dbo.Combo", new[] { "ProductCategoryId" });
            DropIndex("dbo.ComboDetail", new[] { "ProductId" });
            DropIndex("dbo.ComboDetail", new[] { "ComboId" });
            DropIndex("dbo.ProductExtra", new[] { "ExtraId" });
            DropIndex("dbo.ProductExtra", new[] { "ProductId" });
            DropIndex("dbo.OrderDetailExtra", new[] { "ExtraId" });
            DropIndex("dbo.OrderDetailExtra", new[] { "OrderDetailId" });
            DropIndex("dbo.OrderDetailBeverage", new[] { "ProductId" });
            DropIndex("dbo.OrderDetailBeverage", new[] { "OrderDetailId" });
            DropIndex("dbo.BeverageDetails", new[] { "ToppingId" });
            DropIndex("dbo.BeverageDetails", new[] { "SizeId" });
            DropIndex("dbo.BeverageDetails", new[] { "OrderDetailBeverageId" });
            DropTable("dbo.ProductType");
            DropTable("dbo.Topping");
            DropTable("dbo.ProductTopping");
            DropTable("dbo.Size");
            DropTable("dbo.ProductSize");
            DropTable("dbo.Combo");
            DropTable("dbo.ComboDetail");
            DropTable("dbo.ProductExtra");
            DropTable("dbo.Extra");
            DropTable("dbo.OrderDetailExtra");
            DropTable("dbo.OrderDetailBeverage");
            DropTable("dbo.BeverageDetails");
        }
    }
}
