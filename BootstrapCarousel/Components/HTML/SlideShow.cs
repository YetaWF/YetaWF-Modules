/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Modules.BootstrapCarousel.Models;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.BootstrapCarousel.Components {

    public abstract class SlideShowComponent : YetaWFComponent {

        public const string TemplateName = "SlideShow";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    /// <summary>
    /// This component is used by the YetaWF.BootstrapCarousel package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class SlideShowDisplayComponent : SlideShowComponent, IYetaWFComponent<CarouselInfo> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public class CarouselSetup {
            public int Interval { get; set; }
            public bool Keyboard { get; set; }
            public bool Pause { get; set; }
            public bool Wrap { get; set; }

            public int ImageCount { get; set; }
        }

        public async Task<string> RenderAsync(CarouselInfo model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div id='{ControlId}' class='yt_bootstrapcarousel_slideshow t_display'>
    <ol class='t_indicators'>");

            int index = 0;
            foreach(CarouselInfo.CarouselItem slide in model.Slides) {
                if (index == 0)
                    hb.Append($@"
        <li class='t_active'></li>");
                else
                    hb.Append($@"
        <li></li>");
                ++index;
            }
            hb.Append($@"
    </ol>
    <div class='t_inner' role='listbox'>");

            index = 0;
            foreach (CarouselInfo.CarouselItem slide in model.Slides) {
                // can't have white-space between divs
                hb.Append($@"<div class='t_item'>");

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
            <div class='t_caption'>
                {Utility.HE(slide.CompleteCaption.ToString())}
            </div>");
                }
                hb.Append(@"
        </div>");

                ++index;
            }
            hb.Append($@"
    </div>
    <a class='t_prev' href='#' role='button' data-slide='prev'>
        <span class='t_prev_icon' aria-hidden='true'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-angle-left")}</span>
    </a>
    <a class='t_next' href='#' role='button' data-slide='next'>
        <span class='t_next_icon' aria-hidden='true'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-angle-right")}</span>
    </a>
</div>");

            CarouselSetup setup = new CarouselSetup {
                Interval = model.Interval,
                Wrap = model.Wrap,
                Keyboard = model.Keyboard,
                Pause = model.Pause,
                ImageCount = model.Slides.Count,
            };
            Manager.ScriptManager.AddLast($@"new YetaWF_BootstrapCarousel.CarouselComponent('{ControlId}', {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }
    }

    /// <summary>
    /// This component is used by the YetaWF.BootstrapCarousel package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class SlideShowEditComponent : SlideShowComponent, IYetaWFComponent<CarouselInfo> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        internal class UI {
            [UIHint("Tabs")]
            public TabsDefinition TabsDef { get; set; } = null!;
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
                        HtmlBuilder hbt = new HtmlBuilder();
                        using (Manager.StartNestedComponent($"{FieldNamePrefix}.{nameof(model.Slides)}[{tabIndex}]")) {
                            hbt.Append(await HtmlHelper.ForEditContainerAsync(model.Slides[tabIndex], "PropertyList"));
                        }
                        return hbt.ToString();
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
        <input type='button' class='y_button t_apply' value='{HAE(this.__ResStr("btnApply", "Apply"))}' title='{HAE(this.__ResStr("txtApply", "Click to apply the current changes"))}' />
        <input type='button' class='y_button t_up' value='{HAE(this.__ResStr("btnUp", "<<"))}' title='{HAE(this.__ResStr("txtUp", "Click to move the current image"))}' />
        <input type='button' class='y_button t_down' value='{HAE(this.__ResStr("btnDown", ">>"))}' title='{HAE(this.__ResStr("txtDown", "Click move the current image"))}' />
        <input type='button' class='y_button t_ins' value='{HAE(this.__ResStr("btnIns", "Insert"))}' title='{HAE(this.__ResStr("txtIns", "Click to insert a new image before the current image"))}' />
        <input type='button' class='y_button t_add' class='y_button' value='{HAE(this.__ResStr("btnAdd", "Add"))}' title='{HAE(this.__ResStr("txtAdd", "Click to add a new image after the current image"))}' />
        <input type='button' class='y_button t_delete' value='{HAE(this.__ResStr("btnDelete", "Remove"))}' title='{HAE(this.__ResStr("txtDelete", "Click to remove the current image"))}' />
    </div>
</div>");

            Manager.ScriptManager.AddLast($@"new YetaWF_BootstrapCarousel.SlideShowEdit('{ControlId}');");

            return hb.ToString();
        }
    }
}
