namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fee_v3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Store", "ShippingFeeId", "dbo.ShippingFee");
            DropIndex("dbo.Store", new[] { "ShippingFeeId" });
            DropColumn("dbo.Store", "ShippingFeeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Store", "ShippingFeeId", c => c.Int());
            CreateIndex("dbo.Store", "ShippingFeeId");
            AddForeignKey("dbo.Store", "ShippingFeeId", "dbo.ShippingFee", "Id");
        }
    }
}
