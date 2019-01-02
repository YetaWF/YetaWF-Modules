"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */
var YetaWF_Messenger;
(function (YetaWF_Messenger) {
    var SkinBrowserNotificationsModule = /** @class */ (function () {
        function SkinBrowserNotificationsModule() {
            var _this = this;
            if (!("Notification" in window)) {
                console.error("No notification support");
                return;
            }
            var $$ = $;
            var connection = $$.hubConnection(YConfigs.SignalR.Url, { useDefaultPath: false });
            var hubProxy = connection.createHubProxy("YetaWF_Messenger_BrowserNotificationsHub");
            hubProxy.on("Message", function (title, text, icon, timeout, url) {
                if (SkinBrowserNotificationsModule.on) {
                    switch (Notification.permission) {
                        case "default":
                            Notification.requestPermission().then(function (result) {
                                if (result === "granted") {
                                    _this.showNotification(title, text, icon, timeout, url);
                                }
                            });
                            break;
                        case "granted":
                            _this.showNotification(title, text, icon, timeout, url);
                            break;
                        default:
                            console.error("No permission to show notification");
                    }
                }
            });
            connection.start().done(function () { });
            $YetaWF.registerContentChange(function (addonGuid, on) {
                if (addonGuid === SkinBrowserNotificationsModule.MODULEGUID) {
                    SkinBrowserNotificationsModule.on = on;
                }
            });
        }
        SkinBrowserNotificationsModule.prototype.showNotification = function (title, text, icon, timeout, url) {
            var notification = new Notification(title, { body: text, icon: icon, tag: "YetaWF_Messenger.BrowserNotification" });
            if (url) {
                notification.addEventListener("click", function (ev) {
                    window.open(url, "_blank");
                });
            }
            if (timeout)
                setTimeout(notification.close.bind(notification), timeout);
        };
        SkinBrowserNotificationsModule.MODULEGUID = "7F60ABC1-07A1-49f1-8381-BD4276977FF0";
        SkinBrowserNotificationsModule.on = true;
        return SkinBrowserNotificationsModule;
    }());
    YetaWF_Messenger.SkinBrowserNotificationsModule = SkinBrowserNotificationsModule;
})(YetaWF_Messenger || (YetaWF_Messenger = {}));

//# sourceMappingURL=SkinBrowserNotificationsMVC5.js.map
