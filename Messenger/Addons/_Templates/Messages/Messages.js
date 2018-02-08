/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */
var Y_AttrEscape;
var YetaWF_Messenger;
(function (YetaWF_Messenger) {
    var MessagesTemplate = /** @class */ (function () {
        function MessagesTemplate(divId, fromUser, toUser) {
            this.divId = "";
            this.fromUser = "";
            this.toUser = "";
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
            $(document).on("YetaWF_Messenger_Messaging_Message", function (event, o) { return _this.handleMessage(o.key, o.from, o.messageText, o.sent); });
            $(document).on("YetaWF_Messenger_Messaging_MessageSent", function (event, o) { return _this.handleMessageSent(o.key, o.to, o.messageText, o.sent); });
            $(document).on("YetaWF_Messenger_Messaging_MessageSeen", function (event, o) { return _this.handleMessageSeen(o.key, o.to); });
            $(document).on("YetaWF_Messenger_Messaging_AllMessagesSeen", function (event, o) { return _this.handleAllMessagesSeen(o.to); });
            if (this.toUser && this.toUser.length > 0) {
                YetaWF_Messenger.SkinMessagingModule.singleton.allMessagesSeen(this.toUser);
            }
        };
        MessagesTemplate.prototype.handleMessage = function (key, from, messageText, sent) {
            if (from === this.toUser) {
                var line = "<div class=\"t_otheruser t_seen\" data-key=\"" + key + "\">" +
                    ("<div class=\"t_sent\">" + Y_AttrEscape(sent) + "</div>") +
                    ("<div class=\"t_text\">" + Y_AttrEscape(messageText) + "</div>") +
                    "</div>";
                $("#" + this.divId + " .t_messagearea .t_last").before(line);
                this.scrollMessageArea();
                YetaWF_Messenger.SkinMessagingModule.singleton.messageSeen(key);
            }
        };
        MessagesTemplate.prototype.handleMessageSent = function (key, to, messageText, sent) {
            if (to === this.toUser) {
                var line = "<div class=\"t_thisuser t_notseen\" data-key=\"" + key + "\">" +
                    ("<div class=\"t_sent\"><img alt=\"" + Y_AttrEscape(YLocs.YetaWF_Messenger.notSeen) + "\" title=\"" + Y_AttrEscape(YLocs.YetaWF_Messenger.notSeen) + "\" src=\"" + Y_AttrEscape(YConfigs.YetaWF_Messenger.msgNotSeenIcon) + "\">" + sent + "</div>") +
                    ("<div class=\"t_text\">" + Y_AttrEscape(messageText) + "</div>") +
                    "</div>";
                $("#" + this.divId + " .t_messagearea .t_last").before(line);
                this.scrollMessageArea();
            }
        };
        MessagesTemplate.prototype.handleMessageSeen = function (key, to) {
            if (to === this.toUser) {
                this.markAllMessagesSeen(to);
                //this.markMessageSeen(key, to);
            }
        };
        MessagesTemplate.prototype.handleAllMessagesSeen = function (to) {
            if (to === this.toUser) {
                this.markAllMessagesSeen(to);
            }
        };
        MessagesTemplate.prototype.markMessageSeen = function (key, to) {
            var $msgArea = $("#" + this.divId);
            if ($msgArea.length === 0)
                throw "Div " + this.divId + " not found"; /*DEBUG*/
            $("div.t_notseen[data-key=" + key + "] img", $msgArea).remove();
        };
        MessagesTemplate.prototype.markAllMessagesSeen = function (to) {
            var $msgArea = $("#" + this.divId);
            if ($msgArea.length === 0)
                throw "Div " + this.divId + " not found"; /*DEBUG*/
            $("div.t_notseen img", $msgArea).remove();
        };
        MessagesTemplate.prototype.scrollMessageArea = function () {
            var out = document.getElementById(this.divId);
            if (!out)
                throw "Div " + this.divId + " not found"; /*DEBUG*/
            out.style.display = "";
            out.scrollTop = out.scrollHeight - out.clientHeight;
            out = document.getElementById(this.divId + "_none");
            if (!out)
                throw this.divId + "_none not found"; /*DEBUG*/
            out.style.display = "none";
        };
        return MessagesTemplate;
    }());
    YetaWF_Messenger.MessagesTemplate = MessagesTemplate;
})(YetaWF_Messenger || (YetaWF_Messenger = {}));

//# sourceMappingURL=Messages.js.map
