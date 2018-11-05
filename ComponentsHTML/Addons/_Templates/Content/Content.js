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
    //$$$export interface IPackageVolatiles {
    //    DateTimeFormat: string;
    //}
    var ContentEditComponent = /** @class */ (function (_super) {
        __extends(ContentEditComponent, _super);
        function ContentEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId) || this;
            $YetaWF.addObjectDataById(controlId, _this);
            ClassicEditor
                .create($YetaWF.getElementById(_this.ControlId), {
            //highlight: {
            //    options: [
            //        {
            //            model: 'greenMarker',
            //            class: 'marker-green',
            //            title: 'Green marker',
            //            color: 'var(--ck-highlight-marker-green)',
            //            type: 'marker'
            //        },
            //        {
            //            model: 'redPen',
            //            class: 'pen-red',
            //            title: 'Red pen',
            //            color: 'var(--ck-highlight-pen-red)',
            //            type: 'pen'
            //        }
            //    ]
            //},
            //toolbar: [
            //    'heading', '|', 'bulletedList', 'numberedList', 'highlight', 'undo', 'redo'
            //]
            })
                .then(function (editor) {
                {
                    debugger;
                    //editor.ui.componentFactory.names();
                    console.log(editor);
                }
            })
                .catch(function (error) {
                {
                    console.error(error);
                }
            });
            return _this;
        }
        //get value(): Date {
        //    return new Date(this.Hidden.value);
        //}
        //get valueText(): string {
        //    return this.Hidden.value;
        //}
        //set value(val: Date) {
        //    this.setHidden(val);
        //    this.kendoDateTimePicker.value(val);
        //}
        //public clear(): void {
        //    this.setHiddenText("");
        //    this.kendoDateTimePicker.value("");
        //}
        //public enable(enabled: boolean): void {
        //    this.kendoDateTimePicker.enable(enabled);
        //}
        ContentEditComponent.prototype.destroy = function () {
            //$$$this.kendoDateTimePicker.destroy();
            $YetaWF.removeObjectDataById(this.Control.id);
        };
        ContentEditComponent.getControlFromTag = function (elem) { return _super.getControlBaseFromTag.call(this, elem, ContentEditComponent.SELECTOR); };
        ContentEditComponent.getControlFromSelector = function (selector, tags) { return _super.getControlBaseFromSelector.call(this, selector, ContentEditComponent.SELECTOR, tags); };
        ContentEditComponent.getControlById = function (id) { return _super.getControlBaseById.call(this, id, ContentEditComponent.SELECTOR); };
        ContentEditComponent.SELECTOR = ".yt_content.t_edit";
        return ContentEditComponent;
    }(YetaWF.ComponentBase));
    YetaWF_ComponentsHTML.ContentEditComponent = ContentEditComponent;
    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv(function (tag) {
        YetaWF.ComponentBase.clearDiv(tag, ContentEditComponent.SELECTOR, function (control) {
            control.destroy();
        });
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Content.js.map
