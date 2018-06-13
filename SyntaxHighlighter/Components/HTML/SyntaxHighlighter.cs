﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.SyntaxHighlighter.Support;

namespace YetaWF.Modules.SyntaxHighlighter.Components {

    public abstract class SyntaxHighlighterComponent : YetaWFComponent {

        public const string TemplateName = "SyntaxHighlighter";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    public class SyntaxHighlighterDisplayComponent : SyntaxHighlighterComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(string model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='yt_yetawf_syntaxhighlighter_syntaxhighlighter t_display'>");

            if (string.IsNullOrEmpty(model)) {
                hb.Append(YetaWFManager.HtmlEncode(this.__ResStr("noSyntaxHighlighterSkin", "(default)")));
            } else {
                hb.Append(YetaWFManager.HtmlEncode(model));
            }
            hb.Append($@"
</div>");
            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public class SyntaxHighlighterEditComponent : SyntaxHighlighterComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(string model) {

            // get all available skins
            SkinAccess skinAccess = new SkinAccess();
            List<SelectionItem<string>> list = (from theme in skinAccess.GetSyntaxHighlighterThemeList() select new SelectionItem<string>() {
                Text = theme.Name,
                Tooltip = theme.Description,
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
                model = SkinAccess.GetSyntaxHighlighterDefaultSkin();


            if (useDefault)
                list.Insert(0, new SelectionItem<string> {
                    Text = this.__ResStr("default", "(Site Default)"),
                    Tooltip = this.__ResStr("defaultTT", "Use the site defined default theme"),
                    Value = "",
                });
            else if (model == null)
                model = SkinAccess.GetSyntaxHighlighterDefaultSkin();

            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_yetawf_syntaxhighlighter_syntaxhighlighter");
        }
    }
}
