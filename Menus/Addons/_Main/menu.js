/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

var YetaWF_Menu = {};

YetaWF_Menu.init = function (menuId) {
    'use strict';

    var $menu = $('#' + menuId);
    if ($menu.length != 1) throw 'Menu control not found';/*DEBUG*/

    // Mark all li entries with a.t_currenturl or t_currenturlpart as active (the url matches the current page)
    $('#{0} a.t_currenturl,#{0} a.t_currenturlpart'.format(menuId)).closest('li').addClass('k-state-active')
};
