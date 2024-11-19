namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShippingFee : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShippingFee",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FeePerKm = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MinimumFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ShippingFee");
        }
    }
}
