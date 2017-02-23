/* Copyright � 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Serializers;
using YetaWF.Core.Skins;
#if MVC6
using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateSkinModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateSkinModule> {

        public TemplateSkinModuleController() { }

        [Trim]
        public class Model {

            [Caption("Page Skin (Required)"), Description("Page Skin (Required)")]
            [UIHint("PageSkin"), Required, Trim]
            public SkinDefinition PageSkinReq { get; set; }

            [Caption("Popup Skin (Required)"), Description("Popup Skin (Required)")]
            [UIHint("PopupSkin"), Required, Trim]
            public SkinDefinition PopupSkinReq { get; set; }

            [Caption("Module Skins (Required)"), Description("Module Skins (Required)")]
            [UIHint("ModuleSkins"), Required, Trim]
            public SerializableList<SkinDefinition> ModuleSkinsReq { get; set; }

            [Caption("Page Skin"), Description("Page Skin (Required)")]
            [UIHint("PageSkin"), Required, Trim]
            public SkinDefinition PageSkin { get; set; }

            [Caption("Popup Skin"), Description("Popup Skin (Required)")]
            [UIHint("PopupSkin"), Required, Trim]
            public SkinDefinition PopupSkin { get; set; }

            [Caption("Module Skins (Required)"), Description("Module Skins (Required)")]
            [UIHint("ModuleSkins"), Required, Trim]
            public SerializableList<SkinDefinition> ModuleSkins { get; set; }

            [Caption("Page Skin (Read/Only)"), Description("Page Skin (Read/Only)")]
            [UIHint("PageSkin")]
            public SkinDefinition PageSkinRO { get; set; }

            [Caption("Popup Skin (Read/Only)"), Description("Popup Skin (Read/Only)")]
            [UIHint("PopupSkin")]
            public SkinDefinition PopupSkinRO { get; set; }

            [Caption("Module Skins (Read/Only)"), Description("Module Skins (Read/Only)")]
            [UIHint("ModuleSkins")]
            public SerializableList<SkinDefinition> ModuleSkinsRO { get; set; }

            public Model() {
                PageSkinReq = new SkinDefinition();
                PageSkin = new SkinDefinition();
                PageSkinRO = new SkinDefinition();
                PopupSkinReq = new SkinDefinition();
                PopupSkin = new SkinDefinition();
                PopupSkinRO = new SkinDefinition();
                ModuleSkinsReq = new SerializableList<SkinDefinition>();
                ModuleSkins = new SerializableList<SkinDefinition>();
                ModuleSkinsRO = new SerializableList<SkinDefinition>();
            }
        }

        [HttpGet]
        public ActionResult TemplateSkin() {
            Model model = new Model { };
            return View(model);
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateSkin_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
