namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MemberRank : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MemberRanks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        Points = c.Int(nullable: false),
                        Rank = c.String(),
                        TotalSpent = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MemberRanks", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.MemberRanks", new[] { "UserId" });
            DropTable("dbo.MemberRanks");
        }
    }
}
