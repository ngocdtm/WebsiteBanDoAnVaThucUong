namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fee_v31 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "ShippingFee", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Order", "ShippingFee");
        }
    }
}
