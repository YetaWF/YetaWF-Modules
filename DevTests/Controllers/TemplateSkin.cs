/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Serializers;
using YetaWF.Core.Skins;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateSkinModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateSkinModule> {

        public TemplateSkinModuleController() { }

        [Trim]
        public class Model {

            public enum ControlStatusEnum { Normal, Disabled, }

            [Caption("Page Skin (Required)"), Description("Page Skin (Required)")]
            [UIHint("PageSkin"), Trim]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public SkinDefinition PageSkinReq { get; set; }

            [Caption("Popup Skin (Required)"), Description("Popup Skin (Required)")]
            [UIHint("PopupSkin"), Trim]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public SkinDefinition PopupSkinReq { get; set; }

            [Caption("Page Skin"), Description("Page Skin (Required)")]
            [UIHint("PageSkin"), Trim]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public SkinDefinition PageSkin { get; set; }

            [Caption("Popup Skin"), Description("Popup Skin (Required)")]
            [UIHint("PopupSkin"), Trim]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public SkinDefinition PopupSkin { get; set; }

            [Caption("Module Skins"), Description("Module Skins (Required)")]
            [UIHint("ModuleSkins"), Trim]
            public SerializableList<SkinDefinition> ModuleSkins { get; set; }

            [Caption("Page Skin (Read/Only)"), Description("Page Skin (Read/Only)")]
            [UIHint("PageSkin"), ReadOnly]
            public SkinDefinition PageSkinRO { get; set; }

            [Caption("Popup Skin (Read/Only)"), Description("Popup Skin (Read/Only)")]
            [UIHint("PopupSkin"), ReadOnly]
            public SkinDefinition PopupSkinRO { get; set; }

            [Caption("Module Skins (Read/Only)"), Description("Module Skins (Read/Only)")]
            [UIHint("ModuleSkins"), ReadOnly]
            public SerializableList<SkinDefinition> ModuleSkinsRO { get; set; }

            [Caption("Control Status"), Description("Defines the processing status of the controls")]
            [UIHint("Enum")]
            public ControlStatusEnum ControlStatus { get; set; }

            public Model() {
                PageSkinReq = new SkinDefinition();
                PageSkin = new SkinDefinition();
                PageSkinRO = new SkinDefinition();
                PopupSkinReq = new SkinDefinition();
                PopupSkin = new SkinDefinition();
                PopupSkinRO = new SkinDefinition();
                ModuleSkins = new SerializableList<SkinDefinition>();
                ModuleSkinsRO = new SerializableList<SkinDefinition>();
            }
        }

        [AllowGet]
        public ActionResult TemplateSkin() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateSkin_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
