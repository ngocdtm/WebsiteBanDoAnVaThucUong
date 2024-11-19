namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Bevergers_v6 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductExtra", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ProductSize", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ProductTopping", "ProductId", "dbo.Product");
            AddForeignKey("dbo.ProductExtra", "ProductId", "dbo.Product", "Id");
            AddForeignKey("dbo.ProductSize", "ProductId", "dbo.Product", "Id");
            AddForeignKey("dbo.ProductTopping", "ProductId", "dbo.Product", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductTopping", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ProductSize", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ProductExtra", "ProductId", "dbo.Product");
            AddForeignKey("dbo.ProductTopping", "ProductId", "dbo.Product", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ProductSize", "ProductId", "dbo.Product", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ProductExtra", "ProductId", "dbo.Product", "Id", cascadeDelete: true);
        }
    }
}
