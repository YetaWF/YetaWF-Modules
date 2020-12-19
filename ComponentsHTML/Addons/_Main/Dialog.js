"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var DialogClass = /** @class */ (function () {
        function DialogClass() {
        }
        DialogClass.open = function (setup) {
            var _a;
            DialogClass.Setup = setup;
            DialogClass.DragDropInProgress = false;
            var dialog = $YetaWF.createElement("div", { id: "yDialogContainer", tabindex: "-1", role: "dialog", class: "ui-dialog ui-corner-all ui-widget ui-widget-content ui-front ui-dialog-buttons ui-draggable", "aria-describedby": "yAlert", "aria-labelledby": "yAlertTitle" },
                $YetaWF.createElement("div", { class: "ui-dialog-titlebar ui-corner-all ui-widget-header ui-helper-clearfix ui-draggable-handle" },
                    $YetaWF.createElement("span", { id: "yAlertTitle", class: "ui-dialog-title" }, setup.title),
                    $YetaWF.createElement("button", { type: "button", class: "ui-button ui-corner-all ui-widget ui-button-icon-only ui-dialog-titlebar-close", "data-tooltip": (_a = setup.closeText) !== null && _a !== void 0 ? _a : "" },
                        $YetaWF.createElement("span", { class: "ui-button-icon ui-icon ui-icon-closethick" }),
                        $YetaWF.createElement("span", { class: "ui-button-icon-space" }, " "),
                        "Close")),
                $YetaWF.createElement("div", { id: "yAlert", class: "ui-dialog-content ui-widget-content", style: "width: auto; min-height: 25px; max-height: none; height: auto;" }),
                $YetaWF.createElement("div", { class: "ui-dialog-buttonpane ui-widget-content ui-helper-clearfix" },
                    $YetaWF.createElement("div", { class: "ui-dialog-buttonset" })));
            $YetaWF.getElement1BySelector("#yAlert", [dialog]).innerHTML = setup.textHTML;
            // handle all buttons
            var buttonSet = $YetaWF.getElement1BySelector(".ui-dialog-buttonset", [dialog]);
            if (setup.buttons) {
                var _loop_1 = function (buttonDef) {
                    var button = $YetaWF.createElement("button", { type: "button", class: "ui-button ui-corner-all ui-widget" }, buttonDef.text);
                    buttonSet.appendChild(button);
                    $YetaWF.registerEventHandler(button, "click", null, function (ev) {
                        DialogClass.close();
                        buttonDef.click();
                        return false;
                    });
                };
                for (var _i = 0, _b = setup.buttons; _i < _b.length; _i++) {
                    var buttonDef = _b[_i];
                    _loop_1(buttonDef);
                }
            }
            // handle close button
            var closeButton = $YetaWF.getElement1BySelector(".ui-dialog-titlebar button", [dialog]);
            $YetaWF.registerEventHandler(closeButton, "click", null, function (ev) {
                DialogClass.close();
                if (setup.close)
                    setup.close();
                return false;
            });
            $YetaWF.registerEventHandler(dialog, "keydown", null, function (ev) {
                if (ev.key === "Escape") {
                    DialogClass.close();
                    if (setup.close)
                        setup.close();
                    return false;
                }
                return true;
            });
            $YetaWF.registerEventHandler(dialog, "mousedown", ".ui-dialog-titlebar", function (ev) {
                var dialog = $YetaWF.getElementById("yDialogContainer");
                var drect = dialog.getBoundingClientRect();
                DialogClass.XOffsetDD = ev.clientX - drect.left;
                DialogClass.YOffsetDD = ev.clientY - drect.top;
                DialogClass.DragDropInProgress = true;
                return false;
            });
            $YetaWF.registerEventHandler(dialog, "mouseup", null, function (ev) {
                DialogClass.DragDropInProgress = false;
                return false;
            });
            dialog.style.display = "none";
            document.body.appendChild(dialog);
            DialogClass.addOverlay();
            dialog.style.display = "";
            DialogClass.reposition();
            // set focus on first button
            $YetaWF.getElementsBySelector("button", [buttonSet])[0].focus();
            DialogClass.setupDragDrop();
        };
        DialogClass.openSimple = function (setup) {
            DialogClass.Setup = setup;
            DialogClass.DragDropInProgress = false;
            var dialog = $YetaWF.createElement("div", { id: "yDialogContainer", tabindex: "-1", role: "dialog", class: "ui-dialog ui-corner-all ui-widget ui-widget-content ui-front ui-dialog-buttons ui-draggable", "aria-describedby": "yAlert", "aria-labelledby": "yAlertTitle" },
                $YetaWF.createElement("div", { class: "ui-dialog-titlebar ui-corner-all ui-widget-header ui-helper-clearfix ui-draggable-handle" },
                    $YetaWF.createElement("span", { id: "yAlertTitle", class: "ui-dialog-title" }, setup.title)),
                $YetaWF.createElement("div", { id: "yAlert", class: "ui-dialog-content ui-widget-content", style: "width: auto; min-height: 25px; max-height: none; height: auto;" }));
            $YetaWF.getElement1BySelector("#yAlert", [dialog]).innerHTML = setup.textHTML;
            dialog.style.display = "none";
            document.body.appendChild(dialog);
            DialogClass.addOverlay();
            dialog.style.display = "";
            DialogClass.reposition();
            DialogClass.setupDragDrop();
        };
        DialogClass.close = function () {
            DialogClass.DragDropInProgress = false;
            var dialog = $YetaWF.getElementByIdCond("yDialogContainer");
            if (!dialog)
                return;
            dialog.remove();
            var overlay = $YetaWF.getElement1BySelector(".ui-widget-overlay.ui-front");
            overlay.remove();
        };
        Object.defineProperty(DialogClass, "isActive", {
            get: function () {
                return $YetaWF.getElementByIdCond("yDialogContainer") != null;
            },
            enumerable: false,
            configurable: true
        });
        DialogClass.addOverlay = function () {
            var overlay = $YetaWF.createElement("div", { class: "ui-widget-overlay ui-front" });
            document.body.appendChild(overlay);
        };
        DialogClass.setupDragDrop = function () {
            var dialog = $YetaWF.getElementById("yDialogContainer");
            $YetaWF.registerEventHandler(dialog, "mousedown", ".ui-dialog-titlebar", function (ev) {
                var dialog = $YetaWF.getElementById("yDialogContainer");
                var drect = dialog.getBoundingClientRect();
                DialogClass.XOffsetDD = ev.clientX - drect.left;
                DialogClass.YOffsetDD = ev.clientY - drect.top;
                DialogClass.DragDropInProgress = true;
                return false;
            });
            $YetaWF.registerEventHandler(dialog, "mouseup", null, function (ev) {
                DialogClass.DragDropInProgress = false;
                return false;
            });
        };
        DialogClass.reposition = function () {
            DialogClass.DragDropInProgress = false;
            var dialog = $YetaWF.getElementByIdCond("yDialogContainer");
            if (!dialog)
                return;
            var setup = DialogClass.Setup;
            if (window.innerWidth <= setup.width) {
                dialog.style.width = window.innerWidth - 20 + "px";
                dialog.style.height = setup.height > 0 ? setup.height + "px" : "auto";
            }
            else {
                dialog.style.width = setup.width + "px";
                dialog.style.height = "auto";
            }
            // set maxheight for message text
            var text = $YetaWF.getElementById("yAlert");
            text.style.maxHeight = window.innerHeight * 3 / 4 + "px";
            // center
            var drect = dialog.getBoundingClientRect();
            var left = (window.innerWidth - drect.width) / 2;
            var top = (window.innerHeight - drect.height) / 2;
            dialog.style.left = left + "px"; // or + window.pageXOffset if position:absolute
            dialog.style.top = top + "px"; //  + window.pageYOffset
        };
        DialogClass.DragDropInProgress = false;
        DialogClass.XOffsetDD = 0;
        DialogClass.YOffsetDD = 0;
        return DialogClass;
    }());
    YetaWF_ComponentsHTML.DialogClass = DialogClass;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, function (ev) {
        DialogClass.reposition();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
        DialogClass.reposition();
        return true;
    });
    $YetaWF.registerEventHandlerBody("mousemove", null, function (ev) {
        if (DialogClass.DragDropInProgress) {
            var dialog = $YetaWF.getElementById("yDialogContainer");
            var drect = dialog.getBoundingClientRect();
            var left = ev.clientX - DialogClass.XOffsetDD;
            if (left + drect.width > window.innerWidth)
                left = window.innerWidth - drect.width;
            if (left < 0)
                left = 0;
            var top_1 = ev.clientY - DialogClass.YOffsetDD;
            if (top_1 + drect.height > window.innerHeight)
                top_1 = window.innerHeight - drect.height;
            if (top_1 < 0)
                top_1 = 0;
            dialog.style.left = left + "px";
            dialog.style.top = top_1 + "px";
            return false;
        }
        return true;
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Dialog.js.map
