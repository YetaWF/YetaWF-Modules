/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export class SMTPServerEdit extends YetaWF.ComponentBaseDataImpl  {

        public static readonly TEMPLATE: string = "yt_smtpserver";
        public static readonly SELECTOR: string = ".yt_smtpserver.t_edit";

        private Server: HTMLInputElement;
        private Port: IntValueEditComponent;
        private Auth: HTMLSelectElement;
        private Button: HTMLAnchorElement;

        public constructor(controlId: string/*, setup: SMTPServerEditSetup*/) {
            super(controlId, SMTPServerEdit.TEMPLATE, SMTPServerEdit.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: (control: SMTPServerEdit): string | null => {
                    return control.Server.value;
                },
                Enable: (control: SMTPServerEdit, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                    if (clearOnDisable)
                        control.clear();
                },
            });

            this.Server = $YetaWF.getElement1BySelector("input[name$='.Server']", [this.Control]) as HTMLInputElement;
            this.Port = IntValueEditComponent.getControlFromSelector("input[name$='.Port']", IntValueEditComponent.SELECTOR, [this.Control]);
            this.Auth = $YetaWF.getElement1BySelector("select[name$='.Authentication']", [this.Control]) as HTMLSelectElement;
            this.Button = $YetaWF.getElement1BySelector(".t_sendtestemail a", [this.Control]) as HTMLAnchorElement;

            $YetaWF.registerEventHandler(this.Button, "click", null, (ev: Event): boolean => {
                var uri = new YetaWF.Url();
                uri.parse(this.Button.href);
                uri.removeSearch("Server");
                uri.removeSearch("Port");
                uri.removeSearch("Authentication");
                uri.removeSearch("UserName");
                uri.removeSearch("Password");
                uri.removeSearch("SSL");

                uri.addSearch("Server", this.Server.value);
                let port = this.Port.value;
                if (port === 0) {
                    port = 25;
                    this.Port.value = port;
                }
                uri.addSearch("Port", port);
                uri.addSearch("Authentication", this.Auth.value);
                var userName = $YetaWF.getElement1BySelector("input[name$='.UserName']", [this.Control]) as HTMLInputElement;
                uri.addSearch("UserName", userName.value);
                var password = $YetaWF.getElement1BySelector("input[name$='.Password']", [this.Control]) as HTMLInputElement;
                uri.addSearch("Password", password.value);
                var ssl = $YetaWF.getElement1BySelector("input[name$='.SSL']", [this.Control]) as HTMLInputElement;
                uri.addSearch("SSL", ssl.checked ? "true" : "false");
                this.Button.href = uri.toUrl();
                return true;
            });
        }

        public clear(): void {
            this.Server.value = "";
            this.Port.value = 25;
            ($YetaWF.getElement1BySelector("input[name$='.UserName']", [this.Control]) as HTMLInputElement).value = "";
            ($YetaWF.getElement1BySelector("input[name$='.Password']", [this.Control]) as HTMLInputElement).value = "";
            ($YetaWF.getElement1BySelector("input[name$='.SSL']", [this.Control]) as HTMLInputElement).checked = false;
        }
        public enable(enabled: boolean): void {
            $YetaWF.elementEnableToggle(this.Server, enabled);
        }
    }
}