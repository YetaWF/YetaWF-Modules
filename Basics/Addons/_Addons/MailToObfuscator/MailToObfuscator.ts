/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

namespace YetaWF_Basics {

    export class MailtoObfuscatorSkinModule {

        public static on: boolean = true;

    }

    // http://stackoverflow.com/questions/483212/effective-method-to-hide-email-from-spam-bots
    $YetaWF.addWhenReady((tag: HTMLElement): void => {
        if (MailtoObfuscatorSkinModule.on) {
            // find all <a> YGenMailTo tags and format an email address
            var elems = $YetaWF.getElementsBySelector("a.YGenMailTo", [tag]) as HTMLAnchorElement[];
            for (var elem of elems) {
                var addr = $YetaWF.getAttribute(elem, "data-name") + "@" + $YetaWF.getAttribute(elem, "data-domain");
                var s = "mailto:" + addr;

                var subj = $YetaWF.getAttributeCond(elem, "data-subject");
                if (subj) {
                    subj = subj.replace(" ", "+");
                    s += "?subject=" + encodeURI(subj);
                }
                var text = $YetaWF.getAttributeCond(elem, "data-text");
                if (!text)
                    elem.innerText = addr;
                else
                    elem.innerText = text;
                elem.href = s;
            }
        }
    });

    // Handles events turning the addon on/off (used for dynamic content)
    $YetaWF.registerContentChange((addonGuid: string, on: boolean): void => {
        if (addonGuid === "749d0ca9-75e5-40b8-82e3-466a11d3b1d2") {
            MailtoObfuscatorSkinModule.on = on;
        }
    });
}