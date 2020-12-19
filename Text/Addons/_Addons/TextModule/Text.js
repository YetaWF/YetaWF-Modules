"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Text#License */
var YetaWF_Text;
(function (YetaWF_Text) {
    $YetaWF.registerEventHandlerBody("click", ".YetaWF_Text .FAQ_Q", function (ev) {
        var elem = ev.__YetaWFElem;
        for (elem = elem.nextElementSibling; elem; elem = elem.nextElementSibling) {
            if ($YetaWF.elementHasClass(elem, "FAQ_A")) {
                if (elem.style.display === "" || elem.style.display === "none")
                    elem.style.display = "block";
                else
                    elem.style.display = "none";
                break;
            }
        }
        return false;
    });
})(YetaWF_Text || (YetaWF_Text = {}));

//# sourceMappingURL=Text.js.map
