"use strict";
/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var TextAreaEditComponent = /** @class */ (function () {
        function TextAreaEditComponent() {
        }
        return TextAreaEditComponent;
    }());
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
    $YetaWF.registerActivateDiv(function (div) {
        var ckeds = $YetaWF.getElementsBySelector(".yt_textarea.t_edit", [div]);
        for (var _i = 0, ckeds_1 = ckeds; _i < ckeds_1.length; _i++) {
            var cked = ckeds_1[_i];
            var ck = CKEDITOR.instances[cked.id];
            ck.resize("100%", $YetaWF.getAttribute(cked, "data-height"), true);
        }
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
