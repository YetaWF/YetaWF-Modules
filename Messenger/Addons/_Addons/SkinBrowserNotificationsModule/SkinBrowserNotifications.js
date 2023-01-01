"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */
var YetaWF_Messenger;
(function (YetaWF_Messenger) {
    var SkinBrowserNotificationsModule = /** @class */ (function () {
        function SkinBrowserNotificationsModule() {
            var _this = this;
            if (!("Notification" in window)) {
                console.error("No notification support");
                return;
            }
            var connection = new signalR.HubConnectionBuilder()
                .withUrl("".concat(YConfigs.SignalR.Url, "/").concat(SkinBrowserNotificationsModule.PROXY))
                .configureLogging(signalR.LogLevel.Information)
                .build();
            connection.on("Message", function (title, text, icon, timeout, url) {
                _this.handleMessage(title, text, icon, timeout, url);
            });
            connection.start().then(function () { });
        }
        SkinBrowserNotificationsModule.prototype.handleMessage = function (title, text, icon, timeout, url) {
            var _this = this;
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
                        this.showNotification(title, text, icon, timeout, url);
                        break;
                    default:
                        console.error("No permission to show notification");
                }
            }
        };
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
        SkinBrowserNotificationsModule.MODULEGUID = "7f60abc1-07a1-49f1-8381-bd4276977ff0";
        SkinBrowserNotificationsModule.PROXY = "YetaWF_Messenger_BrowserNotificationsHub";
        SkinBrowserNotificationsModule.on = true;
        return SkinBrowserNotificationsModule;
    }());
    YetaWF_Messenger.SkinBrowserNotificationsModule = SkinBrowserNotificationsModule;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, function (ev) {
        var addonGuid = ev.detail.addonGuid;
        var on = ev.detail.on;
        if (addonGuid === SkinBrowserNotificationsModule.MODULEGUID) {
            SkinBrowserNotificationsModule.on = on;
        }
        return true;
    });
})(YetaWF_Messenger || (YetaWF_Messenger = {}));

//# sourceMappingURL=SkinBrowserNotifications.js.map
