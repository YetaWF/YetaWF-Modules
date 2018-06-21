/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class JQueryUISkinComponent : YetaWFComponent {

        public const string TemplateName = "jQueryUISkin";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    public class JQueryUISkinDisplayComponent : JQueryUISkinComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(string model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"<div class='yt_jqueryuiskin t_display'>
                {(string.IsNullOrEmpty(model) ? "&nbsp;" : YetaWFManager.HtmlEncode(model))}
            </div>");

            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public class JQueryUISkinEditComponent : JQueryUISkinComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(string model) {

            // get all available skins
            SkinAccess skinAccess = new SkinAccess();
            List<SelectionItem<string>> list = (from theme in await skinAccess.GetJQueryThemeListAsync() select new SelectionItem<string>() {
                Text = theme.Name,
                Tooltip = theme.Description,
                Value = theme.Name,
            }).ToList();

            bool useDefault = !PropData.GetAdditionalAttributeValue<bool>("NoDefault");
            if (useDefault)
                list.Insert(0, new SelectionItem<string> {
                    Text = this.__ResStr("default", "(Site Default)"),
                    Tooltip = this.__ResStr("defaultTT", "Use the site defined default theme"),
                    Value = "",
                });
            else if (model == null)
                model = await SkinAccess.GetJQueryUIDefaultSkinAsync();

            // display the skins in a drop down
            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_jqueryuiskin");
        }
    }
}
