namespace MyWebsiteEntity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rowversion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EntityDescription", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.EntityTag", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.LikedEntity", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Photo", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Photo", "RowVersion");
            DropColumn("dbo.LikedEntity", "RowVersion");
            DropColumn("dbo.EntityTag", "RowVersion");
            DropColumn("dbo.EntityDescription", "RowVersion");
        }
    }
}
