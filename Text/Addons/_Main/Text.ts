/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Text#License */

namespace YetaWF_Text {

    $YetaWF.registerEventHandlerBody("click", ".YetaWF_Text .FAQ_Q", (ev: MouseEvent): boolean => {
        var elem = ev.__YetaWFElem;
        for (elem = elem.nextElementSibling as HTMLElement; elem; elem = elem.nextElementSibling as HTMLElement) {
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

}
