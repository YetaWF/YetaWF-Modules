/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

namespace YetaWF_Messenger {

    export class SkinActiveUsersModule {

        static readonly PROXY: string = "YetaWF_Messenger_ActiveUsersHub";

        constructor() {

            const connection = new signalR.HubConnectionBuilder()
                .withUrl(`${YConfigs.SignalR.Url}/${SkinActiveUsersModule.PROXY}`)
                .configureLogging(signalR.LogLevel.Information)
                .build();
            connection.start().then((): void => { /*empty*/ });

        }
    }
}
