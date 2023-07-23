"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var Severity;
    (function (Severity) {
        Severity[Severity["Info"] = 0] = "Info";
        Severity[Severity["Warning"] = 1] = "Warning";
        Severity[Severity["Error"] = 2] = "Error";
    })(Severity || (Severity = {}));
    var BasicsImpl = /** @class */ (function () {
        function BasicsImpl() {
            this.ToastDiv = null;
            // LOADING
            // LOADING
            // LOADING
            this.loading = false;
            // TOAST
            this.Toasts = [];
        }
        // PAGE INITIALIZATION
        // PAGE INITIALIZATION
        // PAGE INITIALIZATION
        /** Called when a new full page has been loaded and needs to be initialized */
        BasicsImpl.prototype.initFullPage = function () { };
        Object.defineProperty(BasicsImpl.prototype, "isLoading", {
            get: function () {
                return this.loading;
            },
            enumerable: false,
            configurable: true
        });
        BasicsImpl.prototype.setLoading = function (on) {
            if (on !== false) {
                this.loading = true;
                LoadingSupport.show();
            }
            else {
                this.loading = false;
                LoadingSupport.hide();
            }
        };
        // MESSAGES
        // MESSAGES
        // MESSAGES
        /**
         * Displays an informational message, usually in a popup.
         */
        BasicsImpl.prototype.message = function (message, title, onOK, options) {
            if (YConfigs.Basics.MessageType === YetaWF.MessageTypeEnum.Popups || YVolatile.Basics.ForcePopup) {
                this.alert(message, title || YLocs.Basics.DefaultSuccessTitle, onOK, options);
            }
            else {
                if (!options)
                    options = { encoded: false };
                if (options.canClose === undefined)
                    options.canClose = true;
                if (options.autoClose === undefined)
                    options.autoClose = BasicsImpl.DefaultTimeout;
                this.addToast(Severity.Info, title !== null && title !== void 0 ? title : YLocs.Basics.DefaultSuccessTitle, message, options);
                if (onOK)
                    onOK();
            }
        };
        /**
         * Displays an warning message, usually in a popup.
         */
        BasicsImpl.prototype.warning = function (message, title, onOK, options) {
            if (YConfigs.Basics.MessageType === YetaWF.MessageTypeEnum.Popups || YVolatile.Basics.ForcePopup) {
                this.alert(message, title || YLocs.Basics.DefaultAlertTitle, onOK);
            }
            else {
                if (!options)
                    options = { encoded: false };
                if (options.canClose === undefined)
                    options.canClose = true;
                if (options.autoClose === undefined)
                    options.autoClose = BasicsImpl.DefaultTimeout;
                this.addToast(Severity.Warning, title !== null && title !== void 0 ? title : YLocs.Basics.DefaultAlertTitle, message, options);
                if (onOK)
                    onOK();
            }
        };
        /**
         * Displays an error message, usually in a popup.
         */
        BasicsImpl.prototype.error = function (message, title, onOK, options) {
            if (YConfigs.Basics.MessageType === YetaWF.MessageTypeEnum.Popups || YVolatile.Basics.ForcePopup) {
                this.alert(message, title || YLocs.Basics.DefaultErrorTitle, onOK);
            }
            else {
                if (!options)
                    options = { encoded: false };
                if (options.canClose === undefined)
                    options.canClose = true;
                if (options.autoClose === undefined)
                    options.autoClose = BasicsImpl.DefaultTimeout;
                this.addToast(Severity.Error, title !== null && title !== void 0 ? title : YLocs.Basics.DefaultErrorTitle, message, options);
                if (onOK)
                    onOK();
            }
        };
        /**
         * Displays a confirmation message, usually in a popup.
         */
        BasicsImpl.prototype.confirm = function (message, title, onOK, options) {
            if (YConfigs.Basics.MessageType === YetaWF.MessageTypeEnum.Popups || YVolatile.Basics.ForcePopup) {
                this.alert(message, title || YLocs.Basics.DefaultSuccessTitle, onOK);
            }
            else {
                if (!options)
                    options = { encoded: false };
                if (options.canClose === undefined)
                    options.canClose = true;
                if (options.autoClose === undefined)
                    options.autoClose = BasicsImpl.DefaultTimeout;
                this.addToast(Severity.Info, title !== null && title !== void 0 ? title : YLocs.Basics.DefaultSuccessTitle, message, options);
                if (onOK)
                    onOK();
            }
        };
        /**
         * Displays an alert message, usually in a popup.
         */
        BasicsImpl.prototype.alert = function (message, title, onOK, options) {
            if (YConfigs.Basics.MessageType === YetaWF.MessageTypeEnum.Popups || YVolatile.Basics.ForcePopup) {
                YVolatile.Basics.ForcePopup = false;
                // check if we already have a popup (and close it)
                YetaWF_ComponentsHTML.DialogClass.close();
                options = options || { encoded: false };
                if (!options.encoded) {
                    // change (+nl) to <br/>
                    var escElement = document.createElement("div");
                    escElement.innerText = message;
                    var s = escElement.innerHTML;
                    escElement.remove();
                    message = s.replace(/\(\+nl\)/g, "<br/>");
                }
                YetaWF_ComponentsHTML.DialogClass.open({
                    width: YConfigs.Basics.DefaultAlertWaitWidth,
                    height: YConfigs.Basics.DefaultAlertWaitHeight,
                    title: title !== null && title !== void 0 ? title : YLocs.Basics.DefaultAlertTitle,
                    textHTML: message,
                    closeText: YLocs.Basics.CloseButtonText,
                    close: function () {
                        if (onOK)
                            onOK();
                    },
                    buttons: [
                        {
                            text: YLocs.Basics.OKButtonText,
                            click: function () {
                                if (onOK)
                                    onOK();
                            },
                        },
                    ],
                });
            }
            else {
                if (!options)
                    options = { encoded: false };
                if (options.canClose === undefined)
                    options.canClose = true;
                if (options.autoClose === undefined)
                    options.autoClose = BasicsImpl.DefaultTimeout;
                this.addToast(Severity.Info, title !== null && title !== void 0 ? title : "Success", message, options);
                if (onOK)
                    onOK();
            }
        };
        /**
         * Displays an alert message with Yes/No buttons, usually in a popup.
         */
        BasicsImpl.prototype.alertYesNo = function (message, title, onYes, onNo, options) {
            // change (+nl) to <br/>
            var escElement = document.createElement("div");
            escElement.innerText = message;
            var s = escElement.innerHTML;
            escElement.remove();
            message = s.replace(/\(\+nl\)/g, "<br/>");
            YetaWF_ComponentsHTML.DialogClass.open({
                width: YConfigs.Basics.DefaultAlertYesNoWidth,
                height: YConfigs.Basics.DefaultAlertYesNoHeight,
                title: title !== null && title !== void 0 ? title : YLocs.Basics.DefaultAlertYesNoTitle,
                textHTML: message,
                closeText: YLocs.Basics.CloseButtonText,
                close: function () {
                    if (onNo)
                        onNo();
                },
                buttons: [
                    {
                        text: YLocs.Basics.YesButtonText,
                        click: function () {
                            if (onYes)
                                onYes();
                        },
                    },
                    {
                        text: YLocs.Basics.NoButtonText,
                        click: function () {
                            if (onNo)
                                onNo();
                        },
                    }
                ],
            });
        };
        /**
         * Displays a "Please Wait" message
         */
        BasicsImpl.prototype.pleaseWait = function (message, title) {
            var escElement = document.createElement("div");
            escElement.innerText = message !== null && message !== void 0 ? message : YLocs.Basics.PleaseWaitText;
            message = escElement.innerHTML;
            escElement.remove();
            // show and center the window
            YetaWF_ComponentsHTML.DialogClass.openSimple({
                width: YConfigs.Basics.DefaultPleaseWaitWidth,
                height: YConfigs.Basics.DefaultPleaseWaitHeight,
                title: title !== null && title !== void 0 ? title : YLocs.Basics.PleaseWaitTitle,
                textHTML: message,
            });
        };
        /**
         * Closes the "Please Wait" message (if any).
         */
        BasicsImpl.prototype.pleaseWaitClose = function () {
            YetaWF_ComponentsHTML.DialogClass.close();
        };
        /**
         * Closes any open overlays, menus, dropdownlists, tooltips, etc. (Popup windows are not handled and are explicitly closed using $YetaWF.Popups)
         */
        BasicsImpl.prototype.closeOverlays = function () {
            // all MenuUL menus
            if (YetaWF_ComponentsHTML.MenuULComponent)
                YetaWF_ComponentsHTML.MenuULComponent.closeMenus();
            // tooltips
            ToolTipsHTMLHelper.removeTooltips();
            // dropdowns
            if (YetaWF_ComponentsHTML.DropDownListEditComponent)
                YetaWF_ComponentsHTML.DropDownListEditComponent.closeDropdowns();
            // Close any open menus
            if (YetaWF_ComponentsHTML.MenuComponent)
                YetaWF_ComponentsHTML.MenuComponent.closeAllMenus();
        };
        /**
         * Enable/disable an element.
         * Some child items need some extra settings when disabled=disabled isn't enough.
         * Also used to update visual styles to reflect the status.
         */
        BasicsImpl.prototype.elementEnableToggle = function (elem, enable) {
            elem.removeAttribute("disabled");
            if (!enable)
                elem.setAttribute("disabled", "disabled");
            if (elem.tagName === "A") {
                $YetaWF.elementRemoveClass(elem, "t_disabled");
                if (!enable)
                    $YetaWF.elementAddClass(elem, "t_disabled");
            }
            // mark submit/nosubmit
            if (enable) {
                if ($YetaWF.elementHasClass(elem, "yform-nosubmit-temp"))
                    $YetaWF.elementRemoveClasses(elem, ["yform-novalidate", "yform-nosubmit-temp", "yform-nosubmit"]);
            }
            else {
                if (!$YetaWF.elementHasClass(elem, "yform-nosubmit"))
                    $YetaWF.elementAddClasses(elem, ["yform-novalidate", "yform-nosubmit-temp", "yform-nosubmit"]);
            }
        };
        /**
         * Returns whether the element is enabled.
         */
        BasicsImpl.prototype.isEnabled = function (elem) {
            if ($YetaWF.getAttributeCond(elem, "disabled") != null)
                return false;
            if ($YetaWF.elementHasClass(elem, "t_disabled"))
                return false;
            return true;
        };
        /**
         * Given an element, returns the owner (typically a module) that owns the element.
         * The DOM hierarchy may not reflect this ownership, for example with popup menus which are appended to the <body> tag, but are owned by specific modules.
         */
        BasicsImpl.prototype.getOwnerFromTag = function (tag) {
            if (YetaWF_ComponentsHTML.MenuULComponent)
                return YetaWF_ComponentsHTML.MenuULComponent.getOwnerFromTag(tag);
            return null;
        };
        /**
         * Returns whether a message popup dialog is currently active.
         */
        BasicsImpl.prototype.messagePopupActive = function () {
            return YetaWF_ComponentsHTML.DialogClass.isActive;
        };
        BasicsImpl.prototype.addToast = function (severity, title, message, options) {
            var _this = this;
            var entry = {
                Severity: severity,
                Title: title,
                Text: message,
                CanClose: options.canClose === true,
                Timeout: options.autoClose ? options.autoClose : 0,
                Name: options.name,
                EntryDiv: null,
            };
            if (entry.Name) {
                var found = this.Toasts.find(function (toast) {
                    return entry.Name === toast.Name;
                });
                if (found) {
                    if (entry.Text === found.Text) // same text, leave as is
                        return;
                    else {
                        // new text, replace
                        this.removeToast(found.EntryDiv, found);
                    }
                }
            }
            if (!entry.Text) // empty text
                return;
            this.Toasts.push(entry);
            var entryDiv = document.createElement("div");
            entryDiv.style.opacity = "0";
            entry.EntryDiv = entryDiv;
            $YetaWF.setAttribute(entryDiv, "aria-atomic", "true");
            var html = "";
            if (title)
                html += "<div class='t_title'>".concat($YetaWF.htmlEscape(title), "</div>");
            if (options.canClose)
                //close button image
                html += "<div class='t_close' aria-label='Close'>".concat(YConfigs.YetaWF_ComponentsHTML.SVG_fas_multiply, "</div>");
            if (message) {
                if (!options.encoded) {
                    // change \n to <br/>
                    var s = $YetaWF.htmlEscape(message);
                    s = s.replace(/\(\+nl\)/g, "<br/>");
                    html += "<div class='t_message'>".concat(s, "</div>");
                }
                else {
                    html += "<div class='t_message'>".concat(message, "</div>");
                }
            }
            entryDiv.innerHTML = html;
            $YetaWF.elementAddClass(entryDiv, "t_entry");
            switch (severity) {
                default:
                case Severity.Info:
                    $YetaWF.setAttribute(entryDiv, "role", "status");
                    $YetaWF.setAttribute(entryDiv, "aria-live", "polite");
                    $YetaWF.elementAddClass(entryDiv, "t_info");
                    break;
                case Severity.Warning:
                    $YetaWF.setAttribute(entryDiv, "role", "alert");
                    $YetaWF.setAttribute(entryDiv, "aria-live", "assertive");
                    $YetaWF.elementAddClass(entryDiv, "t_warning");
                    break;
                case Severity.Error:
                    $YetaWF.setAttribute(entryDiv, "role", "alert");
                    $YetaWF.setAttribute(entryDiv, "aria-live", "assertive");
                    $YetaWF.elementAddClass(entryDiv, "t_error");
                    break;
            }
            var toastDiv = this.getToastDiv();
            toastDiv.appendChild(entryDiv);
            $YetaWF.animateHeight(entryDiv, true);
            entryDiv.style.opacity = "";
            if (options.canClose) {
                $YetaWF.registerEventHandler(entryDiv, "click", ".t_close", function (ev) {
                    _this.removeToast(entryDiv, entry);
                    return false;
                });
            }
            if (options.autoClose) {
                setTimeout(function () {
                    _this.removeToast(entryDiv, entry);
                }, options.autoClose);
            }
        };
        BasicsImpl.prototype.removeToast = function (entryDiv, entry) {
            var _this = this;
            entryDiv.style.opacity = "0"; // also animated
            $YetaWF.animateHeight(entryDiv, false, 300, function () {
                entryDiv.remove();
                _this.Toasts = _this.Toasts.filter(function (e) { return e !== entry; });
                if (_this.Toasts.length === 0) {
                    var toastDiv = $YetaWF.getElement1BySelectorCond(BasicsImpl.ToastDivSelector);
                    if (toastDiv)
                        toastDiv.remove();
                }
            });
        };
        BasicsImpl.prototype.getToastDiv = function () {
            // if we're in an iframe popup, find outer window to add toast div
            // if we're not in an iframe, window.parent simply returns window, so we're all good
            var doc = window.parent.document;
            var toastDiv = $YetaWF.getElement1BySelectorCond(BasicsImpl.ToastDivSelector, [doc.body]);
            if (!toastDiv) {
                toastDiv = document.createElement("div");
                if (YConfigs.Basics.MessageType === YetaWF.MessageTypeEnum.ToastRight)
                    $YetaWF.elementAddClass(toastDiv, "t_right");
                else
                    $YetaWF.elementAddClass(toastDiv, "t_left");
                toastDiv.id = "ytoast";
                $YetaWF.setAttribute(toastDiv, "aria-live", "polite");
                $YetaWF.setAttribute(toastDiv, "aria-atomic", "true");
                doc.body.appendChild(toastDiv);
            }
            return toastDiv;
        };
        BasicsImpl.ToastDivSelector = "#ytoast";
        BasicsImpl.DefaultTimeout = 7000;
        return BasicsImpl;
    }());
    YetaWF_ComponentsHTML.BasicsImpl = BasicsImpl;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
// tslint:disable-next-line:variable-name
var YetaWF_BasicsImpl = new YetaWF_ComponentsHTML.BasicsImpl();

//# sourceMappingURL=BasicsImpl.js.map
