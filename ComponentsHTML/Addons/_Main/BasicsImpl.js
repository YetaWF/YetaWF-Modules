"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
/* Basics implementation required by YetaWF */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var BasicsImpl = /** @class */ (function () {
        function BasicsImpl() {
        }
        // LOADING
        // LOADING
        // LOADING
        BasicsImpl.prototype.setLoading = function (on) {
            if (on !== false) {
                $.prettyLoader.show();
            }
            else {
                $.prettyLoader.hide();
            }
        };
        // MESSAGES
        // MESSAGES
        // MESSAGES
        /**
         * Displays an informational message, usually in a popup.
         */
        BasicsImpl.prototype.message = function (message, title, onOK, options) {
            this.alert(message, title || YLocs.Basics.DefaultSuccessTitle, onOK, options);
        };
        /**
         * Displays an error message, usually in a popup.
         */
        BasicsImpl.prototype.error = function (message, title, onOK, options) {
            this.alert(message, title || YLocs.Basics.DefaultErrorTitle, onOK);
        };
        /**
         * Displays a confirmation message, usually in a popup.
         */
        BasicsImpl.prototype.confirm = function (message, title, onOK, options) {
            this.alert(message, title || YLocs.Basics.DefaultSuccessTitle, onOK);
        };
        /**
         * Displays an alert message, usually in a popup.
         */
        BasicsImpl.prototype.alert = function (message, title, onOK, options) {
            var _this = this;
            // check if we already have a popup (and close it)
            this.closeAlert(onOK);
            $("body").prepend("<div id='yalert'></div>");
            var $dialog = $("#yalert");
            options = options || { encoded: false };
            if (!options.encoded) {
                // change \n to <br/>
                $dialog.text(message);
                var s = $dialog.html();
                s = s.replace(/\(\+nl\)/g, "<br/>");
                $dialog.html(s);
            }
            else {
                $dialog.html(message);
            }
            if (title === undefined)
                title = YLocs.Basics.DefaultAlertTitle;
            $dialog.dialog({
                autoOpen: true,
                modal: true,
                width: YConfigs.Basics.DefaultAlertWaitWidth,
                height: YConfigs.Basics.DefaultAlertWaitHeight === 0 ? "auto" : YConfigs.Basics.DefaultAlertWaitHeight,
                closeOnEscape: true,
                closeText: YLocs.Basics.CloseButtonText,
                close: function (event, ui) { return _this.closeAlert(onOK); },
                draggable: true,
                resizable: false,
                title: title,
                buttons: [{
                        text: YLocs.Basics.OKButtonText,
                        click: function (eventObject) {
                            $dialog.dialog("close");
                        }
                    }]
            });
        };
        BasicsImpl.prototype.closeAlert = function (onOK) {
            var $dialog = $("#yalert");
            if ($dialog.length === 0)
                return;
            if ($dialog.attr("data-closing"))
                return;
            $dialog.attr("data-closing", 1);
            var endFunc = onOK;
            onOK = undefined; // clear this so close function doesn't call onOK handler also
            $dialog.dialog("close");
            $dialog.dialog("destroy");
            $dialog.remove();
            if (endFunc)
                endFunc();
        };
        /**
         * Displays an alert message with Yes/No buttons, usually in a popup.
         */
        BasicsImpl.prototype.alertYesNo = function (message, title, onYes, onNo, options) {
            var $body = $("body");
            $body.prepend("<div id='yalert'></div>");
            var $dialog = $("#yalert", $body);
            // change \n to <br/>
            $dialog.text(message);
            var s = $dialog.html();
            s = s.replace(/\(\+nl\)/g, "<br/>");
            $dialog.html(s);
            if (title === undefined)
                title = YLocs.Basics.DefaultAlertYesNoTitle;
            $dialog.dialog({
                autoOpen: true,
                modal: true,
                width: YConfigs.Basics.DefaultAlertYesNoWidth,
                height: YConfigs.Basics.DefaultAlertYesNoHeight === 0 ? "auto" : YConfigs.Basics.DefaultAlertYesNoHeight,
                closeOnEscape: true,
                closeText: YLocs.Basics.CloseButtonText,
                close: function (event, ui) {
                    $dialog.dialog("destroy");
                    $dialog.remove();
                    if (onNo !== undefined)
                        onNo();
                },
                draggable: true,
                resizable: false,
                title: title,
                buttons: [
                    {
                        text: YLocs.Basics.YesButtonText,
                        click: function (eventObject) {
                            var endFunc = onYes;
                            onYes = undefined; // clear this so close function doesn't try do call these
                            onNo = undefined;
                            $dialog.dialog("destroy");
                            $dialog.remove();
                            if (endFunc)
                                endFunc();
                        }
                    },
                    {
                        text: YLocs.Basics.NoButtonText,
                        click: function (eventObject) {
                            var endFunc = onNo;
                            onYes = undefined; // clear this so close function doesn't try do call these
                            onNo = undefined;
                            $dialog.dialog("destroy");
                            $dialog.remove();
                            if (endFunc)
                                endFunc();
                        }
                    }
                ],
            });
        };
        /**
         * Displays a "Please Wait" message
         */
        BasicsImpl.prototype.pleaseWait = function (message, title) {
            // insert <div id="yplwait"></div> at top of page for the window
            // this is automatically removed when destroy() is called
            $("body").prepend("<div id='yplwait'></div>");
            var $popupwin = $("#yplwait");
            var popup = null;
            if (message === undefined)
                message = YLocs.Basics.PleaseWaitText;
            if (title === undefined)
                title = YLocs.Basics.PleaseWaitTitle;
            $popupwin.text(message);
            // Create the window
            $popupwin.kendoWindow({
                actions: [],
                width: YConfigs.Basics.DefaultPleaseWaitWidth,
                height: YConfigs.Basics.DefaultPleaseWaitHeight,
                draggable: true,
                iframe: true,
                modal: true,
                resizable: false,
                title: $YetaWF.htmlEscape(title),
                visible: false,
                close: function (event) {
                    var popup = $popupwin.data("kendoWindow");
                    popup.destroy();
                    popup = null;
                },
            });
            // show and center the window
            popup = $popupwin.data("kendoWindow");
            popup.open().center();
        };
        /**
         * Closes the "Please Wait" message (if any).
         */
        BasicsImpl.prototype.pleaseWaitClose = function () {
            var $popupwin = $("#yplwait");
            if ($popupwin.length === 0)
                return;
            var popup = $popupwin.data("kendoWindow");
            popup.destroy();
        };
        /**
         * Closes any open overlays, menus, dropdownlists, tooltips, etc. (Popup windows are not handled and are explicitly closed using $YetaWF.Popups)
         */
        BasicsImpl.prototype.closeOverlays = function () {
            // Close open bootstrap nav menus (if any) by clicking on the page
            $("body").trigger("click");
            // Close any open kendo menus (if any)
            var $menus = $(".k-menu");
            $menus.each(function (index, element) {
                var menu = $(element).data("kendoMenu");
                menu.close("li.k-item");
            });
            // Close any open smartmenus
            try {
                $(".YetaWF_Menus").collapse("hide");
            }
            catch (e) { }
            // tooltips
            YetaWF_ComponentsHTML.Tooltips.removeTooltips();
        };
        return BasicsImpl;
    }());
    YetaWF_ComponentsHTML.BasicsImpl = BasicsImpl;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
// tslint:disable-next-line:variable-name
var YetaWF_BasicsImpl = new YetaWF_ComponentsHTML.BasicsImpl();

//# sourceMappingURL=BasicsImpl.js.map
