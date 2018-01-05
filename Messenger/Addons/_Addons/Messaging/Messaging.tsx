/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

var CKEDITOR: any;

namespace YetaWF_Messenger {

    export class MessagingModule {

        private idForm: string; 
        private idSend: string;
        private idCancel: string;
        private fromUser: string;
        private toUser: string;

        constructor(idForm: string, idSend: string, idCancel: string, fromUser: string, toUser: string) {
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

            var btnSend: HTMLButtonElement = this.getBtnSendTest();
            btnSend.addEventListener("click", (ev: Event) => this.onClickSend(ev), false);

            this.updateButtons();
        }
        private getSelToUser(): HTMLSelectElement {
            return document.querySelector(`#${this.idForm} select[name="ToUserId"]`) as HTMLSelectElement;
        }
        private getSelToUserTest(): HTMLSelectElement {
            var selToUser: HTMLSelectElement = this.getSelToUser();
            if (!selToUser) throw "user selection not found";/*DEBUG*/
            return selToUser;
        }
        private getEditMsg(): HTMLTextAreaElement {
            return document.querySelector(`#${this.idForm} textarea[name="MessageText"]`) as HTMLTextAreaElement;
        }
        private getEditMsgTest(): HTMLTextAreaElement {
            var editMsg: HTMLTextAreaElement = this.getEditMsg();
            if (!editMsg) throw "editor (new message) not found";/*DEBUG*/
            return editMsg;
        }
        private getBtnSend(): HTMLButtonElement {
            return document.querySelector(`#${this.idForm} input[name="Send"]`) as HTMLButtonElement;
        }
        private getBtnSendTest(): HTMLButtonElement {
            var btnSend: HTMLButtonElement = this.getBtnSend();
            if (!btnSend) throw "send button not found";/*DEBUG*/
            return btnSend;
        }
        private onClickSend(event: Event): void {

            var selToUser: HTMLSelectElement = this.getSelToUserTest(); 
            var editMsg: HTMLTextAreaElement = this.getEditMsgTest();
            var btnSend: HTMLButtonElement = this.getBtnSendTest();

            if (selToUser.selectedIndex <= 0) return;
            var user = selToUser.options[selToUser.selectedIndex].text;

            var msg: string = CKEDITOR.instances[editMsg.id].getData();
            if (msg.trim().length <= 0) return;

            var $$: any = $;
            var connection = $$.hubConnection(YConfigs.Basics.SignalRUrl, { useDefaultPath: false });
            var proxy = connection.createHubProxy('YetaWF_Messenger_Messaging');
            connection.start().done(() => this.onConnectionStarted(connection, proxy, user, msg, editMsg.id));
        } 
        private onConnectionStarted(connection: any, proxy: any, user: string, msg: string, editId: string): void {
            proxy.invoke("Send", user, msg);
            CKEDITOR.instances[editId].setData('');
            connection.stop();
        }
        private updateButtons() {
            var selToUser: HTMLSelectElement = this.getSelToUser();
            var editMsg: HTMLTextAreaElement = this.getEditMsg();
            var btnSend: HTMLButtonElement = this.getBtnSend();

            if (!selToUser || !editMsg || !btnSend) return;

            var enabled: boolean = true;
            if (enabled) {
                if (!selToUser || selToUser.selectedIndex <= 0 || selToUser.options[selToUser.selectedIndex].text.trim().length <= 0)
                    enabled = false;
            }
            if (enabled) {
                var msg: string = CKEDITOR.instances[editMsg.id].getData();
                if (msg.trim().length <= 0)
                    enabled = false;
            }
            btnSend.disabled = !enabled;

            setTimeout(() => this.updateButtons(), 100);
        }
    }
}
