/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* YetaWF component package requirement */
function Y_Loading(starting) {
    if (starting != false) {
        $.prettyLoader.show();
    } else {
        $.prettyLoader.hide();
        Y_PleaseWaitClose();
    }
}

// PRETTYLOADER

$.prettyLoader({
    animation_speed: 'fast', /* fast/normal/slow/integer */
    bind_to_ajax: true, /* true/false */
    delay: false, /* false OR time in milliseconds (ms) */
    loader: YConfigs.YetaWF_ComponentsHTML.LoaderGif, /* Path to your loader gif */
    offset_top: 13, /* integer */
    offset_left: 10 /* integer */
});
