using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace MyWebsiteEntity.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }


        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Entity> Entities { get; set; }
        public DbSet<EntityComment> EntityComment { get; set; }
        public DbSet<EntityDescription> EntityDescription { get; set; }
        public DbSet<EntityTag> EntityTag { get; set; }
        public DbSet<LikedEntity> LikedEntities { get; set; }      
        public DbSet<Photo> Photo { get; set; }
        public IQueryable<int> EntityIds { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class ContentViewModel
    {

    }
       

    public class MainViewModel
    {
        public UserProfile CurrentUser { get; set; }
        public IList<UserProfile> UserProfiles { get; set; }
        public IList<Entity> Entities { get; set; }
        public IList<string> TagNames { get; set; }
        public IList<string> UserNames { get; set; }
        public IList<Photo> Photo { get; set; }
        public IList<int> EntityIds { get; set; }

        public MainViewModel(UserProfile _currentUser, IList<UserProfile> _userProfiles, IList<Entity> _entities, IList<string> _tagNames,
            IList<string> _userNames, IList<Photo> _photo, IList<int> _entityids)
        {
            CurrentUser = _currentUser;
            UserProfiles = _userProfiles;
            Entities = _entities;
            TagNames = _tagNames;
            UserNames = _userNames;
            Photo = _photo;
            EntityIds = _entityids;
        }
    }


    public class SingleCommentModel
    {
        public string UserName { get; set; }
        public string Comment { get; set; }
    }

    public class CommentModel
    {
        public IList<string> UserNames { get; set; }
        public IList<string> Comments { get; set; }

        public int EntityId { get; set; }
        public int opUserId { get; set; }
        public string Description { get; set; }

        public string imgurl { get; set; }
        public string OP { get; set; }
    }

    public class PostModel
    {
        public string Category { get; set; } // Tag
        public string Description { get; set; } //Description
        public HttpPostedFileBase File { get; set; } //PhotoURL (derived from file)
    }

    public class SettingsModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        public HttpPostedFileBase Profile { get; set; }
    }

    [Table("Entity")]
    public class Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int EntityId { get; set; } // PK
    }

    [Table("EntityComment")]
    public class EntityComment
    {
        [ForeignKey("Entity")]
        public int EntityId { get; set; }
        public virtual Entity Entity { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual UserProfile User { get; set; }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CommentNo { get; set; }

        public string Comment { get; set; }
    }

    [Table ("EntityDescription")]
    public class EntityDescription
    {

        [ForeignKey("EntityId")]
        public virtual Entity Entity { get; set; }
        public int EntityId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile User { get; set; }
        public int UserId { get; set; }

        [Key]
        public string Description { get; set; }

    }

    [Table("LikedEntity")]
    public class LikedEntity
    {
        [Key, ForeignKey("Entity")]
        public int EntityId { get; set; } // PK,FK1
        public virtual Entity Entity { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile User { get; set; }
        public int UserId { get; set; } // FK2

        public bool Liked { get; set; }

    }
    

    [Table("EntityTag")]
    public class EntityTag //AKA CATEGORY
    {
        [Key, ForeignKey("Entity")]
        public int EntityId { get; set; } // PK,FK1
        public virtual Entity Entity { get; set; }

        public string TagName { get; set; } //FK2 - unused as FK

        [ForeignKey("UserId")]
        public virtual UserProfile User { get; set; }
        public int UserId { get; set; } // FK3

    }

    /*[Table("Tag")]
    public class Tag
    {
        public string TagName { get; set; } //PK
        public string TagDescription { get; set; }
    }*/

    [Table("Photo")]
    public class Photo
    {
        [Key, ForeignKey("Entity")]
        public int EntityId { get; set; } //PK,FK
        public virtual Entity Entity { get; set; }

        public string PhotoURL { get; set; }

    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Profile { get; set; }
        public string Country { get; set; }
        public string Gender { get; set; }
    }

    

    /*public class MainViewModel
    {
        public DbSet<UserProfile> userProfiles { get; set; }
        public List<Photo> photos { get; set; }
        public List<PostModel> postmodels { get; set; }

        public MainViewModel(DbSet<UserProfile> _userProfiles, List<Photo> _photos, List<PostModel> _postmodels)
        {
            userProfiles = _userProfiles;
            photos = _photos;
            postmodels = _postmodels;
        }
    }*/

    /*public class Comment
    {
        public int Id { get; set; }
        public int PictureId { get; set; }
        public string Content { get; set; }
    }*/



    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
