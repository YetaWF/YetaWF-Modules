/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */
var CKEDITOR;
var YetaWF_Messenger;
(function (YetaWF_Messenger) {
    var MessagingModule = /** @class */ (function () {
        function MessagingModule(idForm, idSend, idCancel, fromUser, toUser) {
            var _this = this;
            this.idForm = idForm;
            this.idSend = idSend;
            this.idCancel = idCancel;
            this.fromUser = fromUser;
            this.toUser = toUser;
            //var selToUser: HTMLSelectElement = this.getSelToUserTest();
            //selToUser.addEventListener("select", () => this.updateButtons(), false);
            // CKEditor change event is not available in source only mode so we're using setTimeout
            //var editMsg: HTMLTextAreaElement = this.getEditMsgTest();
            //CKEDITOR.instances[editMsg.id].on("change", () => this.updateButtons());
            var btnSend = this.getBtnSendTest();
            btnSend.addEventListener("click", function (ev) { return _this.onClickSend(ev); }, false);
            this.updateButtons();
        }
        MessagingModule.prototype.getSelToUser = function () {
            return document.querySelector("#" + this.idForm + " select[name=\"ToUserId\"]");
        };
        MessagingModule.prototype.getSelToUserTest = function () {
            var selToUser = this.getSelToUser();
            if (!selToUser)
                throw "user selection not found"; /*DEBUG*/
            return selToUser;
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
            var _this = this;
            var selToUser = this.getSelToUserTest();
            var editMsg = this.getEditMsgTest();
            var btnSend = this.getBtnSendTest();
            if (selToUser.selectedIndex <= 0)
                return;
            var user = selToUser.options[selToUser.selectedIndex].text;
            var msg = CKEDITOR.instances[editMsg.id].getData();
            if (msg.trim().length <= 0)
                return;
            var $$ = $;
            var connection = $$.hubConnection(YConfigs.Basics.SignalRUrl, { useDefaultPath: false });
            var proxy = connection.createHubProxy('YetaWF_Messenger_Messaging');
            connection.start().done(function () { return _this.onConnectionStarted(connection, proxy, user, msg, editMsg.id); });
        };
        MessagingModule.prototype.onConnectionStarted = function (connection, proxy, user, msg, editId) {
            proxy.invoke("Send", user, msg);
            CKEDITOR.instances[editId].setData('');
            connection.stop();
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
        return MessagingModule;
    }());
    YetaWF_Messenger.MessagingModule = MessagingModule;
})(YetaWF_Messenger || (YetaWF_Messenger = {}));

//# sourceMappingURL=Messaging.js.map
