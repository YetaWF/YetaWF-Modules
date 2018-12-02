"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var SMTPServer = /** @class */ (function () {
        function SMTPServer(id) {
            var _this = this;
            this.Control = $YetaWF.getElementById(id);
            this.Server = $YetaWF.getElement1BySelector("input[name$='.Server']", [this.Control]);
            this.Port = $YetaWF.getElement1BySelector("input[name$='.Port']", [this.Control]);
            this.Auth = $YetaWF.getElement1BySelector("select[name$='.Authentication']", [this.Control]);
            this.Button = $YetaWF.getElement1BySelector(".t_sendtestemail a", [this.Control]);
            $YetaWF.registerEventHandler(this.Button, "click", null, function (ev) {
                var uri = new YetaWF.Url();
                uri.parse(_this.Button.href);
                uri.removeSearch("Server");
                uri.removeSearch("Port");
                uri.removeSearch("Authentication");
                uri.removeSearch("UserName");
                uri.removeSearch("Password");
                uri.removeSearch("SSL");
                uri.addSearch("Server", _this.Server.value);
                var port = _this.Port.value;
                if (port.trim() === "") {
                    port = "25";
                    _this.Port.value = port;
                }
                uri.addSearch("Port", port);
                uri.addSearch("Authentication", _this.Auth.value);
                var userName = $YetaWF.getElement1BySelector("input[name$='.UserName']");
                uri.addSearch("UserName", userName.value);
                var password = $YetaWF.getElement1BySelector("input[name$='.Password']");
                uri.addSearch("Password", password.value);
                var ssl = $YetaWF.getElement1BySelector("input[name$='.SSL']");
                uri.addSearch("SSL", ssl.checked ? "true" : "false");
                _this.Button.href = uri.toUrl();
                return true;
            });
        }
        return SMTPServer;
    }());
    YetaWF_ComponentsHTML.SMTPServer = SMTPServer;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=SMTPServer.js.map
