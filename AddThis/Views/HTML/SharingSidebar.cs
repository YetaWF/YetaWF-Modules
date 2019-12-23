/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/AddThis#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.AddThis.Controllers;
using YetaWF.Modules.AddThis.Modules;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.AddThis.Views {

    public class SharingSidebarView : YetaWFView, IYetaWFView<SharingSidebarModule, SharingSidebarModuleController.DisplayModel> {

        public const string ViewName = "SharingSidebar";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<string> RenderViewAsync(SharingSidebarModule module, SharingSidebarModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (!Manager.EditMode && !string.IsNullOrWhiteSpace(model.Code)) {
                hb.Append(model.Code);
            }

            return Task.FromResult(hb.ToString());
        }
    }
}
