/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

// Remove jQuery use

namespace YetaWF_Messenger {

    export interface IPackageLocs {
        msgOnlineTT: string;
        msgOfflineTT: string;
    }
    export interface IPackageConfigs {
        msgOnlineIcon: string;
        msgOfflineIcon: string;
    }

    export class MessagingModule {

        private idForm: string;
        //private idSend: string;
        //private idCancel: string;

        constructor(idForm: string, idSend: string, idCancel: string, offlineImage: string, onlineImage: string) {
            this.idForm = idForm;
            //this.idSend = idSend;
            //this.idCancel = idCancel;

            //var selToUser: HTMLSelectElement = this.getSelToUserTest();
            $(`#${this.idForm}`).on("change", "select[name='ToUserId']", (): void => this.onUserSelect());

            var btnSend: HTMLButtonElement = this.getBtnSendTest();
            btnSend.addEventListener("click", (ev: Event): void => this.onClickSend(ev), false);

            $(document).on("YetaWF_Messenger_Messaging_UserConnect", (event: any, o: any): void => this.handleUserConnect(o.user));
            $(document).on("YetaWF_Messenger_Messaging_UserDisconnect", (event: any, o: any): void => this.handleUserDisconnect(o.user));

            this.updateButtons();
            this.updateOnlineStatus();
        }
        private handleUserConnect(user: string): void {
            var toUser: string | null = this.getSelToUserValue();
            if (toUser && toUser.length > 0 && toUser === user) {
                this.setOnlineStatus(true);
            }
        }
        private handleUserDisconnect(user: string): void {
            var toUser: string | null = this.getSelToUserValue();
            if (toUser && toUser.length > 0 && toUser === user) {
                this.setOnlineStatus(false);
            }
        }
        private getSelToUser(): HTMLSelectElement {
            return document.querySelector(`#${this.idForm} select[name="ToUserId"]`) as HTMLSelectElement;
        }
        private getSelToUserTest(): HTMLSelectElement {
            var selToUser: HTMLSelectElement = this.getSelToUser();
            if (!selToUser) throw "user selection not found";/*DEBUG*/
            return selToUser;
        }
        private getSelToUserValue(): string | null {
            var selToUser: HTMLSelectElement = this.getSelToUserTest();
            if (selToUser.selectedIndex <= 0) return null;
            var user: string = selToUser.options[selToUser.selectedIndex].text;
            return user;
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

            var editMsg: HTMLTextAreaElement = this.getEditMsgTest();
            //var btnSend: HTMLButtonElement = this.getBtnSendTest();
            var user: string | null = this.getSelToUserValue();
            if (!user) return;

            var msg: string = CKEDITOR.instances[editMsg.id].getData();
            if (msg.trim().length <= 0) return;

            YetaWF_Messenger.SkinMessagingModule.singleton.send(user, msg);

            CKEDITOR.instances[editMsg.id].setData("");
        }
        private onUserSelect(): void {
            this.clearOnlineStatus();
        }
        private updateButtons(): void {
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

            setTimeout((): void => this.updateButtons(), 100);
        }
        private updateOnlineStatus(): void {
            var user: string | null = this.getSelToUserValue();
            if (user && user.length > 0) {
                YetaWF_Messenger.SkinMessagingModule.singleton.isUserOnline(user)
                    .done((online:boolean):void => this.isUserOnlineDone(online))
                    .fail(():void => this.isUserOnlineDone(false));
            } else {
                this.isUserOnlineDone(false);
            }
        }
        private isUserOnlineDone(online: boolean): void {
            this.setOnlineStatus(online);
        }
        private clearOnlineStatus(): JQuery<HTMLElement>  {
            //var divUser: HTMLDivElement = document.querySelector(`#${this.idForm} .yt_yetawf_identity_userid`) as HTMLDivElement;
            var $divId: JQuery<HTMLElement> = $(`#${this.idForm} .yt_yetawf_identity_userid`);
            if ($divId.length === 0) throw "user Id Div not found";/*DEBUG*/
            var $parent: JQuery<HTMLElement> = $divId.parent();
            $("img.t_status", $parent).remove();
            return $parent;

        }
        private setOnlineStatus(online: boolean): void {
            var $parent: JQuery<HTMLElement> = this.clearOnlineStatus();

            var user: string | null = this.getSelToUserValue();
            if (!user) return;

            var content: string;
            if (online) {
                content = `<img class="t_status" alt="${YLocs.YetaWF_Messenger.msgOnlineTT}" title="${YLocs.YetaWF_Messenger.msgOnlineTT}" src="${YConfigs.YetaWF_Messenger.msgOnlineIcon}" >`;
            } else {
                content = `<img class="t_status" alt="${YLocs.YetaWF_Messenger.msgOfflineTT}" title="${YLocs.YetaWF_Messenger.msgOfflineTT}" src="${YConfigs.YetaWF_Messenger.msgOfflineIcon}" >`;
            }
            $parent.append(content);
        }
    }
}
