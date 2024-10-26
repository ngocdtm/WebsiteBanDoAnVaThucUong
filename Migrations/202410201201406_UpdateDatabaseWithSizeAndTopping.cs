namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabaseWithSizeAndTopping : DbMigration
    {
        public override void Up()
        {
            
            CreateTable(
                "dbo.Size",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NameSize = c.String(nullable: false, maxLength: 50),
                        PriceSize = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Product", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
         
              
            
            CreateTable(
                "dbo.Topping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NameTopping = c.String(nullable: false, maxLength: 100),
                        PriceTopping = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Product", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
          
            
           
        }
        
        public override void Down()
        {
          
        }
    }
}
