/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

declare var signalR: any;// smh

namespace YetaWF_Messenger {

    export class SkinBrowserNotificationsModule {

        static readonly MODULEGUID: string = "7f60abc1-07a1-49f1-8381-bd4276977ff0";
        static readonly PROXY: string = "YetaWF_Messenger_BrowserNotificationsHub";

        static on: boolean = true;

        constructor() {

            if (!("Notification" in window)) {
                console.error("No notification support");
                return;
            }

            if (YConfigs.SignalR.Version === "MVC5") {
                var $$: any = $;
                var connection: any = $$.hubConnection(YConfigs.SignalR.Url, { useDefaultPath: false });
                var hubProxy: any = connection.createHubProxy(SkinBrowserNotificationsModule.PROXY);

                hubProxy.on("Message", (title: string, text: string, icon?: string, timeout?: number, url?: string): void => {
                    this.handleMessage(title, text, icon, timeout, url);
                });

                connection.start().done((): void => { /*empty*/ });
            } else {

                const connection = new signalR.HubConnectionBuilder()
                    .withUrl(`${YConfigs.SignalR.Url}/${SkinBrowserNotificationsModule.PROXY}`)
                    .configureLogging(signalR.LogLevel.Information)
                    .build();

                connection.on("Message", (title: string, text: string, icon?: string, timeout?: number, url?: string): void => {
                    this.handleMessage(title, text, icon, timeout, url);
                });

                connection.start().then((): void => { /*empty*/ });
            }
        }

        private handleMessage(title: string, text: string, icon?: string, timeout?: number, url?: string): void {

            if (SkinBrowserNotificationsModule.on) {

                switch (Notification.permission) {
                    case "default":
                        Notification.requestPermission().then((result: NotificationPermission): void => {
                            if (result === "granted") {
                                this.showNotification(title, text, icon, timeout, url);
                            }
                        });
                        break;
                    case "granted":
                        this.showNotification(title, text, icon, timeout, url);
                        break;
                    default:
                        console.error("No permission to show notification");
                }
            }
        }

        private showNotification(title: string, text: string, icon?: string, timeout?: number, url?: string): void {
            var notification = new Notification(title, { body: text, icon: icon, tag: "YetaWF_Messenger.BrowserNotification" });
            if (url) {
                notification.addEventListener("click", (ev: Event): void => {
                    window.open(url, "_blank");
                });
            }
            if (timeout)
                setTimeout(notification.close.bind(notification), timeout);
        }
    }

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, (ev: CustomEvent<YetaWF.DetailsAddonChanged>): boolean => {
        let addonGuid = ev.detail.addonGuid;
        let on = ev.detail.on;
        if (addonGuid === SkinBrowserNotificationsModule.MODULEGUID) {
            SkinBrowserNotificationsModule.on = on;
        }
        return true;
    });
}
