/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.SyntaxHighlighter.Startup;

namespace YetaWF.Modules.SyntaxHighlighter.Components {

    public abstract class HighlightJSComponent : YetaWFComponent {

        public const string TemplateName = "HighlightJS";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// This component is used by the YetaWF.SyntaxHighlighter package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class HighlightJSDisplayComponent : HighlightJSComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<string> RenderAsync(string model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='yt_yetawf_syntaxhighlighter_highlightjs t_display'>");

            if (string.IsNullOrEmpty(model)) {
                hb.Append(Utility.HE(this.__ResStr("noHighlightJSSkin", "(default)")));
            } else {
                hb.Append(Utility.HE(model));
            }
            hb.Append($@"
</div>");
            return Task.FromResult(hb.ToString());
        }
    }

    /// <summary>
    /// This component is used by the YetaWF.SyntaxHighlighter package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class HighlightJSEditComponent : HighlightJSComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<string> RenderAsync(string model) {

            // get all available skins
            SkinAccess skinAccess = new SkinAccess();
            List<SelectionItem<string>> list = (from theme in skinAccess.GetHighlightJSThemeList() select new SelectionItem<string>() {
                Text = theme.Name,
                Value = theme.Name,
            }).ToList();

            bool useDefault = PropData.GetAdditionalAttributeValue<bool>("NoDefault", false);
            if (useDefault)
                list.Insert(0, new SelectionItem<string> {
                    Text = this.__ResStr("default", "(Site Default)"),
                    Tooltip = this.__ResStr("defaultTT", "Use the site defined default theme"),
                    Value = "",
                });
            else if (model == null)
                model = SkinAccess.GetHighlightJSDefaultTheme();

            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_yetawf_syntaxhighlighter_highlightjs");
        }
    }
}
