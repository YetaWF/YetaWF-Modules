/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* Init all module menus */

YetaWF_Basics.addWhenReady(function (tag) {
    $('.yModuleMenu', $(tag)).kendoMenu({
        orientation: "vertical"
    })
    .css({
        width: 'auto'
    });
});


