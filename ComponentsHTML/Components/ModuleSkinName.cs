using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class ModuleSkinNameComponent : YetaWFComponent {

        public const string TemplateName = "ModuleSkinName";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    public class ModuleSkinNameDisplayComponent : ModuleSkinNameComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(string model) {

            ModuleSkinList modSkins = GetSiblingProperty($"{PropertyName}_ModuleSkinList", new ModuleSkinList());
            string name = (from l in modSkins where l.Name == model select l.Name).FirstOrDefault();
            if (name == null)
                name = modSkins.First().Name;
            return Task.FromResult(new YHtmlString(name));
        }
    }
    public class ModuleSkinNameEditComponent : ModuleSkinNameComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(string model) {

            ModuleSkinList modSkins = GetSiblingProperty($"{PropertyName}_ModuleSkinList", new ModuleSkinList());
            List<SelectionItem<string>> list = (from l in modSkins select new SelectionItem<string>() {
                Text = l.Name,
                Tooltip = l.Description,
                Value = l.CssClass,
            }).ToList();
            return await DropDownListComponent.RenderDropDownListAsync(model, list, this, null);
        }
    }
}
