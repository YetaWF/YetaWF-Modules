/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */
var Y_Alert;
var YetaWF_Messenger;
(function (YetaWF_Messenger) {
    var SkinSiteAnnouncementsModule = /** @class */ (function () {
        function SkinSiteAnnouncementsModule() {
        }
        /**
         * Initializes the module instance.
         */
        SkinSiteAnnouncementsModule.prototype.init = function () {
            YetaWF_Basics.RegisterContentChange(function (event, addonGuid, on) {
                if (addonGuid === SkinSiteAnnouncementsModule.MODULEGUID) {
                    SkinSiteAnnouncementsModule.on = on;
                }
            });
            var $$ = $;
            var connection = $$.hubConnection(YConfigs.Basics.SignalRUrl, { useDefaultPath: false });
            var hubProxy = connection.createHubProxy('YetaWF_Messenger_SiteAnnouncement');
            hubProxy.on('message', function (content, title) {
                if (SkinSiteAnnouncementsModule.on)
                    Y_Alert(content, title, null, { encoded: true });
            });
            connection.start().done(function () { });
        };
        SkinSiteAnnouncementsModule.MODULEGUID = "54F6B691-B835-4568-90AA-AA9B308D4272";
        SkinSiteAnnouncementsModule.on = true;
        return SkinSiteAnnouncementsModule;
    }());
    var announceMod = new SkinSiteAnnouncementsModule();
    announceMod.init();
})(YetaWF_Messenger || (YetaWF_Messenger = {}));

//# sourceMappingURL=SkinSiteAnnouncements.js.map
