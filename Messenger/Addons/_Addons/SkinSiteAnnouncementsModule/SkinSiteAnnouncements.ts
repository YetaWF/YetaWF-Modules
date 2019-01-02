/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

namespace YetaWF_Messenger {

    export class SkinSiteAnnouncementsModule {

        static readonly MODULEGUID: string = "54F6B691-B835-4568-90AA-AA9B308D4272";
        static readonly PROXY: string = "YetaWF_Messenger_SiteAnnouncementsHub";

        static on: boolean = true;

        constructor() {

            if (YConfigs.SignalR.Version === "MVC5") {
                var $$: any = $;
                var connection: any = $$.hubConnection(YConfigs.SignalR.Url, { useDefaultPath: false });
                var hubProxy: any = connection.createHubProxy(SkinSiteAnnouncementsModule.PROXY);

                hubProxy.on("Message", (content: string, title: string): void => {
                    this.handleMessage(content, title);
                });

                connection.start().done((): void => { /*empty*/ });
            } else {

                const connection = new signalR.HubConnectionBuilder()
                    .withUrl(`${YConfigs.SignalR.Url}/${SkinSiteAnnouncementsModule.PROXY}`)
                    .configureLogging(signalR.LogLevel.Information)
                    .build();

                connection.on("Message", (content: string, title: string): void => {
                    this.handleMessage(content, title);
                });

                connection.start().then((): void => { /*empty*/ });
            }

            $YetaWF.registerContentChange((addonGuid: string, on: boolean): void => {
                if (addonGuid === SkinSiteAnnouncementsModule.MODULEGUID) {
                    SkinSiteAnnouncementsModule.on = on;
                }
            });
        }
        private handleMessage(content: string, title: string): void {
            if (SkinSiteAnnouncementsModule.on)
                $YetaWF.alert(content, title, undefined, { encoded: true });
        }
    }
}
