using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Extensions;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.BootstrapCarousel.Controllers;
using YetaWF.Modules.BootstrapCarousel.Models;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.ImageRepository.Components {

    public abstract class SlideShowComponent : YetaWFComponent {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(SlideShowComponent), name, defaultValue, parms); }

        public const string TemplateName = "SlideShow";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    public class SlideShowDisplayComponent : SlideShowComponent, IYetaWFComponent<CarouselInfo> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(CarouselInfo model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div id='{ControlId}' class='yt_bootstrapcarousel_slideshow t_display carousel slide' data-ride='carousel' data-interval='{model.Interval}' 
            data-wrap='{(model.Wrap ? "true" : "false")}' data-pause='{(model.Pause ? "hover" : "false")}' data-keyboard='{(model.Keyboard ? "true" : "false")}'>
    <!-- Indicators -->
    <ol class='carousel-indicators'>
");

            int index = 0;
            foreach(CarouselInfo.CarouselItem slide in model.Slides) {
                if (index == 0)
                    hb.Append($@"<li data-target='#{ControlId}' data-slide-to='{index}' class='active'></li>");
                else
                    hb.Append($@"<li data-target='#{ControlId}' data-slide-to='{index}'></li>");
                ++index;
            }
            hb.Append($@"
    </ol>
    <!-- Wrapper for slides -->
    <div class='carousel-inner' role='listbox'>
");

            index = 0;
            foreach (CarouselInfo.CarouselItem slide in model.Slides) {
                hb.Append($@"<div class='carousel-item{(index == 0 ? " active" : "")}'>");

                using (Manager.StartNestedComponent($"{FieldName}[{index}]")) {

                    if (!string.IsNullOrWhiteSpace(slide.Url)) {
                        hb.Append($@"<a href='{YetaWFManager.HtmlAttributeEncode(Manager.CurrentSite.MakeUrl(slide.Url))}' target='_blank' rel='noopener noreferrer' class='{YetaWF.Core.Addons.Basics.CssNoTooltip}'>");
                        hb.Append(await HtmlHelper.ForDisplayAsync(slide, nameof(CarouselInfo.CarouselItem.ImageDisplay)));
                        hb.Append($@"</a>");
                    } else {
                        hb.Append(await HtmlHelper.ForDisplayAsync(slide, nameof(CarouselInfo.CarouselItem.ImageDisplay)));
                    }
                }
                if (!string.IsNullOrWhiteSpace(slide.CompleteCaption.ToString())) {
                    hb.Append($@"<div class='carousel-caption'>
                        {YetaWFManager.HtmlEncode(slide.CompleteCaption.ToString())}
                    </div>");
                }
                hb.Append("</div>");
                ++index;
            }
            hb.Append($@"
    </div>
    <!-- Controls -->
    <a class='carousel-control-prev' href='#{ControlId}' role='button' data-slide='prev'>
        <span class='carousel-control-prev-icon' aria-hidden='true'></span>
        <span class='sr-only'>Previous</span>
    </a>
    <a class='carousel-control-next' href='#{ControlId}' role='button' data-slide='next'>
        <span class='carousel-control-next-icon' aria-hidden='true'></span>
        <span class='sr-only'>Next</span>
    </a>
</div>");

            return hb.ToYHtmlString();
        }
    }

    public class SlideShowEditComponent : SlideShowComponent, IYetaWFComponent<CarouselInfo> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(CarouselInfo model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div id='{ControlId}' class='yt_bootstrapcarousel_slideshow t_edit'>
    {(await HtmlHelper.ForEditContainerAsync(model, "PropertyList")).ToString()}
    <div class='t_slides' id='{DivId}'>
        {PropertyListComponentBase.RenderTabStripStart(DivId)}
");
            for (int i = 0 ; i <= model.Slides.Count-1 ; ++i ) {
                hb.Append(PropertyListComponentBase.RenderTabEntry(DivId, this.__ResStr("tab", "Image {0}", i+1), null, i));
            }
            hb.Append(PropertyListComponentBase.RenderTabStripEnd(DivId));

            int tabEntry = 0;
            foreach (CarouselInfo.CarouselItem slide in model.Slides) {
                hb.Append(PropertyListComponentBase.RenderTabPaneStart(DivId, tabEntry, "t_slide"));
                hb.Append($@"<input type='hidden' value='{tabEntry}'");
                using (Manager.StartNestedComponent($"{FieldName}[{tabEntry}]")) {
                    hb.Append(await HtmlHelper.ForEditContainerAsync(slide, "PropertyList"));
                }
                hb.Append(PropertyListComponentBase.RenderTabPaneEnd(DivId, tabEntry));
                ++tabEntry;
            }
            hb.Append(await PropertyListComponentBase.RenderTabInitAsync(DivId, FieldName, model));

            hb.Append($@"
</div>
    <div class='t_buttons'>
        <input type='button' class='t_apply' value='{YetaWFManager.HtmlAttributeEncode(this.__ResStr("btnApply", "Apply"))}' title='{YetaWFManager.HtmlAttributeEncode(this.__ResStr("txtApply", "Click to apply the current changes"))}' />
        <input type='button' class='t_apply' value='{YetaWFManager.HtmlAttributeEncode(this.__ResStr("btnUp", "<<"))}' title='{YetaWFManager.HtmlAttributeEncode(this.__ResStr("txtUp", "Click to move the current image"))}' />
        <input type='button' class='t_apply' value='{YetaWFManager.HtmlAttributeEncode(this.__ResStr("btnDown", ">>"))}' title='{YetaWFManager.HtmlAttributeEncode(this.__ResStr("txtDown", "Click move the current image"))}' />
        <input type='button' class='t_apply' value='{YetaWFManager.HtmlAttributeEncode(this.__ResStr("btnIns", "Insert"))}' title='{YetaWFManager.HtmlAttributeEncode(this.__ResStr("txtIns", "Click to insert a new image before the current image"))}' />
        <input type='button' class='t_apply' value='{YetaWFManager.HtmlAttributeEncode(this.__ResStr("btnAdd", "Add"))}' title='{YetaWFManager.HtmlAttributeEncode(this.__ResStr("txtAdd", "Click to add a new image after the current image"))}' />
        <input type='button' class='t_apply' value='{YetaWFManager.HtmlAttributeEncode(this.__ResStr("btnDelete", "Remove"))}' title='{YetaWFManager.HtmlAttributeEncode(this.__ResStr("txtDelete", "Click to remove the current image"))}' />
    </div>
</div>
<script>
    YetaWF_BootstrapCarousel.init('{ControlId}');
</script>");

            return hb.ToYHtmlString();
        }
    }
}
