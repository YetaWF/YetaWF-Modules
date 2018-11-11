"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    //interface Setup {
    //}
    var PageSelectionEditComponent = /** @class */ (function (_super) {
        __extends(PageSelectionEditComponent, _super);
        function PageSelectionEditComponent(controlId /*, setup: Setup*/) {
            var _this = _super.call(this, controlId) || this;
            //this.Setup = setup;
            _this.SelectPage = YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromSelector("select", [_this.Control]);
            _this.DivLink = $YetaWF.getElement1BySelector(".t_link", [_this.Control]);
            _this.APage = $YetaWF.getElement1BySelector("a", [_this.DivLink]);
            _this.SelectPage.Control.addEventListener("dropdownlist_change", function (evt) {
                _this.updatePage(_this.SelectPage.value, "");
            });
            _this.updatePage(_this.SelectPage.value, "");
            return _this;
        }
        PageSelectionEditComponent.prototype.updatePage = function (pageGuid, desc) {
            if (pageGuid === undefined || pageGuid === null || pageGuid === "" || pageGuid === "00000000-0000-0000-0000-000000000000") {
                desc = "";
                this.DivLink.style.display = "none";
                //$desc.hide();
            }
            else {
                this.DivLink.style.display = "inline";
                //$desc.show();
            }
            this.APage.href = "/!Page/" + pageGuid; // Globals.PageUrl
            //$desc.text(desc);
        };
        PageSelectionEditComponent.SELECTOR = ".yt_pageselection.t_edit";
        return PageSelectionEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.PageSelectionEditComponent = PageSelectionEditComponent;
    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv(function (tag) {
        PageSelectionEditComponent.clearDiv(tag, PageSelectionEditComponent.SELECTOR);
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=PageSelection.js.map
