using System.Threading.Tasks;
using YetaWF.Core.Support;
using YetaWF.Core.Packages;
using YetaWF.Core.Components;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class HiddenEditComponent : YetaWFComponent, IYetaWFComponent<string> {

        public const string TemplateName = "Hidden";
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public Task<YHtmlString> RenderAsync(string model) {
            YTagBuilder tag = new YTagBuilder("input");
            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Normal);
            tag.MergeAttribute("type", "hidden");
            tag.MergeAttribute("value", model == null ? "" : model.ToString());
            return Task.FromResult(tag.ToYHtmlString(YTagRenderMode.StartTag));
        }
    }
}
