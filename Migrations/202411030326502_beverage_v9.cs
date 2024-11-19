namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class beverage_v9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderDetail", "SelectedSizeIds", c => c.String());
            AddColumn("dbo.OrderDetail", "SelectedToppingIds", c => c.String());
            AddColumn("dbo.OrderDetail", "SelectedExtraIds", c => c.String());
            AddColumn("dbo.OrderDetail", "SizePrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.OrderDetail", "ToppingPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.OrderDetail", "ExtraPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderDetail", "ExtraPrice");
            DropColumn("dbo.OrderDetail", "ToppingPrice");
            DropColumn("dbo.OrderDetail", "SizePrice");
            DropColumn("dbo.OrderDetail", "SelectedExtraIds");
            DropColumn("dbo.OrderDetail", "SelectedToppingIds");
            DropColumn("dbo.OrderDetail", "SelectedSizeIds");
        }
    }
}
