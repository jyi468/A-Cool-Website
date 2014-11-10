A-Cool-Website
==============

This website has been created to model basic functionality similar to the website Pinterest.com. The website was created with the goal of showing what can be done using ASP.NET MVC and the Entity Framework.

====================================
Running Application in Visual Studio
====================================
You can get the application in Visual Studio by connecting to this repository and run using your browser of choice (currently optimized for Firefox). There is already a database populated with user profiles for demonstration purposes but more can be created.


===================
Currently Available:
===================
You can add content (photos) which can be commented on and seen by others. The photos can be assigned categories, which are stored in the database and displayed with the content. You can also put a description to the photo.

To comment on a photo, you must click the content which will then load a partial view using Ajax titled 'CommentZoom.cshtml'. When you post a comment, another Ajax call is made which will then load the new comment onto the partial view. Also, comments can be deleted by the original poster of the comment.

The content is displayed randomly by randomly querying the database and displaying a number of pictures under a certain limit. For example, if you set the limit to 20, it will only display 20 random entities from the database to a user.

This website also uses Masonry, a plugin used to organize content such as photos and videos on a webpage.

This website uses LocalDB through Entity Framework 5. There are databases for user profiles, photos, comments, entities, descriptions and categories. A picture of the database model is also included. The 3 core tables used to navigate and organize all content are User Profile, Entities and LikedEntities.

It is a little bit rough around the edges and unpolished but the core functionality is available.

===================
Things to be added:
==================
- Creating "boards"

- Profile pictures

- Expanding image storing capabilities (database, offsite, URL's)

- Search functionality.

- Release production version on to Microsoft Azure Cloud
