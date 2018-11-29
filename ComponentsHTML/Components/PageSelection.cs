/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Addons;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class PageSelectionComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(PageSelectionComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "PageSelection";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class PageSelectionComponentDisplay : PageSelectionComponentBase, IYetaWFComponent<Guid>, IYetaWFComponent<Guid?> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(Guid model) {
            return await RenderAsync((Guid?)model);
        }
        public async Task<YHtmlString> RenderAsync(Guid? model) {
            HtmlBuilder hb = new HtmlBuilder();

            if (model != null) {
                PageDefinition page = await PageDefinition.LoadPageDefinitionAsync((Guid)model);
                if (page == null)
                    hb.Append(__ResStr("notFound", "(Page not found - {0})", model.ToString()));
                else
                    hb.Append(HE(page.Url));
            }
            return hb.ToYHtmlString();
        }
    }

    public class PageSelectionComponentEdit : PageSelectionComponentBase, IYetaWFComponent<Guid>, IYetaWFComponent<Guid?> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(Guid model) {
            return await RenderAsync((Guid?)model);
        }
        public async Task<YHtmlString> RenderAsync(Guid? model) {
            HtmlBuilder hb = new HtmlBuilder();

            // dropdown
            List<SelectionItem<string>> list;
            list = (
                from p in await PageDefinition.GetDesignedPagesAsync() orderby p.Url select
                    new SelectionItem<string> {
                        Text = p.Url,
                        Value = p.PageGuid.ToString(),
                    }).ToList<SelectionItem<string>>();
            list.Insert(0, new SelectionItem<string> { Text = __ResStr("select", "(select)"), Value = null });

            YHtmlString ddList = await DropDownListComponent.RenderDropDownListAsync(this, (model ?? Guid.Empty).ToString(), list, "yt_pageselection");

            // link
            YTagBuilder tag = new YTagBuilder("a");

            PageDefinition page = null;
            if (model != null)
                page = await PageDefinition.LoadAsync((Guid)model);

            tag.MergeAttribute("href", (page != null ? page.EvaluatedCanonicalUrl : ""));
            tag.MergeAttribute("target", "_blank");
            tag.MergeAttribute("rel", "nofollow noopener noreferrer");
            tag.Attributes.Add(Basics.CssTooltip, __ResStr("linkTT", "Click to preview the page in a new window - not all pages can be displayed correctly and may require additional parameters"));

            tag.InnerHtml = tag.InnerHtml + ImageHTML.BuildKnownIcon("#PagePreview", sprites: Info.PredefSpriteIcons);
            string linkTag = tag.ToString(YTagRenderMode.Normal);

            hb.Append($@"
<div id='{DivId}' class='yt_pageselection t_edit'>
    <div class='t_select'>
        {ddList.ToString()}
    </div>
    <div class='t_link'>
        {linkTag}
    </div>
    <div class='t_description'>
    </div>
</div>
<script>
    new YetaWF_ComponentsHTML.PageSelectionEditComponent('{DivId}');
</script>");

            return hb.ToYHtmlString();
        }
    }
}
