/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

var Y_Message: any;

namespace YetaWF_Messenger {

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
            var connection = $$.hubConnection(YConfigs.Basics.SignalRUrl, { useDefaultPath: false });
            var hubProxy = connection.createHubProxy('YetaWF_Messenger_SiteAnnouncement');
            hubProxy.on('message', function (content) {
                if (SkinSiteAnnouncementsModule.on)
                    Y_Message(content, null, { encoded: true });
            });
            connection.start().done(function () { });
        }
    }

    var announceMod: SkinSiteAnnouncementsModule = new SkinSiteAnnouncementsModule();
    announceMod.init();
}
