/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

var Y_Alert: any;

namespace YetaWF_Messenger {

    export class SkinMessagingModule {

        public connection: any;
        public hubProxy: any;

        public static singleton: SkinMessagingModule;
         
        private tempProxy: any; 
         
        constructor() {
            this.init();
        }
        /**
         * Initializes the module instance.
         */ 
        private init(): void {
            var $$: any = $;
            this.connection = $$.hubConnection(YConfigs.Basics.SignalRUrl, { useDefaultPath: false });
            this.tempProxy = this.connection.createHubProxy('YetaWF_Messenger_Messaging');
            this.tempProxy.on('dummy', function () { });

            this.connection.start().done(() => this.onConnectionStarted());
        }

        private onConnectionStarted() : void {
            this.hubProxy = this.tempProxy;

            this.hubProxy.on('message', (key: number, from: string, message: string, sent: string) => this.handleMessage(key, from, message, sent));
            this.hubProxy.on('messageSent', (key: number, to: string, message: string, sent: string) => this.handleMessageSent(key, to, message, sent));
            this.hubProxy.on('messageSeen', (key: number, to: string) => this.handleMessageSeen(key, to));
            this.hubProxy.on('allMessagesSeen', (to: string) => this.handleAllMessagesSeen(to));            
            this.hubProxy.on('notifyException', (message: string) => this.handleNotifyException(message));
            this.hubProxy.on('userConnect', (user: string) => this.handleUserConnect(user));
            this.hubProxy.on('userDisconnect', (user: string) => this.handleUserDisconnect(user));
        }

        // Incoming message handlers

        private handleMessage(key: number, from: string, messageText: string, sent: string): void {
            $(document).trigger('YetaWF_Messenger_Messaging_Message', {
                key: key,
                from: from,
                messageText: messageText,
                sent: sent,
            });
        }
        private handleMessageSent(key: number, to: string, messageText: string, sent: string): void {
            $(document).trigger('YetaWF_Messenger_Messaging_MessageSent', {
                key: key,
                to: to,
                messageText: messageText,
                sent: sent,
            });
        }
        private handleMessageSeen(key: number, to: string): void {
            $(document).trigger('YetaWF_Messenger_Messaging_MessageSeen', {
                key: key,
                to: to,
            });
        }
        private handleAllMessagesSeen(to: string): void {
            $(document).trigger('YetaWF_Messenger_Messaging_AllMessagesSeen', {
                to: to,
            });
        }        
        private handleNotifyException(message: string): void {
            Y_Alert(message, "Messages Error");
        }
        private handleUserConnect(user: string): void {
            $(document).trigger('YetaWF_Messenger_Messaging_UserConnect', {
                user: user,
            });
        }
        private handleUserDisconnect(user: string): void {
            $(document).trigger('YetaWF_Messenger_Messaging_UserDisconnect', {
                user: user,
            });
        }

        // outgoing commands

        public send(to: string, messageText: string): JQuery.Promise<any> {
            return this.hubProxy.invoke("Send", to, messageText);
        }
        public getOnlineUsers(): JQuery.Promise<any> {
            return this.hubProxy.invoke("GetOnlineUsers");
        }
        public isUserOnline(user: string): JQuery.Promise<any> {
            return this.hubProxy.invoke("IsUserOnline", user);
        }
        public messageSeen(key: number): JQuery.Promise<any> {
            return this.hubProxy.invoke("MessageSeen", key);
        }
        public allMessagesSeen(fromUser: string): JQuery.Promise<any> {
            return this.hubProxy.invoke("AllMessagesSeen", fromUser);
        }
    }
}

YetaWF_Messenger.SkinMessagingModule.singleton = new YetaWF_Messenger.SkinMessagingModule()

