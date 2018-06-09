using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class FileFolderSizeComponentBase : YetaWFComponent {

        public const string TemplateName = "FileFolderSize";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class FileFolderSizeDisplayComponent : FileFolderSizeComponentBase, IYetaWFComponent<long> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(long model) {
            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='yt_filefoldersize t_display'>
    {Formatting.FormatFileFolderSize(model)}
</div>
");
            return Task.FromResult(hb.ToYHtmlString());
        }
    }
}
