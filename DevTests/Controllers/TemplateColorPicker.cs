/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateColorPickerModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateColorPickerModule> {

        public TemplateColorPickerModuleController() { }

        [Trim]
        public class Model {

            public enum ControlStatusEnum { Normal, Disabled, }

            [Caption("ColorPicker (Required)"), Description("ColorPicker (Required)")]
            [UIHint("ColorPicker"), AdditionalMetadata("Palette", "basic"), Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string Prop1Req { get; set; }

            [Caption("ColorPicker (Required)"), Description("ColorPicker (Required)")]
            [UIHint("ColorPicker"), AdditionalMetadata("TileSize", 32), AdditionalMetadata("Palette", "websafe"), Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string Prop2Req { get; set; }

            [Caption("ColorPicker (Required)"), Description("ColorPicker (Required)")]
            [UIHint("ColorPicker"), AdditionalMetadata("TileSize", 32), AdditionalMetadata("Palette", ""), AdditionalMetadata("Preview", true), Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string Prop3Req { get; set; }

            [Caption("ColorPicker (Required)"), Description("ColorPicker (Required)")]
            [UIHint("ColorPicker"), AdditionalMetadata("TileSize", 24), AdditionalMetadata("Palette", "#000,#333,#666,#999,#ccc,#fff"), AdditionalMetadata("Columns", 2), Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string Prop4Req { get; set; }

            [Caption("ColorPicker"), Description("ColorPicker")]
            [UIHint("ColorPicker"), Trim]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string Prop1 { get; set; }

            [Caption("ColorPicker (Read/Only)"), Description("ColorPicker (read/only)")]
            [UIHint("ColorPicker"), ReadOnly]
            public string Prop1RO { get; set; }

            [Caption("Control Status"), Description("Defines the processing status of the controls")]
            [UIHint("Enum")]
            public ControlStatusEnum ControlStatus { get; set; }

            public Model() { }
        }

        [AllowGet]
        public ActionResult TemplateColorPicker() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateColorPicker_Partial(Model model) {
            model.Prop1RO = model.Prop1Req;
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
