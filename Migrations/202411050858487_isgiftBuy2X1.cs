namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isgiftBuy2X1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderDetail", "IsGift", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderDetail", "IsGift");
        }
    }
}
