/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */
var Y_Alert;
var YetaWF_Messenger;
(function (YetaWF_Messenger) {
    var MessagesTemplate = /** @class */ (function () {
        function MessagesTemplate(divId, fromUser, toUser) {
            this.hubProxy = null;
            this.hubConnection = null;
            this.init(divId, fromUser, toUser);
        }
        /**
         * Initializes the module instance.
         */
        MessagesTemplate.prototype.init = function (divId, fromUser, toUser) {
            var _this = this;
            this.divId = divId;
            this.fromUser = fromUser;
            this.toUser = toUser;
            var $$ = $;
            this.hubConnection = $$.hubConnection(YConfigs.Basics.SignalRUrl, { useDefaultPath: false });
            var proxy = this.hubConnection.createHubProxy('YetaWF_Messenger_Messaging');
            proxy.on('message', function (from, message, sent) { return _this.handleMessage(from, message, sent); });
            proxy.on('messageSent', function (to, message, sent) { return _this.handleMessageSent(to, message, sent); });
            proxy.on('notifyException', function (message) { return _this.handleNotifyException(message); });
            this.hubConnection.start().done(function () { _this.hubProxy = proxy; });
            YetaWF_Basics.addObjectDataById(MessagesTemplate.TemplateClass, divId, this);
        };
        /**
         * Terminates the module instance (needs addClearDivForObjects and addObjectDataById)
         */
        MessagesTemplate.prototype.term = function () {
            if (this.hubConnection) {
                this.hubConnection.stop();
                this.hubConnection = null;
            }
        };
        MessagesTemplate.prototype.handleMessage = function (from, messageText, sent) {
            if (from === this.toUser) {
                var line = '<div class="t_otheruser t_seen">' +
                    '<div class="t_sent">' + sent + '</div>' +
                    '<div class="t_text">' + messageText + '</div>' +
                    '</div>';
                $("#" + this.divId + " .t_messagearea .t_last").before(line);
                this.scrollMessageArea();
            }
        };
        MessagesTemplate.prototype.handleMessageSent = function (to, messageText, sent) {
            if (to === this.toUser) {
                var line = '<div class="t_thisuser t_seen">' +
                    '<div class="t_sent">' + sent + '</div>' +
                    '<div class="t_text">' + messageText + '</div>' +
                    '</div>';
                $("#" + this.divId + " .t_messagearea .t_last").before(line);
                this.scrollMessageArea();
            }
        };
        MessagesTemplate.prototype.scrollMessageArea = function () {
            var out = document.getElementById(this.divId);
            if (!out)
                throw this.divId + " not found"; /*DEBUG*/
            out.style.display = '';
            out.scrollTop = out.scrollHeight - out.clientHeight;
            out = document.getElementById(this.divId + "_none");
            if (!out)
                throw this.divId + "_none not found"; /*DEBUG*/
            out.style.display = 'none';
        };
        MessagesTemplate.prototype.handleNotifyException = function (message) {
            Y_Alert(message, "Messages Error");
        };
        MessagesTemplate.TemplateClass = "yt_messenger_messages";
        return MessagesTemplate;
    }());
    YetaWF_Messenger.MessagesTemplate = MessagesTemplate;
    // register cleanup for this template class
    YetaWF_Basics.addClearDivForObjects(MessagesTemplate.TemplateClass);
})(YetaWF_Messenger || (YetaWF_Messenger = {}));

//# sourceMappingURL=Messages.js.map
