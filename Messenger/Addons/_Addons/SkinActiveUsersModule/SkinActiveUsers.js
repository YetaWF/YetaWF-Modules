"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */
var YetaWF_Messenger;
(function (YetaWF_Messenger) {
    var SkinActiveUsersModule = /** @class */ (function () {
        function SkinActiveUsersModule() {
            var connection = new signalR.HubConnectionBuilder()
                .withUrl("".concat(YConfigs.SignalR.Url, "/").concat(SkinActiveUsersModule.PROXY))
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
