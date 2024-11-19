namespace WebsiteBanDoAnVaThucUong.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class address_v1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StreetAddress = c.String(nullable: false, maxLength: 250),
                        ProvinceId = c.Int(nullable: false),
                        ProvinceName = c.String(nullable: false, maxLength: 100),
                        DistrictId = c.Int(nullable: false),
                        DistrictName = c.String(nullable: false, maxLength: 100),
                        WardId = c.Int(nullable: false),
                        WardName = c.String(nullable: false, maxLength: 100),
                        Latitude = c.Decimal(nullable: false, precision: 18, scale: 9),
                        Longitude = c.Decimal(nullable: false, precision: 18, scale: 9),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.ProvinceId, t.DistrictId, t.WardId });
            AddColumn("dbo.Store", "AddressId", c => c.Int(nullable: true));
            Sql(@"
            INSERT INTO dbo.Addresses (StreetAddress, ProvinceId, ProvinceName, DistrictId, 
                                     DistrictName, WardId, WardName, Latitude, Longitude, CreatedAt)
            SELECT Address, 0, '', 0, '', 0, '', Lat, Long, GETDATE()
            FROM dbo.Store
            WHERE Address IS NOT NULL;

            UPDATE s
            SET s.AddressId = a.Id
            FROM dbo.Store s
            INNER JOIN dbo.Addresses a ON s.Address = a.StreetAddress;
        ");
            //Tạo foreign key sau khi đã có dữ liệu
            CreateIndex("dbo.Store", "AddressId");
            AddForeignKey("dbo.Store", "AddressId", "dbo.Addresses", "Id");
            DropColumn("dbo.Store", "Address");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Store", "Address", c => c.String());
            DropForeignKey("dbo.Store", "AddressId", "dbo.Addresses");
            DropIndex("dbo.Store", new[] { "AddressId" });
            DropIndex("dbo.Addresses", new[] { "ProvinceId", "DistrictId", "WardId" });
            DropColumn("dbo.Store", "AddressId");
            DropTable("dbo.Addresses");
        }
    }
}
