namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Bevergers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Size", "ProductId", "dbo.Product");
            DropForeignKey("dbo.Topping", "ProductId", "dbo.Product");
            DropForeignKey("dbo.Size", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Size", new[] { "ProductId" });
            DropIndex("dbo.Size", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Topping", new[] { "ProductId" });
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
                "dbo.ProductType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Alias = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductExtra",
                c => new
                    {
                        ProductId = c.Int(nullable: false),
                        ExtraId = c.Int(nullable: false),
                        Id = c.Int(nullable: false, identity: true),
                        IsRecommended = c.Boolean(nullable: false),
                        SpecialPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.ProductId, t.ExtraId })
                .ForeignKey("dbo.Extra", t => t.ExtraId, cascadeDelete: true)
                .ForeignKey("dbo.Product", t => t.ProductId)
                .Index(t => t.ProductId)
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
                "dbo.ProductSize",
                c => new
                    {
                        ProductId = c.Int(nullable: false),
                        SizeId = c.Int(nullable: false),
                        Id = c.Int(nullable: false, identity: true),
                        IsRecommended = c.Boolean(nullable: false),
                        SpecialPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.ProductId, t.SizeId })
                .ForeignKey("dbo.Product", t => t.ProductId)
                .ForeignKey("dbo.Size", t => t.SizeId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.SizeId);
            
            CreateTable(
                "dbo.ProductTopping",
                c => new
                    {
                        ProductId = c.Int(nullable: false),
                        ToppingId = c.Int(nullable: false),
                        Id = c.Int(nullable: false, identity: true),
                        IsRecommended = c.Boolean(nullable: false),
                        SpecialPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.ProductId, t.ToppingId })
                .ForeignKey("dbo.Product", t => t.ProductId)
                .ForeignKey("dbo.Topping", t => t.ToppingId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.ToppingId);

            //////////////// HERE
            // 1. Chèn dữ liệu mặc định vào ProductType
            Sql("INSERT INTO dbo.ProductType (Name, Alias) VALUES ('Food', 'food'), ('Beverage', 'beverage')");
            // 2. Thêm cột ProductTypeId vào ProductCategory với giá trị mặc định là 1 (food)
            AddColumn("dbo.ProductCategory", "ProductTypeId", c => c.Int(nullable: false, defaultValue: 1));
            // 3. Cập nhật giá trị ProductTypeId cho các bản ghi hiện có
            Sql("UPDATE dbo.ProductCategory SET ProductTypeId = 1");
            ////////////////STOP
            
            AddColumn("dbo.AspNetUsers", "Phone", c => c.String());
            AddColumn("dbo.OrderDetail", "SelectedSizeIds", c => c.String());
            AddColumn("dbo.OrderDetail", "SelectedToppingIds", c => c.String());
            AddColumn("dbo.OrderDetail", "SelectedExtraIds", c => c.String());
            AddColumn("dbo.OrderDetail", "SizePrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.OrderDetail", "ToppingPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.OrderDetail", "ExtraPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("dbo.ProductCategory", "ProductTypeId");
            AddForeignKey("dbo.ProductCategory", "ProductTypeId", "dbo.ProductType", "Id", cascadeDelete: true);
            DropColumn("dbo.Size", "ProductId");
            DropColumn("dbo.Size", "ApplicationUser_Id");
            DropColumn("dbo.Topping", "ProductId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Topping", "ProductId", c => c.Int(nullable: false));
            AddColumn("dbo.Size", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Size", "ProductId", c => c.Int(nullable: false));
            DropForeignKey("dbo.ProductTopping", "ToppingId", "dbo.Topping");
            DropForeignKey("dbo.ProductTopping", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ProductSize", "SizeId", "dbo.Size");
            DropForeignKey("dbo.ProductSize", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ProductExtra", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ProductExtra", "ExtraId", "dbo.Extra");
            DropForeignKey("dbo.ComboDetail", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ComboDetail", "ComboId", "dbo.Combo");
            DropForeignKey("dbo.Combo", "ProductCategoryId", "dbo.ProductCategory");
            DropForeignKey("dbo.ProductCategory", "ProductTypeId", "dbo.ProductType");
            DropIndex("dbo.ProductTopping", new[] { "ToppingId" });
            DropIndex("dbo.ProductTopping", new[] { "ProductId" });
            DropIndex("dbo.ProductSize", new[] { "SizeId" });
            DropIndex("dbo.ProductSize", new[] { "ProductId" });
            DropIndex("dbo.ProductExtra", new[] { "ExtraId" });
            DropIndex("dbo.ProductExtra", new[] { "ProductId" });
            DropIndex("dbo.ProductCategory", new[] { "ProductTypeId" });
            DropIndex("dbo.Combo", new[] { "ProductCategoryId" });
            DropIndex("dbo.ComboDetail", new[] { "ProductId" });
            DropIndex("dbo.ComboDetail", new[] { "ComboId" });
            DropColumn("dbo.ProductCategory", "ProductTypeId");
            DropColumn("dbo.OrderDetail", "ExtraPrice");
            DropColumn("dbo.OrderDetail", "ToppingPrice");
            DropColumn("dbo.OrderDetail", "SizePrice");
            DropColumn("dbo.OrderDetail", "SelectedExtraIds");
            DropColumn("dbo.OrderDetail", "SelectedToppingIds");
            DropColumn("dbo.OrderDetail", "SelectedSizeIds");
            DropColumn("dbo.AspNetUsers", "Phone");
            DropTable("dbo.ProductTopping");
            DropTable("dbo.ProductSize");
            DropTable("dbo.Extra");
            DropTable("dbo.ProductExtra");
            DropTable("dbo.ProductType");
            DropTable("dbo.Combo");
            DropTable("dbo.ComboDetail");
            CreateIndex("dbo.Topping", "ProductId");
            CreateIndex("dbo.Size", "ApplicationUser_Id");
            CreateIndex("dbo.Size", "ProductId");
            AddForeignKey("dbo.Size", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Topping", "ProductId", "dbo.Product", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Size", "ProductId", "dbo.Product", "Id", cascadeDelete: true);
        }
    }
}
