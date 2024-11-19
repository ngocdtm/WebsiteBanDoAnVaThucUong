namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Bevergers_v5 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.ProductExtra");
            DropPrimaryKey("dbo.ProductSize");
            DropPrimaryKey("dbo.ProductTopping");
            AddPrimaryKey("dbo.ProductExtra", new[] { "ProductId", "ExtraId" });
            AddPrimaryKey("dbo.ProductSize", new[] { "ProductId", "SizeId" });
            AddPrimaryKey("dbo.ProductTopping", new[] { "ProductId", "ToppingId" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.ProductTopping");
            DropPrimaryKey("dbo.ProductSize");
            DropPrimaryKey("dbo.ProductExtra");
            AddPrimaryKey("dbo.ProductTopping", "Id");
            AddPrimaryKey("dbo.ProductSize", "Id");
            AddPrimaryKey("dbo.ProductExtra", "Id");
        }
    }
}
