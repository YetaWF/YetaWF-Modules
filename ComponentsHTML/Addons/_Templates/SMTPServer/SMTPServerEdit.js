"use strict";
/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var SMTPServerEdit = /** @class */ (function (_super) {
        __extends(SMTPServerEdit, _super);
        function SMTPServerEdit(controlId /*, setup: SMTPServerEditSetup*/) {
            var _this = _super.call(this, controlId, SMTPServerEdit.TEMPLATE, SMTPServerEdit.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: function (control) {
                    return control.Server.value;
                },
                Enable: function (control, enable, clearOnDisable) {
                    control.enable(enable);
                    if (clearOnDisable)
                        control.clear();
                },
            }) || this;
            _this.Server = $YetaWF.getElement1BySelector("input[name$='.Server']", [_this.Control]);
            _this.Port = YetaWF_ComponentsHTML.IntValueEditComponent.getControlFromSelector("input[name$='.Port']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [_this.Control]);
            _this.Auth = $YetaWF.getElement1BySelector("select[name$='.Authentication']", [_this.Control]);
            _this.Button = $YetaWF.getElement1BySelector(".t_sendtestemail a", [_this.Control]);
            $YetaWF.registerEventHandler(_this.Button, "click", null, function (ev) {
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
                if (port === 0) {
                    port = 25;
                    _this.Port.value = port;
                }
                uri.addSearch("Port", port);
                uri.addSearch("Authentication", _this.Auth.value);
                var userName = $YetaWF.getElement1BySelector("input[name$='.UserName']", [_this.Control]);
                uri.addSearch("UserName", userName.value);
                var password = $YetaWF.getElement1BySelector("input[name$='.Password']", [_this.Control]);
                uri.addSearch("Password", password.value);
                var ssl = $YetaWF.getElement1BySelector("input[name$='.SSL']", [_this.Control]);
                uri.addSearch("SSL", ssl.checked ? "true" : "false");
                _this.Button.href = uri.toUrl();
                return true;
            });
            return _this;
        }
        SMTPServerEdit.prototype.clear = function () {
            this.Server.value = "";
            this.Port.value = 25;
            $YetaWF.getElement1BySelector("input[name$='.UserName']", [this.Control]).value = "";
            $YetaWF.getElement1BySelector("input[name$='.Password']", [this.Control]).value = "";
            $YetaWF.getElement1BySelector("input[name$='.SSL']", [this.Control]).checked = false;
        };
        SMTPServerEdit.prototype.enable = function (enabled) {
            $YetaWF.elementEnableToggle(this.Server, enabled);
        };
        SMTPServerEdit.TEMPLATE = "yt_smtpserver";
        SMTPServerEdit.SELECTOR = ".yt_smtpserver.t_edit";
        return SMTPServerEdit;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.SMTPServerEdit = SMTPServerEdit;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=SMTPServerEdit.js.map
