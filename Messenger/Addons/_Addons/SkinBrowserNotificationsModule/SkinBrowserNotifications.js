"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */
var YetaWF_Messenger;
(function (YetaWF_Messenger) {
    var SkinBrowserNotificationsModule = /** @class */ (function () {
        function SkinBrowserNotificationsModule() {
            var _this = this;
            if (!("Notification" in window)) {
                console.error("No notification support");
                return;
            }
            if (YConfigs.SignalR.Version === "MVC5") {
                var $$ = $;
                var connection = $$.hubConnection(YConfigs.SignalR.Url, { useDefaultPath: false });
                var hubProxy = connection.createHubProxy(SkinBrowserNotificationsModule.PROXY);
                hubProxy.on("Message", function (title, text, icon, timeout, url) {
                    _this.handleMessage(title, text, icon, timeout, url);
                });
                connection.start().done(function () { });
            }
            else {
                var connection_1 = new signalR.HubConnectionBuilder()
                    .withUrl(YConfigs.SignalR.Url + "/" + SkinBrowserNotificationsModule.PROXY)
                    .configureLogging(signalR.LogLevel.Information)
                    .build();
                connection_1.on("Message", function (title, text, icon, timeout, url) {
                    _this.handleMessage(title, text, icon, timeout, url);
                });
                connection_1.start().then(function () { });
            }
            $YetaWF.registerContentChange(function (addonGuid, on) {
                if (addonGuid === SkinBrowserNotificationsModule.MODULEGUID) {
                    SkinBrowserNotificationsModule.on = on;
                }
            });
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
        SkinBrowserNotificationsModule.MODULEGUID = "7F60ABC1-07A1-49f1-8381-BD4276977FF0";
        SkinBrowserNotificationsModule.PROXY = "YetaWF_Messenger_BrowserNotificationsHub";
        SkinBrowserNotificationsModule.on = true;
        return SkinBrowserNotificationsModule;
    }());
    YetaWF_Messenger.SkinBrowserNotificationsModule = SkinBrowserNotificationsModule;
})(YetaWF_Messenger || (YetaWF_Messenger = {}));

//# sourceMappingURL=SkinBrowserNotifications.js.map
