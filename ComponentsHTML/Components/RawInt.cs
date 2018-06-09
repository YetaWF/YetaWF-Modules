using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class RawIntComponentBase : YetaWFComponent {

        public const string TemplateName = "RawInt";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class RawIntDisplayComponent : RawIntComponentBase, IYetaWFComponent<int> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(int model) {
            return Task.FromResult(new YHtmlString(model.ToString()));
        }
    }
}
