/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

namespace YetaWF_Basics {

    export class MailtoObfuscatorSkinModule {

        public static on: boolean = true;

    }

    // http://stackoverflow.com/questions/483212/effective-method-to-hide-email-from-spam-bots
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, (ev: CustomEvent<YetaWF.DetailsEventNavPageLoaded>): boolean => {
        if (MailtoObfuscatorSkinModule.on) {
            for (let tag of ev.detail.containers) {
            // find all <a> YGenMailTo tags and format an email address
                let elems = $YetaWF.getElementsBySelector("a.YGenMailTo", [tag]) as HTMLAnchorElement[];
                for (let elem of elems) {
                    let addr = $YetaWF.getAttribute(elem, "data-name") + "@" + $YetaWF.getAttribute(elem, "data-domain");
                    let s = "mailto:" + addr;

                    let subj = $YetaWF.getAttributeCond(elem, "data-subject");
                    if (subj) {
                        subj = subj.replace(" ", "+");
                        s += "?subject=" + encodeURI(subj);
                    }
                    let text = $YetaWF.getAttributeCond(elem, "data-text");
                    if (!text)
                        elem.innerText = addr;
                    else
                        elem.innerText = text;
                    elem.href = s;
                }
            }
        }
        return true;
    });

    // Handles events turning the addon on/off (used for dynamic content)
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, (ev: CustomEvent<YetaWF.DetailsAddonChanged>): boolean => {
        let addonGuid = ev.detail.addonGuid;
        let on = ev.detail.on;
        if (addonGuid === "749d0ca9-75e5-40b8-82e3-466a11d3b1d2") {
            MailtoObfuscatorSkinModule.on = on;
        }
        return true;
    });
}