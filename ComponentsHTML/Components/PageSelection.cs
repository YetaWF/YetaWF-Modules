using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class PageSelectionComponent : YetaWFComponent, IYetaWFComponent<Guid>, IYetaWFComponent<Guid?> {

        public const string TemplateName = "PageSelection";

        public override ComponentType GetComponentType() { return ComponentType.Edit; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

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
            list.Insert(0, new SelectionItem<string> { Text = this.__ResStr("select", "(select)"), Value = null });

            YHtmlString ddList = await DropDownListComponent.RenderDropDownListAsync((model ?? Guid.Empty).ToString(), list, this, "yt_pageselection");

            // link
            YTagBuilder tag = new YTagBuilder("a");

            PageDefinition page = null;
            if (model != null)
                page = await PageDefinition.LoadAsync((Guid)model);

            tag.MergeAttribute("href", (page != null ? page.EvaluatedCanonicalUrl : ""));
            tag.MergeAttribute("target", "_blank");
            tag.MergeAttribute("rel", "nofollow noopener noreferrer");
            tag.Attributes.Add(Basics.CssTooltip, this.__ResStr("linkTT", "Click to preview the page in a new window - not all pages can be displayed correctly and may require additional parameters"));

            // image
            SkinImages skinImages = new SkinImages();
            string imageUrl = await skinImages.FindIcon_TemplateAsync("PagePreview.png", Package, TemplateName);
            YTagBuilder tagImg = ImageHelper.BuildKnownImageYTag(imageUrl, alt: this.__ResStr("linkAlt", "Preview"));

            tag.InnerHtml = tag.InnerHtml + tagImg.ToString(YTagRenderMode.StartTag);
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
    YetaWF_PageSelection.init('{DivId}');
</script>");
            
            return hb.ToYHtmlString();
        }
    }
}
