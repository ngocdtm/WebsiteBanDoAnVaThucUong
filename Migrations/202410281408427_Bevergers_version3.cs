namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Bevergers_version3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductCategory", "ProductTypeId", c => c.Int(nullable: false, defaultValue: 1)); // Sử dụng defaultValue: 1

            CreateIndex("dbo.ProductCategory", "ProductTypeId");
            AddForeignKey("dbo.ProductCategory", "ProductTypeId", "dbo.ProductType", "Id", cascadeDelete: true);
            //  Lệnh sau giúp đảm bảo bảng ProductType có ít nhất một bản ghi trước khi cập nhật ProductCategory
            Sql("IF NOT EXISTS (SELECT * FROM dbo.ProductType) BEGIN INSERT INTO dbo.ProductType (Name, Alias) VALUES ('Default', 'default') END");
            // Lệnh sau giúp đặt ProductTypeId mặc định cho các bản ghi trong ProductCategory
            Sql("UPDATE dbo.ProductCategory SET ProductTypeId = (SELECT TOP 1 Id FROM dbo.ProductType)");

        }

        public override void Down()
        {
            DropForeignKey("dbo.ProductCategory", "ProductTypeId", "dbo.ProductType");
            DropIndex("dbo.ProductCategory", new[] { "ProductTypeId" });
            DropColumn("dbo.ProductCategory", "ProductTypeId");
        }
    }
}
