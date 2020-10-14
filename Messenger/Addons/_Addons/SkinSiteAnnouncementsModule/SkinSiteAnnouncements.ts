/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

namespace YetaWF_Messenger {

    export class SkinSiteAnnouncementsModule {

        static readonly MODULEGUID: string = "54f6b691-b835-4568-90aa-aa9b308d4272";
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

        }
        private handleMessage(content: string, title: string): void {
            if (SkinSiteAnnouncementsModule.on)
                $YetaWF.alert(content, title, undefined, { encoded: true, canClose: true, autoClose: 0 });
        }
    }

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, (ev: CustomEvent<YetaWF.DetailsAddonChanged>): boolean => {
        let addonGuid = ev.detail.addonGuid;
        let on = ev.detail.on;
        if (addonGuid === SkinSiteAnnouncementsModule.MODULEGUID) {
            SkinSiteAnnouncementsModule.on = on;
        }
        return true;
    });
}
