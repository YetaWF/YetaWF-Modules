/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */
var Y_Alert;
var YetaWF_Messenger;
(function (YetaWF_Messenger) {
    var SkinMessagingModule = /** @class */ (function () {
        function SkinMessagingModule() {
            this.init();
        }
        /**
         * Initializes the module instance.
         */
        SkinMessagingModule.prototype.init = function () {
            var _this = this;
            var $$ = $;
            this.connection = $$.hubConnection(YConfigs.Basics.SignalRUrl, { useDefaultPath: false });
            this.tempProxy = this.connection.createHubProxy("YetaWF_Messenger_Messaging");
            this.tempProxy.on("dummy", function () { });
            this.connection.start().done(function () { return _this.onConnectionStarted(); });
        };
        SkinMessagingModule.prototype.onConnectionStarted = function () {
            var _this = this;
            this.hubProxy = this.tempProxy;
            this.hubProxy.on("message", function (key, from, message, sent) { return _this.handleMessage(key, from, message, sent); });
            this.hubProxy.on("messageSent", function (key, to, message, sent) { return _this.handleMessageSent(key, to, message, sent); });
            this.hubProxy.on("messageSeen", function (key, to) { return _this.handleMessageSeen(key, to); });
            this.hubProxy.on("allMessagesSeen", function (to) { return _this.handleAllMessagesSeen(to); });
            this.hubProxy.on("notifyException", function (message) { return _this.handleNotifyException(message); });
            this.hubProxy.on("userConnect", function (user) { return _this.handleUserConnect(user); });
            this.hubProxy.on("userDisconnect", function (user) { return _this.handleUserDisconnect(user); });
        };
        // Incoming message handlers
        SkinMessagingModule.prototype.handleMessage = function (key, from, messageText, sent) {
            $(document).trigger("YetaWF_Messenger_Messaging_Message", {
                key: key,
                from: from,
                messageText: messageText,
                sent: sent,
            });
        };
        SkinMessagingModule.prototype.handleMessageSent = function (key, to, messageText, sent) {
            $(document).trigger("YetaWF_Messenger_Messaging_MessageSent", {
                key: key,
                to: to,
                messageText: messageText,
                sent: sent,
            });
        };
        SkinMessagingModule.prototype.handleMessageSeen = function (key, to) {
            $(document).trigger("YetaWF_Messenger_Messaging_MessageSeen", {
                key: key,
                to: to,
            });
        };
        SkinMessagingModule.prototype.handleAllMessagesSeen = function (to) {
            $(document).trigger("YetaWF_Messenger_Messaging_AllMessagesSeen", {
                to: to,
            });
        };
        SkinMessagingModule.prototype.handleNotifyException = function (message) {
            Y_Alert(message, "Messages Error");
        };
        SkinMessagingModule.prototype.handleUserConnect = function (user) {
            $(document).trigger("YetaWF_Messenger_Messaging_UserConnect", {
                user: user,
            });
        };
        SkinMessagingModule.prototype.handleUserDisconnect = function (user) {
            $(document).trigger("YetaWF_Messenger_Messaging_UserDisconnect", {
                user: user,
            });
        };
        // outgoing commands
        SkinMessagingModule.prototype.send = function (to, messageText) {
            return this.hubProxy.invoke("Send", to, messageText);
        };
        SkinMessagingModule.prototype.getOnlineUsers = function () {
            return this.hubProxy.invoke("GetOnlineUsers");
        };
        SkinMessagingModule.prototype.isUserOnline = function (user) {
            return this.hubProxy.invoke("IsUserOnline", user);
        };
        SkinMessagingModule.prototype.messageSeen = function (key) {
            return this.hubProxy.invoke("MessageSeen", key);
        };
        SkinMessagingModule.prototype.allMessagesSeen = function (fromUser) {
            return this.hubProxy.invoke("AllMessagesSeen", fromUser);
        };
        return SkinMessagingModule;
    }());
    YetaWF_Messenger.SkinMessagingModule = SkinMessagingModule;
})(YetaWF_Messenger || (YetaWF_Messenger = {}));
YetaWF_Messenger.SkinMessagingModule.singleton = new YetaWF_Messenger.SkinMessagingModule();

//# sourceMappingURL=SkinMessaging.js.map
