namespace MyWebsiteEntity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        EmailAddress = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Profile = c.String(),
                        Country = c.String(),
                        Gender = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Entity",
                c => new
                    {
                        EntityId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.EntityId);
            
            CreateTable(
                "dbo.EntityComment",
                c => new
                    {
                        EntityId = c.Int(nullable: false),
                        CommentNo = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Comment = c.String(),
                    })
                .PrimaryKey(t => t.EntityId)
                .ForeignKey("dbo.Entity", t => t.EntityId)
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.EntityId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.EntityDescription",
                c => new
                    {
                        EntityId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.EntityId)
                .ForeignKey("dbo.Entity", t => t.EntityId)
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.EntityId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.EntityTag",
                c => new
                    {
                        EntityId = c.Int(nullable: false),
                        TagName = c.String(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EntityId)
                .ForeignKey("dbo.Entity", t => t.EntityId)
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.EntityId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.LikedEntity",
                c => new
                    {
                        EntityId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Liked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.EntityId)
                .ForeignKey("dbo.Entity", t => t.EntityId)
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.EntityId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Photo",
                c => new
                    {
                        EntityId = c.Int(nullable: false),
                        PhotoURL = c.String(),
                    })
                .PrimaryKey(t => t.EntityId)
                .ForeignKey("dbo.Entity", t => t.EntityId)
                .Index(t => t.EntityId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Photo", new[] { "EntityId" });
            DropIndex("dbo.LikedEntity", new[] { "UserId" });
            DropIndex("dbo.LikedEntity", new[] { "EntityId" });
            DropIndex("dbo.EntityTag", new[] { "UserId" });
            DropIndex("dbo.EntityTag", new[] { "EntityId" });
            DropIndex("dbo.EntityDescription", new[] { "UserId" });
            DropIndex("dbo.EntityDescription", new[] { "EntityId" });
            DropIndex("dbo.EntityComment", new[] { "UserId" });
            DropIndex("dbo.EntityComment", new[] { "EntityId" });
            DropForeignKey("dbo.Photo", "EntityId", "dbo.Entity");
            DropForeignKey("dbo.LikedEntity", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.LikedEntity", "EntityId", "dbo.Entity");
            DropForeignKey("dbo.EntityTag", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.EntityTag", "EntityId", "dbo.Entity");
            DropForeignKey("dbo.EntityDescription", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.EntityDescription", "EntityId", "dbo.Entity");
            DropForeignKey("dbo.EntityComment", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.EntityComment", "EntityId", "dbo.Entity");
            DropTable("dbo.Photo");
            DropTable("dbo.LikedEntity");
            DropTable("dbo.EntityTag");
            DropTable("dbo.EntityDescription");
            DropTable("dbo.EntityComment");
            DropTable("dbo.Entity");
            DropTable("dbo.UserProfile");
        }
    }
}
