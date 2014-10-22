using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using MyWebsiteEntity.Filters;
using MyWebsiteEntity.Models;
using System.Data;
using System.IO;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;

namespace MyWebsiteEntity.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [RequireHttps]
    public class AccountController : Controller
    {

        Repository _repository = new Repository();
        /*
        public ActionResult MainViewModel()
        {
            UsersContext db = new UsersContext();
            var userProfile = db.UserProfiles.FirstOrDefault(u => u.UserName.Equals(User.Identity.Name));
            var entities = db.Entities.ToList();
            var ecomment = db.EntityComment.ToList();
            var edescription = db.EntityDescription.ToList();
            var etag = db.EntityTag.ToList();
            var photos = db.Photo.ToList();

            IQueryable<int> eids = db.EntityIds;
            MainViewModel main = new MainViewModel(userProfile, entities,
                ecomment, edescription, etag, photos, eids);

            return View(main);
        }*/

        //POST: comments
        [HttpPost]
        public ActionResult Comment(string comment, string url, int id)
        {
            using (UsersContext db = new UsersContext())
            {
                //current user
                UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.Equals(User.Identity.Name));
                EntityComment entityComment = new EntityComment();
                entityComment.UserId = user.UserId;
                entityComment.EntityId = id;
                entityComment.Comment = comment;
                
                db.EntityComment.Add(entityComment);
                db.SaveChanges();
            }
            return RedirectToAction("CommentZoom", "Account", new { id = id, url = url });
        }

        //GET:
        //Use the entity id of the clicked photo and apply the "zoom"
        //by returning a model containing description, current comments and photo
        public ActionResult CommentZoom(int id, string url) //string passed is entityid
        {
            var model = new CommentModel();
            using (UsersContext db = new UsersContext())
            {
                //current user
                UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.Equals(User.Identity.Name));

                // Get op user, user ID, entity ID, img relative url
                var op_query = (from u in db.UserProfiles
                                 join l in db.LikedEntities
                                 on u.UserId equals l.UserId
                                 join e in db.Entities
                                 on l.EntityId equals e.EntityId
                                 select u).ToList();

                var op = op_query.First();

                var currentUserId = user.UserId;
                var opUserId = op.UserId;
                var opEntityId = id;
                var imgurl = url;
                

                // Get description (description string)
                var opDescriptionList = (from desc in db.EntityDescription
                                    where desc.EntityId == opEntityId
                                    select desc.Description).ToList();


                // Get comments and associated usernames (find userIds)
                //Should be IQuerable<string>
                var comments = (from c in db.EntityComment
                               where c.EntityId == opEntityId
                               select c.Comment).ToList();

                var users = (from u in db.UserProfiles
                              join c in db.EntityComment
                              on u.UserId equals c.UserId
                              select u.UserName).ToList();

                // Create model containing description, current comments, UserId (OP)
                // and commenters UserId's

                model.UserNames = users;
                model.Comments = comments;
                model.opUserId = opUserId;
                model.EntityId = opEntityId;
                model.Description = opDescriptionList.First().ToString();
                model.imgurl = imgurl;
                model.OP = op.UserName;
            }
            return PartialView("_CommentZoom", model);
        }

        // GET: /Account/Main
        public ActionResult Display(UserProfile model)
            //Get current User -> Display all images in list
        {

            return View();
        }
        
        // POST: /Account/Main
        [HttpPost]
        public ActionResult Post(PostModel model) //upload content
        {
            /*********** START - CREATE DB OBJECTS     *************/
            UsersContext db = new UsersContext();
            UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.Equals(User.Identity.Name));

            // Entity
            Entity entity = new Entity(); // primary key auto generated
            //entity.EntityId = 1;

            /*********** START - SETUP KEYS/DB OBJECTS *************/
            int eid = entity.EntityId;
            int uid = user.UserId;

            //Set up LikedEntity
            LikedEntity Lentity = new LikedEntity();
            Lentity.EntityId = eid;
            Lentity.UserId = uid;

            //Set up Photo
            Photo photo = new Photo();
            photo.EntityId = eid;

            //Set up EntityDescription
            EntityDescription edescription = new EntityDescription();
            edescription.EntityId = eid;
            edescription.UserId = uid;

            //Set up EntityTag (Category)
            EntityTag etag = new EntityTag();
            etag.EntityId = eid;
            etag.UserId = uid;

            /*********** END - SETUP KEYS/DB OBJECTS ***************/

            /*********** START- ADD CONTENT ***************/
            // We need to add a PhotoURL, Category and Description
            if(ModelState.IsValid)
            {
                if (model.File != null)
                {
                    string pic = Path.GetFileName(model.File.FileName);
                    string localpath = Path.Combine(Server.MapPath("~/Images/Pictures"), pic);
                    string relativepath = "~/Images/Pictures/" + pic;

                    //PhotoURL
                    photo.PhotoURL = relativepath; //save relative path in photo object

                    //Category
                    etag.TagName = model.Category;

                    //Description
                    edescription.Description = model.Description;

                    //Save objects to tables
                    db.LikedEntities.Add(Lentity);
                    db.Photo.Add(photo);
                    db.EntityDescription.Add(edescription);
                    db.EntityTag.Add(etag);
                    db.Entities.Add(entity);

                    bool saveFailed;
                    do
                    {
                        saveFailed = false;
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            saveFailed = true;

                            // Update original values from the database 
                            foreach (var entry in ex.Entries)
                            {
                                entry.Reload();
                            }
                        }

                    } while (saveFailed);

                    // Save image in folder
                    model.File.SaveAs(localpath);

                }
                return RedirectToAction("Main", "Account");
                /*********** END - ADD CONTENT ****************/
            }
            else
            {
                ModelState.AddModelError("URL", "Upload Failed.");
            }
            return View(model);
        }


        // GET: /Account/Settings
        public ActionResult Settings()
        {
            return View();
        }

        // POST: /Account/Settings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Settings(SettingsModel model, HttpPostedFileBase file)
        {
            using (UsersContext db = new UsersContext())
            {
                UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.Equals(User.Identity.Name));

                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/Profile"), pic);
                    user.Profile = path;
                    // Save image in folder
                    file.SaveAs(path);

                }
                /*db.Entry(user).State = EntityState.Modified;
                var dbC = ((IObjectContextAdapter)db).ObjectContext;

                var refreshableObjects = db.ChangeTracker.Entries().Select(c => c.Entity).ToList();
                dbC.Refresh(RefreshMode.StoreWins, refreshableObjects);
                db.SaveChanges();*/
                RedirectToAction("Settings", "Account");
            }
            return View(model);
        }

        // GET: /Account/Main
        public ActionResult Main() // Return model with entities, photos
                                    // comments, etc.
        {
            UsersContext db = new UsersContext();
            //var userProfiles = new List<UserProfile>();
            UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.Equals(User.Identity.Name));

            //list of ALL liked entities RANDOMIZED
            var likedEnts = 
                from lents in db.LikedEntities
                select lents;

            int entityLimit = 20; // Entity limit is the max number of entities per page


            //Limit entities
            if (likedEnts.Count() > entityLimit) 
            {
                likedEnts = likedEnts.Distinct().Take(entityLimit); // Take x distinct, random liked Entities                   
            }
            


            List<int> entids = new List<int>(); //list of Entity ID's
            List<Entity> entities = new List<Entity>();
            List<EntityTag> tags = new List<EntityTag>();
            List<string> tagNames = new List<string>();
            List<Photo> photos = new List<Photo>();
            List<string> userNames = new List<string>();
            List<UserProfile> userProfiles = new List<UserProfile>();


            //get entities
            var entities_query = from ents in db.Entities
                                 join lents in likedEnts
                                 on ents.EntityId equals lents.EntityId
                                 join u in db.UserProfiles
                                 on lents.UserId equals u.UserId
                                 orderby Guid.NewGuid()
                                 select ents;

            //var userprofiles_query = from users in db.UserProfiles

            List<int> entIDS = new List<int>();
            foreach(var ent in entities_query)
            {
                entities.Add(ent);
                entIDS.Add(ent.EntityId);
            }

            List<LikedEntity> RandomLents = new List<LikedEntity>(); // used for matching up correct 
                                                                     // entities and users
            foreach(var id in entIDS)
            {
                tags.Add(db.EntityTag.FirstOrDefault(u => u.EntityId.Equals(id)));
                photos.Add(db.Photo.FirstOrDefault(u => u.EntityId.Equals(id)));
                RandomLents.Add(db.LikedEntities.FirstOrDefault(u => u.EntityId.Equals(id)));
            }

            foreach(var lent in RandomLents)
            {
                userNames.Add(lent.User.UserName);
                userProfiles.Add(lent.User);
            }

            tagNames = getTagNames(tags);

            MainViewModel main = new MainViewModel(userProfiles, entities,
                tagNames, userNames, photos, entIDS);

            return View(main);
        }

        // Returns list of tag names
        public List<string> getTagNames(List<EntityTag> tags)
        {
            List<string> tagNames = new List<string>();
            foreach(EntityTag tag in tags)
            {
                if(tag.TagName == "1")
                {
                    tagNames.Add("Animals");
                }
                else if(tag.TagName == "2")
                {
                    tagNames.Add("Cars & Motorcycles");
                }
                else if (tag.TagName == "3")
                {
                    tagNames.Add("Fashion");
                }
                else if (tag.TagName == "4")
                {
                    tagNames.Add("Film, Music & Books");
                }
                else if (tag.TagName == "5")
                {
                    tagNames.Add("Food & Drink");
                }
                else if (tag.TagName == "6")
                {
                    tagNames.Add("Hair & Beauty");
                }
                else if (tag.TagName == "7")
                {
                    tagNames.Add("Humor");
                }
                else if (tag.TagName == "8")
                {
                    tagNames.Add("Photography");
                }
                else if (tag.TagName == "9")
                {
                    tagNames.Add("Sports");
                }
                else if (tag.TagName == "10")
                {
                    tagNames.Add("Technology");
                }
                else if (tag.TagName == "11")
                {
                    tagNames.Add("Travel");
                }
                else //other
                {
                    tagNames.Add("Other");
                }
            }
            return tagNames;
        }



        //
        // GET: /Account/Start
        [AllowAnonymous]
        public ActionResult Start()
        {
            return View();
        }
        


        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }



        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)//, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                //@ViewBag.IsModelValid = true;
                //return RedirectToLocal(returnUrl);
                return RedirectToAction("Main", "Account");
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The username or password provided is incorrect.");
            //ViewBag.ErrorMessage = "The username or password provided is incorrect."
            return View(model);
        }

        

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
                return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
              
                if (ModelState.IsValid)
                {

                    // Attempt to register the user
                    try
                    {
                        using (UsersContext db = new UsersContext())
                        {                                                       

                            WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                            WebSecurity.Login(model.UserName, model.Password);

                            //string username = User.Identity.Name;
                            // Get user profile of created user
                            UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.Equals(model.UserName));

                            user.EmailAddress = model.EmailAddress;

                            db.Entry(user).State = EntityState.Modified;

                            db.SaveChanges();

                            return RedirectToAction("Main", "Account");
                        }
                    }
                    catch (MembershipCreateUserException e)
                    {
                        ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                    }
                }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");

            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
