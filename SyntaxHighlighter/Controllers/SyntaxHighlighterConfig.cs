/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.SyntaxHighlighter.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.SyntaxHighlighter.Controllers {

    public class SyntaxHighlighterConfigModuleController : ControllerImpl<YetaWF.Modules.SyntaxHighlighter.Modules.SyntaxHighlighterConfigModule> {

        public SyntaxHighlighterConfigModuleController() { }

        [Trim]
        [Header("The settings are used to define syntax highlighting for the entire site. " +
            "If more than one syntax highlighter is available, both can be used, but should not be used on the same page. " +
            "To activate syntax highlighting, the Site, Page or Module Settings must reference one of the available skin modules. " +
            "The <pre> and/or <code> tags must be used to define the desired syntax coloring language.")]
        public class Model {

            [Category("Highlight.js"), Caption("Skin"), Description("The skin used for syntax highlighting")]
            [UIHint("YetaWF_SyntaxHighlighter_HighlightJS"), StringLength(ConfigData.MaxSkinName), AdditionalMetadata("NoDefault", true), Trim]
            public string HighlightJSSkin { get; set; }

            [TextAbove("The syntax highlighter uses Highlight.js.")]
            [Category("Highlight.js"), Caption("Help"), Description("The syntax highlighter uses Highlight.js")]
            [UIHint("Url"), ReadOnly]
            public string HighlightJSUrl { get; set; }

            [Category("Highlight.js"), Caption("Languages"), Description("Displays help for the languages that can be used by Highlight.js for syntax coloring - Not all may be installed and available")]
            [UIHint("Url"), ReadOnly]
            public string LanguagesHighlightJSUrl { get; set; }

            [Category("Alex Gorbatchev"), Caption("Skin"), Description("The skin used for syntax highlighting")]
            [UIHint("YetaWF_SyntaxHighlighter_SyntaxHighlighter"), StringLength(ConfigData.MaxSkinName), AdditionalMetadata("NoDefault", true), Trim]
            public string SyntaxHighlighterSkin { get; set; }

            [TextAbove("The syntax highlighter uses Alex Gorbatchev's SyntaxHighlighter.")]
            [Category("Alex Gorbatchev"), Caption("Help"), Description("The syntax highlighter uses Alex Gorbatchev's SyntaxHighlighter")]
            [UIHint("Url"), ReadOnly]
            public string SyntaxHighlighterUrl { get; set; }

            [Category("Alex Gorbatchev"), Caption("Languages"), Description("Displays help for the languages (brushes) that can be used by Alex Gorbatchev's syntax highlighter - Not all may be installed and available")]
            [UIHint("Url"), ReadOnly]
            public string LanguagesSyntaxHighlighterUrl { get; set; }

            public ConfigData GetData(ConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(ConfigData data) {
                ObjectSupport.CopyData(data, this);
                SyntaxHighlighterUrl = "http://alexgorbatchev.com/SyntaxHighlighter/";
                HighlightJSUrl = "https://highlightjs.org";
                LanguagesHighlightJSUrl = "http://highlightjs.readthedocs.io/en/latest/css-classes-reference.html#language-names-and-aliases";
                LanguagesSyntaxHighlighterUrl = "http://alexgorbatchev.com/SyntaxHighlighter/manual/brushes/";
            }
            public Model() { }
        }

        [AllowGet]
        public async Task<ActionResult> SyntaxHighlighterConfig() {
            using (ConfigDataProvider dataProvider = new ConfigDataProvider()) {
                Model model = new Model { };
                ConfigData data = await dataProvider.GetItemAsync();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The syntax highlighter settings could not be found"));
                model.SetData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> SyntaxHighlighterConfig_Partial(Model model) {
            using (ConfigDataProvider dataProvider = new ConfigDataProvider()) {
                ConfigData data = await dataProvider.GetItemAsync();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display
                await dataProvider.UpdateConfigAsync(data);
                return FormProcessed(model, this.__ResStr("okSaved", "Syntax highlighter settings saved"));
            }
        }
    }
}
