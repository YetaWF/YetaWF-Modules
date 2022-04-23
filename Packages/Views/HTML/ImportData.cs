/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Packages.Controllers;
using YetaWF.Modules.Packages.Modules;

namespace YetaWF.Modules.Packages.Views {

    public class ImportDataView : YetaWFView, IYetaWFView<ImportDataModule, ImportDataModuleController.ImportDataModel> {

        public const string ViewName = "ImportData";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(ImportDataModule module, ImportDataModuleController.ImportDataModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='t_description'>
    {HE(this.__ResStr("text", "Import package data from a zip file (created by Export Data)."))}
</div>
{await HtmlHelper.ForEditAsync(model, nameof(model.UploadFile))}");

            return hb.ToString();
        }
    }
}
