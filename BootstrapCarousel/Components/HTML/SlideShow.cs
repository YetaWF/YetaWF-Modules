/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.BootstrapCarousel.Controllers;
using YetaWF.Modules.BootstrapCarousel.Models;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.BootstrapCarousel.Components {

    public abstract class SlideShowComponent : YetaWFComponent {

        public const string TemplateName = "SlideShow";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    public class SlideShowDisplayComponent : SlideShowComponent, IYetaWFComponent<CarouselInfo> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<string> RenderAsync(CarouselInfo model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div id='{ControlId}' class='yt_bootstrapcarousel_slideshow t_display carousel slide' data-interval='{model.Interval}'
            data-wrap='{(model.Wrap ? "true" : "false")}' data-pause='{(model.Pause ? "hover" : "false")}' data-keyboard='{(model.Keyboard ? "true" : "false")}'>
    <!-- Indicators -->
    <ol class='carousel-indicators'>");

            int index = 0;
            foreach(CarouselInfo.CarouselItem slide in model.Slides) {
                if (index == 0)
                    hb.Append($@"
        <li data-target='#{ControlId}' data-slide-to='{index}' class='active'></li>");
                else
                    hb.Append($@"
        <li data-target='#{ControlId}' data-slide-to='{index}'></li>");
                ++index;
            }
            hb.Append($@"
    </ol>
    <!-- Wrapper for slides -->
    <div class='carousel-inner' role='listbox'>");

            index = 0;
            foreach (CarouselInfo.CarouselItem slide in model.Slides) {
                hb.Append($@"
        <div class='carousel-item{(index == 0 ? " active" : "")}'>");

                using (Manager.StartNestedComponent($"{FieldNamePrefix}.{nameof(model.Slides)}[{index}]")) {

                    if (!string.IsNullOrWhiteSpace(slide.Url)) {
                        hb.Append($@"
            <a href='{HAE(Manager.CurrentSite.MakeUrl(slide.Url))}' target='_blank' rel='noopener noreferrer' class='{YetaWF.Core.Addons.Basics.CssNoTooltip}'>
                {await HtmlHelper.ForDisplayAsync(slide, nameof(CarouselInfo.CarouselItem.ImageDisplay))}
            </a>");
                    } else {
                        hb.Append(await HtmlHelper.ForDisplayAsync(slide, nameof(CarouselInfo.CarouselItem.ImageDisplay)));
                    }
                }
                if (!string.IsNullOrWhiteSpace(slide.CompleteCaption.ToString())) {
                    hb.Append($@"
            <div class='carousel-caption'>
                {Utility.HtmlEncode(slide.CompleteCaption.ToString())}
            </div>");
                }
                hb.Append(@"
        </div>");

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

            Manager.ScriptManager.AddLast($@"$('#{ControlId}').carousel();");

            return hb.ToString();
        }
    }

    public class SlideShowEditComponent : SlideShowComponent, IYetaWFComponent<CarouselInfo> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        internal class UI {
            [UIHint("Tabs")]
            public TabsDefinition TabsDef { get; set; }
        }
        
        public async Task<string> RenderAsync(CarouselInfo model) {

            UI ui = new UI {
                TabsDef = new TabsDefinition {
                    ActiveTabIndex = model._ActiveTab,
                }
            };
            for (int i = 0; i <= model.Slides.Count - 1; ++i) {
                ui.TabsDef.Tabs.Add(new TabEntry {
                    Caption = this.__ResStr("tab", "Image {0}", i + 1),
                    PaneCssClasses = "t_slide",
                    RenderPaneAsync = async (int tabIndex) => {
                        HtmlBuilder hb = new HtmlBuilder();
                        using (Manager.StartNestedComponent($"{FieldNamePrefix}.{nameof(model.Slides)}[{tabIndex}]")) {
                            hb.Append(await HtmlHelper.ForEditContainerAsync(model.Slides[tabIndex], "PropertyList"));
                        }
                        return hb.ToString();
                    },
                });
            }

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"
<div id='{ControlId}' class='yt_bootstrapcarousel_slideshow t_edit'>
    {await HtmlHelper.ForEditContainerAsync(model, "PropertyList")}
    <div class='t_slides' id='{DivId}'>
        {await HtmlHelper.ForDisplayAsync(ui, nameof(ui.TabsDef), HtmlAttributes: new { __NoTemplate = true })}
    </div>
    <div class='t_buttons'>
        <input type='button' class='t_apply' value='{HAE(this.__ResStr("btnApply", "Apply"))}' title='{HAE(this.__ResStr("txtApply", "Click to apply the current changes"))}' />
        <input type='button' class='t_up' value='{HAE(this.__ResStr("btnUp", "<<"))}' title='{HAE(this.__ResStr("txtUp", "Click to move the current image"))}' />
        <input type='button' class='t_down' value='{HAE(this.__ResStr("btnDown", ">>"))}' title='{HAE(this.__ResStr("txtDown", "Click move the current image"))}' />
        <input type='button' class='t_ins' value='{HAE(this.__ResStr("btnIns", "Insert"))}' title='{HAE(this.__ResStr("txtIns", "Click to insert a new image before the current image"))}' />
        <input type='button' class='t_add' value='{HAE(this.__ResStr("btnAdd", "Add"))}' title='{HAE(this.__ResStr("txtAdd", "Click to add a new image after the current image"))}' />
        <input type='button' class='t_delete' value='{HAE(this.__ResStr("btnDelete", "Remove"))}' title='{HAE(this.__ResStr("txtDelete", "Click to remove the current image"))}' />
    </div>
</div>");

            Manager.ScriptManager.AddLast($@"new YetaWF_BootstrapCarousel.SlideShowEdit('{ControlId}');");

            return hb.ToString();
        }
    }
}
