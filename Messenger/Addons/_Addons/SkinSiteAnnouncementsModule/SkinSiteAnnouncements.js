"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */
var YetaWF_Messenger;
(function (YetaWF_Messenger) {
    var SkinSiteAnnouncementsModule = /** @class */ (function () {
        function SkinSiteAnnouncementsModule() {
            $YetaWF.registerContentChange(function (addonGuid, on) {
                if (addonGuid === SkinSiteAnnouncementsModule.MODULEGUID) {
                    SkinSiteAnnouncementsModule.on = on;
                }
            });
            var $$ = $;
            var connection = $$.hubConnection(YConfigs.YetaWF_Messenger.SignalRUrl, { useDefaultPath: false });
            var hubProxy = connection.createHubProxy("YetaWF_Messenger_SiteAnnouncement");
            hubProxy.on("message", function (content, title) {
                if (SkinSiteAnnouncementsModule.on)
                    $YetaWF.alert(content, title, undefined, { encoded: true });
            });
            connection.start().done(function () { });
        }
        SkinSiteAnnouncementsModule.MODULEGUID = "54F6B691-B835-4568-90AA-AA9B308D4272";
        SkinSiteAnnouncementsModule.on = true;
        return SkinSiteAnnouncementsModule;
    }());
    YetaWF_Messenger.SkinSiteAnnouncementsModule = SkinSiteAnnouncementsModule;
})(YetaWF_Messenger || (YetaWF_Messenger = {}));
