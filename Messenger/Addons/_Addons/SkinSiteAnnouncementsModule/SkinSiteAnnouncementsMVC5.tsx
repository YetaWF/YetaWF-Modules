/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

namespace YetaWF_Messenger {

    export class SkinSiteAnnouncementsModule {

        static readonly MODULEGUID: string = "54F6B691-B835-4568-90AA-AA9B308D4272";

        static on: boolean = true;

        constructor() {

            var $$: any = $;
            var connection: any = $$.hubConnection(YConfigs.SignalR.Url, { useDefaultPath: false });
            var hubProxy: any = connection.createHubProxy("YetaWF_Messenger_SiteAnnouncementsHub");

            hubProxy.on("Message", (content: string, title: string): void => {
                if (SkinSiteAnnouncementsModule.on)
                    $YetaWF.alert(content, title, undefined, { encoded: true });
            });

            connection.start().done((): void => { /*empty*/ });

            $YetaWF.registerContentChange((addonGuid: string, on: boolean): void => {
                if (addonGuid === SkinSiteAnnouncementsModule.MODULEGUID) {
                    SkinSiteAnnouncementsModule.on = on;
                }
            });
        }
    }
}
