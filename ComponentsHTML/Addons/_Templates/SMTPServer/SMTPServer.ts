/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    //$$$$ needs ComponentBaseNoDataImpl
    export class SMTPServer {

        private Control: HTMLElement;
        private Server: HTMLInputElement;
        private Port: HTMLInputElement;
        private Auth: HTMLSelectElement;
        private Button: HTMLAnchorElement;

        public constructor(id: string) {

            this.Control = $YetaWF.getElementById(id);

            this.Server = $YetaWF.getElement1BySelector("input[name$='.Server']", [this.Control]) as HTMLInputElement;
            this.Port = $YetaWF.getElement1BySelector("input[name$='.Port']", [this.Control]) as HTMLInputElement;
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
                var port = this.Port.value;
                if (port.trim() === "") {
                    port = "25";
                    this.Port.value = port;
                }
                uri.addSearch("Port", port);
                uri.addSearch("Authentication", this.Auth.value);
                var userName = $YetaWF.getElement1BySelector("input[name$='.UserName']") as HTMLInputElement;
                uri.addSearch("UserName", userName.value);
                var password = $YetaWF.getElement1BySelector("input[name$='.Password']") as HTMLInputElement;
                uri.addSearch("Password", password.value);
                var ssl = $YetaWF.getElement1BySelector("input[name$='.SSL']") as HTMLInputElement;
                uri.addSearch("SSL", ssl.checked ? "true" : "false");
                this.Button.href = uri.toUrl();
                return true;
            });
        }
    }
}