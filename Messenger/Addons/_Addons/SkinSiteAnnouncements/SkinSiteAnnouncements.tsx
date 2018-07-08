/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

namespace YetaWF_Messenger {

    export interface IPackageConfigs {
        SignalRUrl: string;
    }

    class SkinSiteAnnouncementsModule {

        static readonly MODULEGUID: string = "54F6B691-B835-4568-90AA-AA9B308D4272";

        static on: boolean = true;

        /**
         * Initializes the module instance.
         */
        init(): void {

            YetaWF_Basics.RegisterContentChange(function (event: Event, addonGuid: string, on: boolean): void {
                if (addonGuid === SkinSiteAnnouncementsModule.MODULEGUID) {
                    SkinSiteAnnouncementsModule.on = on;
                }
            });

            var $$: any = $;
            var connection: any = $$.hubConnection(YConfigs.YetaWF_Messenger.SignalRUrl, { useDefaultPath: false });
            var hubProxy: any = connection.createHubProxy("YetaWF_Messenger_SiteAnnouncement");
            hubProxy.on("message", function (content: string, title: string): void {
                if (SkinSiteAnnouncementsModule.on)
                    YetaWF_Basics.alert(content, title, null, { encoded: true });
            });
            connection.start().done(function (): void { /*empty*/ });
        }
    }

    var announceMod: SkinSiteAnnouncementsModule = new SkinSiteAnnouncementsModule();
    announceMod.init();
}
