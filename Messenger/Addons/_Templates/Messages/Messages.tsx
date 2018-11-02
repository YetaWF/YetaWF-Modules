/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

namespace YetaWF_Messenger {

    export interface IPackageLocs {
        notSeen: string;
    }
    export interface IPackageConfigs {
        msgNotSeenIcon: string;
    }

    export class MessagesTemplate {

        private divId: string = "";
        //private fromUser: string = "";
        private toUser: string = "";

        constructor(divId: string, fromUser: string, toUser: string) {
            this.init(divId, fromUser, toUser);
        }

        /**
         * Initializes the module instance.
         */
        private init(divId: string, fromUser: string, toUser: string): void {

            this.divId = divId;
            //this.fromUser = fromUser;
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
                var line: string = `<div class="t_otheruser t_seen" data-key="${key}">` +
                    `<div class="t_sent">${$YetaWF.htmlAttrEscape(sent)}</div>` +
                    `<div class="t_text">${$YetaWF.htmlAttrEscape(messageText)}</div>` +
                    "</div>";
                $(`#${this.divId} .t_messagearea .t_last`).before(line);
                this.scrollMessageArea();
                YetaWF_Messenger.SkinMessagingModule.singleton.messageSeen(key);
            }
        }
        private handleMessageSent(key: number, to: string, messageText: string, sent: string): void {
            if (to === this.toUser) {
                var line: string = `<div class="t_thisuser t_notseen" data-key="${key}">` +
                    `<div class="t_sent"><img alt="${$YetaWF.htmlAttrEscape(YLocs.YetaWF_Messenger.notSeen)}" title="${$YetaWF.htmlAttrEscape(YLocs.YetaWF_Messenger.notSeen)}" src="${$YetaWF.htmlAttrEscape(YConfigs.YetaWF_Messenger.msgNotSeenIcon)}">${sent}</div>` +
                    `<div class="t_text">${$YetaWF.htmlAttrEscape(messageText)}</div>` +
                    "</div>";
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
        //private markMessageSeen(key: number, to: string): void {
        //    var $msgArea: JQuery<HTMLElement> = $(`#${this.divId}`);
        //    if ($msgArea.length === 0) throw `Div ${this.divId} not found`;/*DEBUG*/
        //    $(`div.t_notseen[data-key=${key}] img`, $msgArea).remove();
        //}
        private markAllMessagesSeen(to: string): void {
            var $msgArea: JQuery<HTMLElement> = $(`#${this.divId}`);
            if ($msgArea.length === 0) throw `Div ${this.divId} not found`;/*DEBUG*/
            $(`div.t_notseen img`, $msgArea).remove();
        }
        private scrollMessageArea(): void {
            var out: HTMLElement | null = document.getElementById(this.divId);
            if (!out) throw `Div ${this.divId} not found`;/*DEBUG*/
            out.style.display = "";
            out.scrollTop = out.scrollHeight - out.clientHeight;
            out = document.getElementById(`${this.divId}_none`);
            if (!out) throw `${this.divId}_none not found`;/*DEBUG*/
            out.style.display = "none";
        }
    }
}
