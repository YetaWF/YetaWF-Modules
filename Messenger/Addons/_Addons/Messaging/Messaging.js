"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */
var CKEDITOR;
var YetaWF_Messenger;
(function (YetaWF_Messenger) {
    var MessagingModule = /** @class */ (function () {
        function MessagingModule(idForm, idSend, idCancel, offlineImage, onlineImage) {
            var _this = this;
            this.idForm = idForm;
            this.idSend = idSend;
            this.idCancel = idCancel;
            var selToUser = this.getSelToUserTest();
            $("#" + this.idForm).on("change", "select[name='ToUserId']", function () { return _this.onUserSelect(); });
            var btnSend = this.getBtnSendTest();
            btnSend.addEventListener("click", function (ev) { return _this.onClickSend(ev); }, false);
            $(document).on("YetaWF_Messenger_Messaging_UserConnect", function (event, o) { return _this.handleUserConnect(o.user); });
            $(document).on("YetaWF_Messenger_Messaging_UserDisconnect", function (event, o) { return _this.handleUserDisconnect(o.user); });
            this.updateButtons();
            this.updateOnlineStatus();
        }
        MessagingModule.prototype.handleUserConnect = function (user) {
            var toUser = this.getSelToUserValue();
            if (toUser && toUser.length > 0 && toUser === user) {
                this.setOnlineStatus(true);
            }
        };
        MessagingModule.prototype.handleUserDisconnect = function (user) {
            var toUser = this.getSelToUserValue();
            if (toUser && toUser.length > 0 && toUser === user) {
                this.setOnlineStatus(false);
            }
        };
        MessagingModule.prototype.getSelToUser = function () {
            return document.querySelector("#" + this.idForm + " select[name=\"ToUserId\"]");
        };
        MessagingModule.prototype.getSelToUserTest = function () {
            var selToUser = this.getSelToUser();
            if (!selToUser)
                throw "user selection not found"; /*DEBUG*/
            return selToUser;
        };
        MessagingModule.prototype.getSelToUserValue = function () {
            var selToUser = this.getSelToUserTest();
            if (selToUser.selectedIndex <= 0)
                return null;
            var user = selToUser.options[selToUser.selectedIndex].text;
            return user;
        };
        MessagingModule.prototype.getEditMsg = function () {
            return document.querySelector("#" + this.idForm + " textarea[name=\"MessageText\"]");
        };
        MessagingModule.prototype.getEditMsgTest = function () {
            var editMsg = this.getEditMsg();
            if (!editMsg)
                throw "editor (new message) not found"; /*DEBUG*/
            return editMsg;
        };
        MessagingModule.prototype.getBtnSend = function () {
            return document.querySelector("#" + this.idForm + " input[name=\"Send\"]");
        };
        MessagingModule.prototype.getBtnSendTest = function () {
            var btnSend = this.getBtnSend();
            if (!btnSend)
                throw "send button not found"; /*DEBUG*/
            return btnSend;
        };
        MessagingModule.prototype.onClickSend = function (event) {
            var editMsg = this.getEditMsgTest();
            var btnSend = this.getBtnSendTest();
            var user = this.getSelToUserValue();
            if (!user)
                return;
            var msg = CKEDITOR.instances[editMsg.id].getData();
            if (msg.trim().length <= 0)
                return;
            YetaWF_Messenger.SkinMessagingModule.singleton.send(user, msg);
            CKEDITOR.instances[editMsg.id].setData("");
        };
        MessagingModule.prototype.onUserSelect = function () {
            this.clearOnlineStatus();
        };
        MessagingModule.prototype.updateButtons = function () {
            var _this = this;
            var selToUser = this.getSelToUser();
            var editMsg = this.getEditMsg();
            var btnSend = this.getBtnSend();
            if (!selToUser || !editMsg || !btnSend)
                return;
            var enabled = true;
            if (enabled) {
                if (!selToUser || selToUser.selectedIndex <= 0 || selToUser.options[selToUser.selectedIndex].text.trim().length <= 0)
                    enabled = false;
            }
            if (enabled) {
                var msg = CKEDITOR.instances[editMsg.id].getData();
                if (msg.trim().length <= 0)
                    enabled = false;
            }
            btnSend.disabled = !enabled;
            setTimeout(function () { return _this.updateButtons(); }, 100);
        };
        MessagingModule.prototype.updateOnlineStatus = function () {
            var _this = this;
            var user = this.getSelToUserValue();
            if (user && user.length > 0) {
                YetaWF_Messenger.SkinMessagingModule.singleton.isUserOnline(user)
                    .done(function (online) { return _this.isUserOnlineDone(online); })
                    .fail(function () { return _this.isUserOnlineDone(false); });
            }
            else {
                this.isUserOnlineDone(false);
            }
        };
        MessagingModule.prototype.isUserOnlineDone = function (online) {
            this.setOnlineStatus(online);
        };
        MessagingModule.prototype.clearOnlineStatus = function () {
            //var divUser: HTMLDivElement = document.querySelector(`#${this.idForm} .yt_yetawf_identity_userid`) as HTMLDivElement;
            var $divId = $("#" + this.idForm + " .yt_yetawf_identity_userid");
            if ($divId.length === 0)
                throw "user Id Div not found"; /*DEBUG*/
            var $parent = $divId.parent();
            $("img.t_status", $parent).remove();
            return $parent;
        };
        MessagingModule.prototype.setOnlineStatus = function (online) {
            var $parent = this.clearOnlineStatus();
            var user = this.getSelToUserValue();
            if (!user)
                return;
            var content;
            if (online) {
                content = "<img class=\"t_status\" alt=\"" + YLocs.YetaWF_Messenger.msgOnlineTT + "\" title=\"" + YLocs.YetaWF_Messenger.msgOnlineTT + "\" src=\"" + YConfigs.YetaWF_Messenger.msgOnlineIcon + "\" >";
            }
            else {
                content = "<img class=\"t_status\" alt=\"" + YLocs.YetaWF_Messenger.msgOfflineTT + "\" title=\"" + YLocs.YetaWF_Messenger.msgOfflineTT + "\" src=\"" + YConfigs.YetaWF_Messenger.msgOfflineIcon + "\" >";
            }
            $parent.append(content);
        };
        return MessagingModule;
    }());
    YetaWF_Messenger.MessagingModule = MessagingModule;
})(YetaWF_Messenger || (YetaWF_Messenger = {}));
