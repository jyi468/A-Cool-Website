$(document).ready(function () {

    var body = $('body');

    /**********************/
    /*   Main-Content     */



    model_container = $('.model-container');

    //Load comment into comment zoom
    $('#main-inner').on("click", ".primary", function () {
        theComment = $('.commentContent').val();
        theId = $('.primary').attr('id');
        theUrl = $('.primary').attr('name');
        link = $('#buttonText').attr('id');
        $('.listComments').load("/Account/Comment", { id : theId, url: theUrl, comment: theComment });
        return false;
    });

    // Use for deleting comments
    $(document).on("click", "#deleteButton", function () {
        theCommentNo = $('.PinDeleteCommentButton').attr('data-ph');
        theId = $('.commentDescriptionCreator').attr('data-ph'); //entity id
        theUrl = $('.model-box').find('li').attr('src'); //photo url
        //link = $('#buttonText').attr('id');
        toggleCommentDelete;
        //alert("deletebutton pressed")
        $('.listComments').load("/Account/DeleteComment", { commentNo: theCommentNo, id: theId, url: theUrl  });
        return false;
    });

    /**************/
    /*   Delete (derived from logout)   */
    /**************/

    toggleCommentDelete = function () {
        //$('model-container').toggleClass('body-locked');
        body.toggleClass('body-locked');
        $('#delete1').toggleClass('dp-block');
        //alert('gotClicked');
    };

    //$('.PinDeleteCommentButton').on('click', toggleCommentDelete);

    $(document).on('click', '.PinDeleteCommentButton', toggleCommentDelete);
    $(document).on('click', '#cancelButton', toggleCommentDelete);

    /**************/
    /*   Logout   */
    /**************/
    close_logoutmodelpopup = $('#logoutButton');
    logoutcontent = $('.logout');
    logout_modelcontent = $('#logout1');

    toggleLogoutContentPopUp = function () {
        body.toggleClass('body-locked');
        logout_modelcontent.toggleClass('dp-block');
    };

    logoutcontent.on('click', toggleLogoutContentPopUp);
    close_logoutmodelpopup.on('click', toggleLogoutContentPopUp)


    // Make body locked and dim out background
    // Render partial view (zooming) 
    $(".forPartial").click(function () {
        body.toggleClass('body-locked');
        model_container.toggleClass('dp-block');
        $('#partialRender').load(this.href);
        return false;
    });


    // Close and empty render partial
    $(document).on("click", ".close-model", function () {
        $('#partialRender').empty();
        body.toggleClass('body-locked');
        model_container.toggleClass('dp-block');
    });

    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            if ($('#partialRender').find('div').length) {
                $('#partialRender').empty();
                body.toggleClass('body-locked');
                model_container.toggleClass('dp-block');
            }
        }
    });




    /**************/
    /* Categories */
    /**************/
    categories = $('.categories');
    CategoriesWrapper = $('.CategoriesWrapper');

    toggleCategoryDropDown = function () {

        if (CategoriesWrapper.hasClass('active')) {
            CategoriesWrapper.removeClass('active');
        } else {
            CategoriesWrapper.addClass('active');
        }
    };

    categories.on('click', toggleCategoryDropDown);

    /**************/
    /*   Search   */
    /**************/

    typeahead = $('.typeahead');
    field = $('.field');

    toggleSearchDropDown = function () {

        if (field.val() === "") {
            typeahead.css("display", "none");
        } else {
            typeahead.css("display", "block");
        }
    };

    field.on('keyup', toggleSearchDropDown);

    /**************/
    /* AddContent */
    /**************/
    close_modelpopup = $('.close-modelpopup');
    addcontent = $('.addcontent');
    model_content = $('.model-content');

    toggleNewContentPopUp = function () {
        body.toggleClass('body-locked');
        model_content.toggleClass('dp-block');
    };

    addcontent.on('click', toggleNewContentPopUp);
    close_modelpopup.on('click', toggleNewContentPopUp);




    /***************/
    /*   Profile   */
    /***************/
    Profile = $('.Profile');
    ProfileDropdown = $('.ProfileDropdown');

    toggleProfileDropDown = function () {
        if (ProfileDropdown.css("display") === "block") {
            ProfileDropdown.css("display", "none");
        } else {
            ProfileDropdown.css("display", "block");
        }
    };

    Profile.on('click', toggleProfileDropDown);


    /**********************/
    /*   Notifications   */
    /*********************/
    notifications = $('.notifications');
    notificationsDropdown = $('.notificationsDropdown');

    toggleNotificationsDropDown = function () {
        if (notificationsDropdown.hasClass('active')) {
            notificationsDropdown.removeClass('active');
        } else {
            notificationsDropdown.addClass('active');
        }
    };

    notifications.on('click', toggleNotificationsDropDown);

    /***********************/
    /*   Display Photos Div*/
    /***********************/
    container = $('.js-masonry');


    /*Chrome Fix for Masonry*/
    var $container = $('#container');

    $container.imagesLoaded(function () {
        $container.masonry({
            itemSelector: '.box'
        });
    });


});