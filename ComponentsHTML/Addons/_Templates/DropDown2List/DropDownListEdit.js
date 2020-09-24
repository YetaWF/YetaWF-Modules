"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
    //$$$ interface AjaxData {
    //    data: kendo.data.DataSource;
    //    tooltips: string[] | null;
    //}
    var DropDown2ListEditComponent = /** @class */ (function (_super) {
        __extends(DropDown2ListEditComponent, _super);
        function DropDown2ListEditComponent(controlId) {
            var _this = _super.call(this, controlId, DropDown2ListEditComponent.TEMPLATE, DropDown2ListEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: DropDown2ListEditComponent.EVENTCHANGE,
                GetValue: function (control) {
                    return control.value;
                },
                Enable: function (control, enable, clearOnDisable) {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            }, false, function (tag, control) {
            }) || this;
            _this.Popup = null;
            _this.Enabled = true;
            _this.Focused = false;
            _this.Input = $YetaWF.getElement1BySelector(".t_input", [_this.Control]);
            _this.Select = $YetaWF.getElement1BySelector("select", [_this.Control]);
            _this.Container = $YetaWF.getElement1BySelector(".t_container", [_this.Control]);
            $YetaWF.registerEventHandler(_this.Container, "mouseenter", null, function (ev) {
                if (_this.Enabled) {
                    $YetaWF.elementRemoveClass(_this.Container, "k-state-hover");
                    $YetaWF.elementAddClass(_this.Container, "k-state-hover");
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.Container, "mouseleave", null, function (ev) {
                if (_this.Enabled)
                    $YetaWF.elementRemoveClass(_this.Container, "k-state-hover");
                return true;
            });
            $YetaWF.registerEventHandler(_this.Container, "click", null, function (ev) {
                if (_this.Enabled)
                    _this.openPopup();
                return false;
            });
            $YetaWF.registerEventHandler(_this.Control, "focusin", null, function (ev) {
                if (_this.Enabled) {
                    $YetaWF.elementRemoveClass(_this.Container, "k-state-focused");
                    $YetaWF.elementAddClass(_this.Container, "k-state-focused");
                    _this.Focused = true;
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "focusout", null, function (ev) {
                //$$$$ this.closePopup();
                $YetaWF.elementRemoveClass(_this.Container, "k-state-focused");
                _this.Focused = false;
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "keydown", null, function (ev) {
                if (_this.Enabled) {
                    var key = ev.key;
                    if (ev.altKey) {
                        if (key === "ArrowDown" || key === "Down" || key === "ArrowRight" || key === "Right") {
                            _this.openPopup();
                            return false;
                        }
                    }
                    else {
                        if (key === "ArrowDown" || key === "Down" || key === "ArrowRight" || key === "Right") {
                            ++_this.selectedIndex;
                            return false;
                        }
                        else if (key === "ArrowUp" || key === "Up" || key === "ArrowLeft" || key === "Left") {
                            --_this.selectedIndex;
                            return false;
                        }
                        else if (key === "Home") {
                            _this.selectedIndex = 0;
                            return false;
                        }
                        else if (key === "End") {
                            var total = _this.totalItems;
                            _this.selectedIndex = total - 1;
                            return false;
                        }
                        else if (key === "Escape") {
                            _this.closePopup();
                            return false;
                        }
                        else if (key.length === 1) {
                            // find an entry starting with the character pressed
                            var opts = _this.Select.options;
                            var len = opts.length;
                            for (var i = _this.selectedIndex + 1; i < len; ++i) {
                                key = key.toLowerCase();
                                if (opts[i].text.toLowerCase().startsWith(key)) {
                                    _this.selectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                }
                return true;
            });
            return _this;
        }
        Object.defineProperty(DropDown2ListEditComponent.prototype, "value", {
            get: function () {
                return this.Select.value;
            },
            set: function (val) {
                this.Select.value = val;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DropDown2ListEditComponent.prototype, "selectedIndex", {
            get: function () {
                return this.Select.selectedIndex;
            },
            set: function (index) {
                var total = this.totalItems;
                if (index < 0 || index >= total)
                    return;
                this.Select.selectedIndex = index;
                this.Select.options[index].selected = true;
                this.clearSelectedPopupItem();
                this.selectPopupItem();
                this.Input.innerText = this.Select.options[index].text;
                this.sendChangeEvent();
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DropDown2ListEditComponent.prototype, "totalItems", {
            get: function () {
                return this.Select.options.length;
            },
            enumerable: false,
            configurable: true
        });
        // retrieve the tooltip for the nth item (index) in the dropdown list
        DropDown2ListEditComponent.prototype.getToolTip = function (index) {
            var total = this.totalItems;
            if (index < 0 || index >= total)
                return null;
            var opt = this.Select.options[index];
            var tt = $YetaWF.getAttributeCond(opt, YConfigs.Basics.CssTooltip);
            return tt;
        };
        DropDown2ListEditComponent.prototype.clear = function () {
            this.closePopup();
            this.selectedIndex = 0;
        };
        DropDown2ListEditComponent.prototype.enable = function (enabled) {
            this.closePopup();
            $YetaWF.elementEnableToggle(this.Select, enabled);
            $YetaWF.elementEnableToggle(this.Container, enabled);
            $YetaWF.elementRemoveClass(this.Container, "k-state-disabled");
            this.Control.removeAttribute("tabindex");
            if (!enabled) {
                $YetaWF.elementAddClass(this.Container, "k-state-disabled");
                $YetaWF.setAttribute(this.Control, "aria-disabled", "true");
            }
            else {
                $YetaWF.setAttribute(this.Control, "tabindex", "0");
                $YetaWF.setAttribute(this.Control, "aria-disabled", "false");
            }
            this.Enabled = enabled;
        };
        DropDown2ListEditComponent.prototype.sendChangeEvent = function () {
            var event = document.createEvent("Event");
            event.initEvent(DropDown2ListEditComponent.EVENTCHANGE, true, true);
            this.Control.dispatchEvent(event);
            FormsSupport.validateElement(this.Select);
        };
        DropDown2ListEditComponent.prototype.openPopup = function () {
            var _this = this;
            if (this.Popup) {
                this.closePopup();
                return;
            }
            this.Popup =
                $YetaWF.createElement("div", { id: "yDDPopup", "data-owner": this.ControlId, class: "k-animation-container", "aria-hidden": "false" },
                    $YetaWF.createElement("div", { class: "k-list-container k-popup k-group k-reset", "data-role": "popup", "aria-hidden": "false" },
                        $YetaWF.createElement("div", { class: "k-list-scroller", unselectable: "on" },
                            $YetaWF.createElement("ul", { unselectable: "on", class: "k-list k-reset", tabindex: "-1", "aria-hidden": "true", "aria-live": "off", "data-role": "staticlist", role: "listbox" }))));
            var ul = $YetaWF.getElement1BySelector("ul", [this.Popup]);
            var opts = this.Select.options;
            var len = opts.length;
            for (var i = 0; i < len; ++i) {
                ul.append($YetaWF.createElement("li", { tabindex: "-1", role: "option", unselectable: "on", class: "k-item", "data-index": i }, opts[i].innerText));
            }
            DropDown2ListEditComponent.positionPopup(this.Popup);
            document.body.append(this.Popup);
            this.selectPopupItem();
            $YetaWF.registerEventHandler(this.Popup, "click", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                var index = Number($YetaWF.getAttribute(li, "data-index"));
                _this.selectedIndex = index;
                _this.closePopup();
                return false;
            });
            $YetaWF.registerEventHandler(this.Popup, "mouseover", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "k-state-hover");
                $YetaWF.elementAddClass(li, "k-state-hover");
                return true;
            });
            $YetaWF.registerEventHandler(this.Popup, "mouseout", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "k-state-hover");
                return true;
            });
        };
        DropDown2ListEditComponent.prototype.closePopup = function () {
            if (!this.Popup)
                return;
            this.Popup.remove();
            this.Popup = null;
        };
        DropDown2ListEditComponent.positionPopup = function (popup) {
            if (!popup)
                return;
            var ownerId = $YetaWF.getAttribute(popup, "data-owner");
            var dd = $YetaWF.getElementById(ownerId);
            var rect = dd.getBoundingClientRect();
            var top = window.pageYOffset + rect.bottom;
            var left = window.pageXOffset + rect.left;
            var width = rect.width;
            // set left, top, width on #yDDPopup
            popup.style.width = width + "px";
            popup.style.top = top + "px";
            popup.style.left = left + "px";
            // resize based on number of entries wanted
            var scroller = $YetaWF.getElement1BySelector(".k-list-scroller", [popup]);
            scroller.style.height = "200px"; //$$$$
        };
        DropDown2ListEditComponent.prototype.selectPopupItem = function () {
            var index = this.Select.selectedIndex;
            if (index < 0)
                return;
            if (this.Popup) {
                var ul = $YetaWF.getElement1BySelector("ul", [this.Popup]);
                var li = ul.children[index];
                $YetaWF.elementRemoveClasses(li, ["k-state-selected", "k-state-focused"]);
                $YetaWF.elementAddClass(li, "k-state-selected");
                var ariaId = $YetaWF.getAttribute(this.Control, "aria-activedescendant");
                li.id = ariaId;
                $YetaWF.setAttribute(li, "aria-selected", "true");
                if (this.Focused)
                    $YetaWF.elementAddClass(li, "k-state-focused");
                var scroller = $YetaWF.getElement1BySelector(".k-list-scroller", [this.Popup]);
                var rectElem = li.getBoundingClientRect();
                var rectContainer = scroller.getBoundingClientRect();
                if (rectElem.bottom > rectContainer.bottom)
                    li.scrollIntoView(false);
                if (rectElem.top < rectContainer.top)
                    li.scrollIntoView();
            }
        };
        DropDown2ListEditComponent.prototype.clearSelectedPopupItem = function () {
            if (this.Popup) {
                var lis = $YetaWF.getElementsBySelector("ul li.k-state-selected", [this.Popup]);
                for (var _i = 0, lis_1 = lis; _i < lis_1.length; _i++) {
                    var li = lis_1[_i];
                    $YetaWF.elementRemoveClasses(li, ["k-state-selected", "k-state-focused"]);
                    $YetaWF.setAttribute(li, "aria-selected", "false");
                    li.id = "";
                }
            }
            this.Input.innerText = "";
        };
        DropDown2ListEditComponent.prototype.ajaxUpdate = function (data, ajaxUrl, onSuccess, onFailure) {
            //$YetaWF.setLoading(true);
            //var uri = $YetaWF.parseUrl(ajaxUrl);
            //uri.addSearchSimpleObject(data);
            //var request: XMLHttpRequest = new XMLHttpRequest();
            //request.open("POST", ajaxUrl);
            //request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            //request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
            //request.onreadystatechange = (ev: Event): any => {
            //    if (request.readyState === 4 /*DONE*/) {
            //        $YetaWF.setLoading(false);
            //        var retVal = $YetaWF.processAjaxReturn(request.responseText, request.statusText, request, this.Control, undefined, undefined, (data: AjaxData): void => {
            //            // $(this.Control).val(null);
            //            $(this.Control).kendoDropDown2List({
            //                dataTextField: "t",
            //                dataValueField: "v",
            //                dataSource: data.data,
            //                autoWidth: true,
            //                change: (): void => {
            //                    this.sendChangeEvent();
            //                }
            //            });
            //            this.Setup.ToolTips = data.tooltips;
            //            this.KendoDropDown2List = $(this.Control).data("kendoDropDown2List");
            //            if (onSuccess) {
            //                onSuccess(data);
            //            } else {
            //                this.value = "";
            //                this.sendChangeEvent();
            //            }
            //        });
            //        if (!retVal) {
            //            if (onFailure)
            //                onFailure(request.responseText);
            //        }
            //    }
            //};
            //request.send(uri.toFormData());
        };
        DropDown2ListEditComponent.TEMPLATE = "yt_dropdown2list_base";
        DropDown2ListEditComponent.SELECTOR = ".yt_dropdown2list_base.t_edit";
        DropDown2ListEditComponent.EVENTCHANGE = "dropdown2list_change";
        DropDown2ListEditComponent.POPUPID = "yDDPopup";
        return DropDown2ListEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.DropDown2ListEditComponent = DropDown2ListEditComponent;
    // We need to delay initialization until divs become visible so we can calculate the dropdown width
    //$YetaWF.registerActivateDiv((tag: HTMLElement): void => {
    //    var ctls = $YetaWF.getElementsBySelector(DropDown2ListEditComponent.SELECTOR, [tag]);
    //    for (let ctl of ctls) {
    //        var control: DropDown2ListEditComponent = YetaWF.ComponentBaseDataImpl.getControlFromTag(ctl, DropDown2ListEditComponent.SELECTOR);
    //        control.updateWidth();
    //    }
    //});
    //// handle submit/apply
    $YetaWF.registerCustomEventHandlerDocument(DropDown2ListEditComponent.EVENTCHANGE, ".ysubmitonchange " + DropDown2ListEditComponent.SELECTOR, function (ev) {
        $YetaWF.Forms.submitOnChange(ev.target);
        return false;
    });
    $YetaWF.registerCustomEventHandlerDocument(DropDown2ListEditComponent.EVENTCHANGE, ".yapplyonchange " + DropDown2ListEditComponent.SELECTOR, function (ev) {
        $YetaWF.Forms.applyOnChange(ev.target);
        return false;
    });
    $YetaWF.registerCustomEventHandlerDocument(DropDown2ListEditComponent.EVENTCHANGE, ".yreloadonchange " + DropDown2ListEditComponent.SELECTOR, function (ev) {
        $YetaWF.Forms.reloadOnChange(ev.target);
        return false;
    });
    $(window).smartresize(function () {
        var popup = $YetaWF.getElementByIdCond(DropDown2ListEditComponent.POPUPID);
        if (popup)
            DropDown2ListEditComponent.positionPopup(popup);
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
