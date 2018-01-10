/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

var Y_AttrEscape: any;

namespace YetaWF_Messenger {

    export class MessagesTemplate {

        private divId: string;
        private fromUser: string;
        private toUser: string;

        constructor(divId: string, fromUser: string, toUser: string) {
            this.init(divId, fromUser, toUser);
        }

        /**
         * Initializes the module instance.
         */
        private init(divId: string, fromUser: string, toUser: string): void {

            this.divId = divId;
            this.fromUser = fromUser;
            this.toUser = toUser;

            $(document).on("YetaWF_Messenger_Messaging_Message", (event: any, o: any) => this.handleMessage(o.key, o.from, o.messageText, o.sent));
            $(document).on("YetaWF_Messenger_Messaging_MessageSent", (event: any, o: any) => this.handleMessageSent(o.key, o.to, o.messageText, o.sent));
            $(document).on("YetaWF_Messenger_Messaging_MessageSeen", (event: any, o: any) => this.handleMessageSeen(o.key, o.to));
            $(document).on("YetaWF_Messenger_Messaging_AllMessagesSeen", (event: any, o: any) => this.handleAllMessagesSeen(o.to));
            
            if (this.toUser && this.toUser.length > 0) {
                YetaWF_Messenger.SkinMessagingModule.singleton.allMessagesSeen(this.toUser);
            }
        }
        private handleMessage(key: number, from: string, messageText: string, sent: string): void {
            if (from === this.toUser) {
                var line = `<div class="t_otheruser t_seen" data-key="${key}">` +
                    `<div class="t_sent">${Y_AttrEscape(sent)}</div>` + 
                    `<div class="t_text">${Y_AttrEscape(messageText)}</div>` + 
                    '</div>';
                $(`#${this.divId} .t_messagearea .t_last`).before(line);
                this.scrollMessageArea();
                YetaWF_Messenger.SkinMessagingModule.singleton.messageSeen(key);
            }
        }
        private handleMessageSent(key: number, to: string, messageText: string, sent: string): void {
            if (to === this.toUser) {
                var line = `<div class="t_thisuser t_notseen" data-key="${key}">` +
                    `<div class="t_sent"><img alt="${Y_AttrEscape(YLocs.YetaWF_Messenger.notSeen)}" title="${Y_AttrEscape(YLocs.YetaWF_Messenger.notSeen)}" src="${Y_AttrEscape(YConfigs.YetaWF_Messenger.msgNotSeenIcon)}">${sent}</div>` +
                    `<div class="t_text">${Y_AttrEscape(messageText)}</div>` +
                    '</div>';
                $(`#${this.divId} .t_messagearea .t_last`).before(line);
                this.scrollMessageArea();
            }
        }
        private handleMessageSeen(key: number, to: string): void {
            if (to === this.toUser) {
                this.markAllMessagesSeen(to);
                //this.markMessageSeen(key, to);
            }
        }
        private handleAllMessagesSeen(to: string): void {
            if (to === this.toUser) {
                this.markAllMessagesSeen(to);
            }
        }
        private markMessageSeen(key: number, to: string): void {
            var $msgArea: JQuery<HTMLElement> = $(`#${this.divId}`);
            if ($msgArea.length == 0) throw `Div ${this.divId} not found`;/*DEBUG*/
            $(`div.t_notseen[data-key=${key}] img`, $msgArea).remove();
        }
        private markAllMessagesSeen(to: string): void {
            var $msgArea: JQuery<HTMLElement> = $(`#${this.divId}`);
            if ($msgArea.length == 0) throw `Div ${this.divId} not found`;/*DEBUG*/
            $(`div.t_notseen img`, $msgArea).remove();
        }
        private scrollMessageArea() {
            var out: HTMLElement | null = document.getElementById(this.divId);
            if (!out) throw `Div ${this.divId} not found`;/*DEBUG*/
            out.style.display = '';
            out.scrollTop = out.scrollHeight - out.clientHeight;
            out = document.getElementById(`${this.divId}_none`);
            if (!out) throw `${this.divId}_none not found`;/*DEBUG*/
            out.style.display = 'none';
        }
    }
}
