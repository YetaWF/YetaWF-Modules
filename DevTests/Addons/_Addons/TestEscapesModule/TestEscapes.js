"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */
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
var YetaWF_DevTests;
(function (YetaWF_DevTests) {
    var TestEscapesModule = /** @class */ (function (_super) {
        __extends(TestEscapesModule, _super);
        function TestEscapesModule(id) {
            var _this = _super.call(this, id, TestEscapesModule.SELECTOR, null) || this;
            $YetaWF.registerEventHandler(_this.Module, "click", "input[name='message']", function (ev) {
                $YetaWF.message("TEST <A> &amp; & @ {0} TEST");
                return true;
            });
            $YetaWF.registerEventHandler(_this.Module, "click", "input[name='error']", function (ev) {
                $YetaWF.error("TEST <A> &amp; & @ {{0}} TEST");
                return true;
            });
            $YetaWF.registerEventHandler(_this.Module, "click", "input[name='alert']", function (ev) {
                $YetaWF.alert("TEST <A> &amp; & @ {{0}} TEST(+nl)(+nl)TEST <A> &amp; & @ {{0}} TEST", "TITLE <A> &amp; & @ {{0}} TEST");
                return true;
            });
            $YetaWF.registerEventHandler(_this.Module, "click", "input[name='confirm']", function (ev) {
                $YetaWF.confirm("TEST <A> &amp; & @ {{0}} TEST");
                return true;
            });
            $YetaWF.registerEventHandler(_this.Module, "click", "input[name='alertYesNo']", function (ev) {
                $YetaWF.alertYesNo("TEST <A> &amp; & @ {{0}} TEST TEST <A> &amp; & @ {{0}} TEST TEST <A> &amp;(+nl)(+nl)(+nl)& @ {{0}} TEST TEST <A> &amp; & @ {{0}} TEST TEST <A> &amp; & @ {{0}} TEST TEST <A> &amp; & @ {{0}} TEST TEST <A> &amp; & @ {{0}} TEST TEST <A> &amp; & @ {{0}} TEST TEST <A> &amp; & @ {{0}} TEST TEST <A> &amp; & @ {{0}} TEST ", "TITLE <A> &amp; & @ {{0}} TEST", function () { $YetaWF.message("Yes"); }, function () { $YetaWF.message("No"); }, { encoded: false });
                return true;
            });
            $YetaWF.registerEventHandler(_this.Module, "click", "input[name='pleaseWait']", function (ev) {
                $YetaWF.pleaseWait("Reload page to continue\n\nTEST <A> &amp; & @ {{0}} TEST", "TITLE <A> &amp; & @ {{0}} TEST");
                return true;
            });
            $YetaWF.registerEventHandler(_this.Module, "click", "input[name='jserror']", function (ev) {
                // generate a javascript error (use eval to prevent build errors)
                var s = "aaa.bbb.ccc = 10;";
                // tslint:disable-next-line:no-eval
                eval(s);
                return true;
            });
            return _this;
        }
        TestEscapesModule.SELECTOR = ".YetaWF_DevTests_TestEscapes";
        return TestEscapesModule;
    }(YetaWF.ModuleBaseNoDataImpl));
    YetaWF_DevTests.TestEscapesModule = TestEscapesModule;
})(YetaWF_DevTests || (YetaWF_DevTests = {}));

//# sourceMappingURL=TestEscapes.js.map
