/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.BootstrapCarousel.Controllers;
using YetaWF.Modules.BootstrapCarousel.Modules;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.BootstrapCarousel.Views {

    public class CarouselDisplayView : YetaWFView, IYetaWFView2<CarouselDisplayModule, CarouselDisplayModuleController.Model> {

        public const string ViewName = "CarouselDisplay";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(CarouselDisplayModule module, CarouselDisplayModuleController.Model model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (Manager.EditMode) {
                hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
{await RenderEndFormAsync()}
");
            } else {
                hb.Append(await HtmlHelper.ForDisplayAsync(model, nameof(model.SlideShow)));
            }
            return hb.ToString();
        }

        public async Task<string> RenderPartialViewAsync(CarouselDisplayModule module, CarouselDisplayModuleController.Model model) {

            HtmlBuilder hb = new HtmlBuilder();

            using (Manager.StartNestedComponent($"{nameof(model.SlideShow)}")) {

                hb.Append(await HtmlHelper.ForEditAsync(model, nameof(model.SlideShow)));

            }
            return hb.ToString();

        }
    }
}
