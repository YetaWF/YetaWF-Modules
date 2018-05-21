using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class PaneSelectionComponent : YetaWFComponent, IYetaWFComponent<string> {

        public const string TemplateName = "PaneSelection";

        public override ComponentType GetComponentType() { return ComponentType.Edit; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public async Task<YHtmlString> RenderAsync(string model) {

            List<string> list;
            if (!TryGetSiblingProperty($"{PropertyName}_List", out list))
                list = new List<string>();
            List<SelectionItem<string>> itemList = new List<SelectionItem<string>>();
            foreach (string l in list) {
                itemList.Add(new SelectionItem<string> {
                    Text = l,
                    Value = l,
                });
            }
            return await DropDownListComponent.RenderDropDownListAsync(model, itemList, this, "yt_paneselection");
        }
    }
}
