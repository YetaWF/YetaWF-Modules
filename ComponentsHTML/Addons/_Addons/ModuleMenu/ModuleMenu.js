/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* Init all module menus */

$YetaWF.addWhenReady(function (tag) {
    $('.yModuleMenu', $(tag)).kendoMenu({
        orientation: "vertical"
    })
    .css({
        width: 'auto'
    });
});


