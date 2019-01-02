/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class ScrollerComponentBase : YetaWFComponent {

        public const string TemplateName = "Scroller";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class ScrollerDisplayComponent : ScrollerComponentBase, IYetaWFComponent<IEnumerable> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(IEnumerable model) {
            HtmlBuilder hb = new HtmlBuilder();

            string uiHint = PropData.GetAdditionalAttributeValue<string>("Template");
            if (uiHint == null) throw new InternalError("No UIHint available for scroller");


            hb.Append($@"
<div id='{DivId}' class='yt_scroller t_display'>
    <a class='t_left' href='javascript:void(0)'></a>
    <div class='t_scrollarea'>
        <div class='t_items'>");

            foreach (var item in model) {
                YTagBuilder tag = new YTagBuilder("div");
                tag.AddCssClass("t_item");
                tag.InnerHtml = (await HtmlHelper.ForDisplayContainerAsync(item, uiHint)).ToString();
                hb.Append(tag.ToString(YTagRenderMode.Normal));
            }

            hb.Append($@"
        </div>
    </div>
    <a class='t_right' href='javascript:void(0)'></a>
</div>
<script>
    new YetaWF_ComponentsHTML.ScrollerComponent('{DivId}');
</script>");

            return hb.ToYHtmlString();
        }
    }
}
