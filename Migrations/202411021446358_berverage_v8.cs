namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class berverage_v8 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BeverageDetails", "SizeId", "dbo.Size");
            DropForeignKey("dbo.BeverageDetails", "ToppingId", "dbo.Topping");
            DropIndex("dbo.BeverageDetails", new[] { "SizeId" });
            DropIndex("dbo.BeverageDetails", new[] { "ToppingId" });
            AlterColumn("dbo.BeverageDetails", "SizeId", c => c.Int());
            AlterColumn("dbo.BeverageDetails", "ToppingId", c => c.Int());
            CreateIndex("dbo.BeverageDetails", "SizeId");
            CreateIndex("dbo.BeverageDetails", "ToppingId");
            AddForeignKey("dbo.BeverageDetails", "SizeId", "dbo.Size", "Id");
            AddForeignKey("dbo.BeverageDetails", "ToppingId", "dbo.Topping", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BeverageDetails", "ToppingId", "dbo.Topping");
            DropForeignKey("dbo.BeverageDetails", "SizeId", "dbo.Size");
            DropIndex("dbo.BeverageDetails", new[] { "ToppingId" });
            DropIndex("dbo.BeverageDetails", new[] { "SizeId" });
            AlterColumn("dbo.BeverageDetails", "ToppingId", c => c.Int(nullable: false));
            AlterColumn("dbo.BeverageDetails", "SizeId", c => c.Int(nullable: false));
            CreateIndex("dbo.BeverageDetails", "ToppingId");
            CreateIndex("dbo.BeverageDetails", "SizeId");
            AddForeignKey("dbo.BeverageDetails", "ToppingId", "dbo.Topping", "Id", cascadeDelete: true);
            AddForeignKey("dbo.BeverageDetails", "SizeId", "dbo.Size", "Id", cascadeDelete: true);
        }
    }
}
