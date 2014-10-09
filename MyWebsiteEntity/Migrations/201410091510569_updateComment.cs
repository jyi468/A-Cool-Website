namespace MyWebsiteEntity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateComment : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EntityComment", "EntityId", "dbo.Entity");
            DropForeignKey("dbo.EntityDescription", "EntityId", "dbo.Entity");
            DropIndex("dbo.EntityComment", new[] { "EntityId" });
            DropIndex("dbo.EntityDescription", new[] { "EntityId" });
            AlterColumn("dbo.EntityDescription", "Description", c => c.String(nullable: false, maxLength: 128));
            DropPrimaryKey("dbo.EntityComment", new[] { "EntityId" });
            AddPrimaryKey("dbo.EntityComment", "CommentNo");
            DropPrimaryKey("dbo.EntityDescription", new[] { "EntityId" });
            AddPrimaryKey("dbo.EntityDescription", "Description");
            AddForeignKey("dbo.EntityComment", "EntityId", "dbo.Entity", "EntityId", cascadeDelete: true);
            AddForeignKey("dbo.EntityDescription", "EntityId", "dbo.Entity", "EntityId", cascadeDelete: true);
            CreateIndex("dbo.EntityComment", "EntityId");
            CreateIndex("dbo.EntityDescription", "EntityId");
            DropColumn("dbo.EntityDescription", "RowVersion");
            DropColumn("dbo.EntityTag", "RowVersion");
            DropColumn("dbo.LikedEntity", "RowVersion");
            DropColumn("dbo.Photo", "RowVersion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Photo", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.LikedEntity", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.EntityTag", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.EntityDescription", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            DropIndex("dbo.EntityDescription", new[] { "EntityId" });
            DropIndex("dbo.EntityComment", new[] { "EntityId" });
            DropForeignKey("dbo.EntityDescription", "EntityId", "dbo.Entity");
            DropForeignKey("dbo.EntityComment", "EntityId", "dbo.Entity");
            DropPrimaryKey("dbo.EntityDescription", new[] { "Description" });
            AddPrimaryKey("dbo.EntityDescription", "EntityId");
            DropPrimaryKey("dbo.EntityComment", new[] { "CommentNo" });
            AddPrimaryKey("dbo.EntityComment", "EntityId");
            AlterColumn("dbo.EntityDescription", "Description", c => c.String());
            CreateIndex("dbo.EntityDescription", "EntityId");
            CreateIndex("dbo.EntityComment", "EntityId");
            AddForeignKey("dbo.EntityDescription", "EntityId", "dbo.Entity", "EntityId");
            AddForeignKey("dbo.EntityComment", "EntityId", "dbo.Entity", "EntityId");
        }
    }
}
