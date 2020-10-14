"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var TextAreaEditComponent = /** @class */ (function (_super) {
        __extends(TextAreaEditComponent, _super);
        function TextAreaEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, TextAreaEditComponent.TEMPLATE, TextAreaEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.TextArea,
                ChangeEvent: null,
                GetValue: function (control) {
                    return control.value;
                },
                Enable: function (control, enable, clearOnDisable) {
                    var editor = CKEDITOR.instances[control.id];
                    if (enable) {
                        control.removeAttribute("readonly");
                        $YetaWF.elementRemoveClass(control, "k-state-disabled");
                        try {
                            editor.setReadOnly(false);
                        }
                        catch (e) { }
                    }
                    else {
                        control.setAttribute("readonly", "readonly");
                        $YetaWF.elementAddClass(control, "k-state-disabled");
                        try {
                            editor.setReadOnly(true);
                        }
                        catch (e) { }
                        if (clearOnDisable)
                            editor.setData("");
                    }
                },
            }) || this;
            _this.Setup = setup;
            var config = {
                customConfig: setup.CDNUrl,
                height: setup.EmHeight + "em",
                allowedContent: setup.RestrictedHtml ? false : true,
                filebrowserWindowFeatures: setup.FilebrowserWindowFeatures
            };
            if (setup.FilebrowserImageBrowseUrl) {
                config.filebrowserImageBrowseUrl = setup.FilebrowserImageBrowseUrl;
                config.filebrowserImageBrowseLinkUrl = setup.FilebrowserImageBrowseUrl;
            }
            if (setup.FilebrowserPageBrowseUrl) {
                config.filebrowserBrowseUrl = setup.FilebrowserPageBrowseUrl;
            }
            var ckEd = CKEDITOR.replace(controlId, config);
            ckEd.on("blur", function () {
                _this.Control.value = ckEd.getData();
                FormsSupport.validateElementFully(_this.Control);
            });
            // save data in the textarea field when the form is submitted
            $YetaWF.Forms.addPreSubmitHandler(setup.InPartialView, {
                form: $YetaWF.Forms.getForm($YetaWF.getElementById(controlId)),
                callback: function (entry) {
                    var ckEd = entry.userdata;
                    _this.Control.value = ckEd.getData();
                },
                userdata: ckEd,
            });
            return _this;
        }
        TextAreaEditComponent.TEMPLATE = "yt_textarea";
        TextAreaEditComponent.SELECTOR = ".yt_textarea.t_edit";
        return TextAreaEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.TextAreaEditComponent = TextAreaEditComponent;
    // Override the built-in Save button to use our Form submit
    // Need to wait for the ckeditor instance to finish initialization
    // because CKEDITOR.instances.editor.commands is an empty object
    // if you try to use it immediately after CKEDITOR.replace('editor');
    if (typeof CKEDITOR !== "undefined") { // CKEDITOR is only defined when an editable textarea is used
        CKEDITOR.on("instanceReady", function (ev) {
            // Replace the old save's exec function with the new one
            if (ev.editor.commands.save) {
                // Create a new command with the desired exec function
                var overridecmd = new CKEDITOR.command(ev.editor, {
                    exec: function (editor) {
                        var $form = $(editor.element.$).closest("form." + YConfigs.Forms.CssFormAjax);
                        if ($form.length !== 1)
                            throw "Couldn't find form"; /*DEBUG*/
                        $YetaWF.Forms.submit($form[0], false);
                    }
                });
                ev.editor.commands.save.exec = overridecmd.exec;
            }
        });
    }
    // when a tab page is switched, resize all the ckeditors in the newly visible panel (custom event)
    // when we're in a float div (property list or tabbed property list) the parent width isn't available until after the
    // page has completely loaded, so we need to set it again.
    // For other cases (outside float div) this does no harm and resizes to the current size.
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTACTIVATEDIV, null, function (ev) {
        var ckeds = $YetaWF.getElementsBySelector(TextAreaEditComponent.SELECTOR, ev.detail.tags);
        for (var _i = 0, ckeds_1 = ckeds; _i < ckeds_1.length; _i++) {
            var cked = ckeds_1[_i];
            var ctrl = TextAreaEditComponent.getControlFromTag(cked, TextAreaEditComponent.SELECTOR);
            var ck = CKEDITOR.instances[cked.id];
            try {
                ck.resize("100%", ctrl.Setup.EmHeight, true);
            }
            catch (e) { }
        }
        return true;
    });
    // A <div> is being emptied. Destroy all ckeditors the <div> may contain.
    $YetaWF.registerClearDiv(function (tag) {
        var list = $YetaWF.getElementsBySelector("textarea.yt_textarea", [tag]);
        for (var _i = 0, list_1 = list; _i < list_1.length; _i++) {
            var el = list_1[_i];
            if (CKEDITOR.instances[el.id])
                CKEDITOR.instances[el.id].destroy();
        }
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=TextArea.js.map
