/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Basics.Controllers {

    public class ModuleSkinInfoModuleController : ControllerImpl<YetaWF.Modules.Basics.Modules.ModuleSkinInfoModule> {

        public ModuleSkinInfoModuleController() { }

        public class DisplayModel {

            [Caption("Page Skin"), Description("The skin used to for the current page")]
            [UIHint("PageSkin")]
            public SkinDefinition PageSelectedSkin { get; set; }

            [Caption("Page Popup Skin"), Description("The skin used for the current page in a popup window")]
            [UIHint("PopupSkin")]
            public SkinDefinition PageSelectedPopupSkin { get; set; }

            [Caption("Page jQuery UI Skin"), Description("The page skin for jQuery-UI elements (buttons, modal dialogs, etc.)")]
            [UIHint("jQueryUISkin")]
            public string PagejQueryUISkin { get; set; }

            [Caption("Page Kendo UI Skin"), Description("The page skin for Kendo UI elements (buttons, modal dialogs, etc.)")]
            [UIHint("KendoUISkin")]
            public string PageKendoUISkin { get; set; }

            [Caption("Page Syntax Highlighter Skin"), Description("The page skin for syntax highlighting (in text areas)")]
            [UIHint("YetaWF_SyntaxHighlighter_SyntaxHighlighterSkin")]
            public string PageSyntaxHighlighterSkin { get; set; }

            [Caption("Site Default Page Skin"), Description("The default skin used for pages")]
            [UIHint("PageSkin")]
            public SkinDefinition SiteSelectedSkin { get; set; }

            [Caption("Site Default Popup Skin"), Description("The default skin used in a popup window")]
            [UIHint("PopupSkin")]
            public SkinDefinition SiteSelectedPopupSkin { get; set; }

            [Caption("Site Default jQuery UI Skin"), Description("The default skin for jQuery-UI elements (buttons, modal dialogs, etc.)")]
            [UIHint("jQueryUISkin")]
            public string SitejQueryUISkin { get; set; }

            [Caption("Site Default Kendo UI Skin"), Description("The default skin for Kendo UI elements (buttons, modal dialogs, etc.)")]
            [UIHint("KendoUISkin")]
            public string SiteKendoUISkin { get; set; }

            [Caption("Site Syntax Highlighter Skin"), Description("The default skin for syntax highlighting in textareas")]
            [UIHint("YetaWF_SyntaxHighlighter_SyntaxHighlighter")]
            public string SiteSyntaxHighlighterSkin { get; set; }

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
            model.SiteSelectedSkin = Manager.CurrentSite.SelectedSkin;
            model.SiteSelectedPopupSkin = Manager.CurrentSite.SelectedPopupSkin;
            model.SitejQueryUISkin = Manager.CurrentSite.jQueryUISkin;
            model.SiteKendoUISkin = Manager.CurrentSite.KendoUISkin;
            model.SiteSyntaxHighlighterSkin = Manager.CurrentSite.SyntaxHighlighterSkin;
            model.PageSelectedSkin = Manager.CurrentPage.SelectedSkin;
            model.PageSelectedPopupSkin = Manager.CurrentPage.SelectedPopupSkin;
            model.PagejQueryUISkin = Manager.CurrentPage.jQueryUISkin;
            model.PageKendoUISkin = Manager.CurrentPage.KendoUISkin;
            model.PageSyntaxHighlighterSkin = Manager.CurrentPage.SyntaxHighlighterSkin;
            model.Characters = "<span class='t_chars'>abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789<br/>abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789</span>";
            return View(model);
        }
    }
}