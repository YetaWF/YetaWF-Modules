"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
/* This is only used by components in ComponentsHTML (this package) */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var PopupsSupportClass = /** @class */ (function () {
        function PopupsSupportClass() {
        }
        /**
         * inline - as soon as we're loading, resize the popup window, if we're in a popup
         * this is only used by full page loads (i.e., a popup using an iframe)
         */
        PopupsSupportClass.prototype.pageLoad = function () {
            if (YVolatile.Basics.IsInPopup) {
                var $popupwin = $("#ypopup", $(window.parent.document));
                var popup = window.parent.document.YPopupWindowActive;
                // get the popup window height
                var width = YVolatile.Skin.PopupWidth;
                var height = YVolatile.Skin.PopupHeight;
                popup.setOptions({
                    width: width,
                    height: height,
                });
                YetaWF_Basics.setCondense($popupwin, width);
                popup.center().open();
                // show/hide the maximize button (not directly supported so we'll do it manually)
                if ($popupwin.length == 0)
                    throw "Couldn't find popup window"; /*DEBUG*/
                var $popWindow = $popupwin.closest('.k-widget.k-window');
                if ($popWindow.length == 0)
                    throw "Couldn't find enclosing popup window"; /*DEBUG*/
                if (YVolatile.Skin.PopupMaximize)
                    $('.k-window-action.k-button,.k-window-action.k-link', $popWindow).eq(0).show(); // show the maximize button
                else
                    $('.k-window-action.k-button,.k-window-action.k-link', $popWindow).eq(0).hide(); // hide the maximize button
            }
        };
        return PopupsSupportClass;
    }());
    YetaWF_ComponentsHTML.PopupsSupportClass = PopupsSupportClass;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
var PopupsSupport = new YetaWF_ComponentsHTML.PopupsSupportClass();
// Page Load
/**
 * Currently active popup window
 */
document.YPopupWindowActive = null;
/**
 * Initial full page load (popup in iframe)
 */
PopupsSupport.pageLoad();
