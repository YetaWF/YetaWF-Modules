/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.Basics.Controllers {

    public class ModuleSkinInfoModuleController : ControllerImpl<YetaWF.Modules.Basics.Modules.ModuleSkinInfoModule> {

        public ModuleSkinInfoModuleController() { }

        public class DisplayModel {

            [Caption("Site Defined Skin"), Description("The skin used for all pages/popups")]
            [UIHint("Skin")]
            public SkinDefinition SiteSelectedSkin { get; set; }

            [Caption("Site Defined Kendo UI Skin"), Description("The skin for Kendo UI elements (date, time, datetime)")]
            [UIHint("KendoUISkin")]
            public string SiteKendoUISkin { get; set; }

            [Caption("Letters"), Description("The letters used to calculate the average character width and height")]
            [UIHint("TextArea"), AdditionalMetadata("Encode", false), ReadOnly]
            public string Characters { get; set; }

            [Caption("Average Char. Width"), Description("The average character width, calculated using the current skin")]
            [UIHint("IntValue")]
            public int Width { get; set; }
            [Caption("Char. Height"), Description("The character height, calculated using the current skin")]
            [UIHint("IntValue")]
            public int Height { get; set; }

            [Caption("Letters Width"), Description("The overall width of the letters shown, calculated using the current skin")]
            [UIHint("IntValue")]
            public int LettersWidth { get; set; }
            [Caption("Letters Height"), Description("The overall height of the letters shown, calculated using the current skin")]
            [UIHint("IntValue")]
            public int LettersHeight { get; set; }
        }

        [AllowGet]
        public ActionResult ModuleSkinInfo() {
            DisplayModel model = new DisplayModel();
            model.SiteSelectedSkin = Manager.CurrentSite.Skin;
            model.SiteKendoUISkin = Manager.CurrentSite.KendoUISkin;
            model.Characters = "<span class='t_chars'>abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789<br/>abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789</span>";
            return View(model);
        }
    }
}