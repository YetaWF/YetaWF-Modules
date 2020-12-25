"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */
var YetaWF_Messenger;
(function (YetaWF_Messenger) {
    var SkinActiveUsersModule = /** @class */ (function () {
        function SkinActiveUsersModule() {
            var connection = new signalR.HubConnectionBuilder()
                .withUrl(YConfigs.SignalR.Url + "/" + SkinActiveUsersModule.PROXY)
                .configureLogging(signalR.LogLevel.Information)
                .build();
            connection.start().then(function () { });
        }
        SkinActiveUsersModule.PROXY = "YetaWF_Messenger_ActiveUsersHub";
        return SkinActiveUsersModule;
    }());
    YetaWF_Messenger.SkinActiveUsersModule = SkinActiveUsersModule;
})(YetaWF_Messenger || (YetaWF_Messenger = {}));

//# sourceMappingURL=SkinActiveUsers.js.map
