/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

declare var grecaptcha: any;

namespace YetaWF_ComponentsHTML {

    export interface IPackageConfigs {
        SiteKey: string;
        Theme: string;
        Size: string;
    }

    export class RecaptchaV2 {

        public static recaptchaInit(id: string): void {
            var recaptcha = $YetaWF.getElementById(id);
            RecaptchaV2.onLoad(recaptcha);
        }

        public static onLoad(tag: HTMLElement): void {

            if (typeof grecaptcha === "undefined" || !grecaptcha.render) {
                // keep trying until grecaptcha is available
                setTimeout((): void => {
                    RecaptchaV2.onLoad(tag);
                }, 100);
                return;
            }
            grecaptcha.render(tag, {
                "sitekey": YConfigs.YetaWF_ComponentsHTML.SiteKey,
                "theme": YConfigs.YetaWF_ComponentsHTML.Theme,
                "size": YConfigs.YetaWF_ComponentsHTML.Size,
            });
        }
    }
}
