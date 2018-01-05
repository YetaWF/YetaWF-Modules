/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

var Y_Alert: any;

namespace YetaWF_Messenger {

    export class MessagesTemplate {

        private divId: string;
        private fromUser: string;
        private toUser: string;

        private hubProxy: any = null;
        private hubConnection: any = null;

        public static readonly TemplateClass = "yt_messenger_messages";

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

            var $$: any = $;
            this.hubConnection = $$.hubConnection(YConfigs.Basics.SignalRUrl, { useDefaultPath: false });
            var proxy: any = this.hubConnection.createHubProxy('YetaWF_Messenger_Messaging');

            proxy.on('message', (from: string, message: string, sent: string) => this.handleMessage(from, message, sent));
            proxy.on('messageSent', (to: string, message: string, sent: string) => this.handleMessageSent(to, message, sent));
            proxy.on('notifyException', (message: string) => this.handleNotifyException(message));

            this.hubConnection.start().done(() => { this.hubProxy = proxy; });

            YetaWF_Basics.addObjectDataById(MessagesTemplate.TemplateClass, divId, this);
        }
        /**
         * Terminates the module instance (needs addClearDivForObjects and addObjectDataById)
         */
        private term() {
            if (this.hubConnection) {
                this.hubConnection.stop();
                this.hubConnection = null;
            }
        }
        private handleMessage(from: string, messageText: string, sent: string): void {
            if (from === this.toUser) {
                var line = '<div class="t_otheruser t_seen">' +
                    '<div class="t_sent">' + sent + '</div>' +
                    '<div class="t_text">' + messageText + '</div>' +
                    '</div>';
                $(`#${this.divId} .t_messagearea .t_last`).before(line);
                this.scrollMessageArea();
            }
        }
        private handleMessageSent(to: string, messageText: string, sent: string): void {
            if (to === this.toUser) {
                var line = '<div class="t_thisuser t_seen">' +
                    '<div class="t_sent">' + sent + '</div>' +
                    '<div class="t_text">' + messageText + '</div>' +
                    '</div>';
                $(`#${this.divId} .t_messagearea .t_last`).before(line);
                this.scrollMessageArea();
            }
        }
        private scrollMessageArea() {
            var out: HTMLElement | null = document.getElementById(this.divId);
            if (!out) throw `${this.divId} not found`;/*DEBUG*/
            out.style.display = '';
            out.scrollTop = out.scrollHeight - out.clientHeight;
            out = document.getElementById(`${this.divId}_none`);
            if (!out) throw `${this.divId}_none not found`;/*DEBUG*/
            out.style.display = 'none';
        }
        private handleNotifyException(message: string): void {
            Y_Alert(message, "Messages Error");
        }
    }

    // register cleanup for this template class
    YetaWF_Basics.addClearDivForObjects(MessagesTemplate.TemplateClass);
}
