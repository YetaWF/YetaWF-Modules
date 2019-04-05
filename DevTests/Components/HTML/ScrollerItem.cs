/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.DevTests.Controllers;

namespace YetaWF.Modules.DevTests.Components {

    public abstract class ScrollerItemComponent : YetaWFComponent {

        public const string TemplateName = "ScrollerItem";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class ScrollerItemDisplayComponent : ScrollerItemComponent, IYetaWFContainer<TemplateScrollerModuleController.ScrollerItem> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<string> RenderContainerAsync(TemplateScrollerModuleController.ScrollerItem model) {
            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='yt_yetawf_devtests_scrolleritem t_display'>
    <div class='t_image'>{await HtmlHelper.ForDisplayAsync(model, nameof(model.Image))}</div>
    <div class='t_title'>{HE(model.Title)}</div>
    <div class='t_summary'>{HE(model.Summary)}</div>
    <div class='y_cleardiv'></div>
</div>");

            return hb.ToString();
        }
    }
}
