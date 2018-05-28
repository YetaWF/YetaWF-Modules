using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Extensions;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.ImageRepository.Components {

    public abstract class ImageSelectionComponent : YetaWFComponent {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ImageSelectionComponent), name, defaultValue, parms); }

        public const string TemplateName = "ImageSelection";

        public override Package GetPackage() { return ImageRepository.Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class ImageSelectionEditComponent : ImageSelectionComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(string model) {

            HtmlBuilder hb = new HtmlBuilder();

            ImageSelectionInfo info = GetSiblingProperty<ImageSelectionInfo>($"{PropertyName}_Info");

            hb.Append($@"
<div id='{ControlId}' class='yt_imagerepository_imageselection'>
    {await HtmlHelper.ForEditComponentAsync(Container, PropertyName, model, "Hidden", Validation: true)}
    <div class='t_imgarea'>
        <div class='t_list'>
            <select class='t_native' name='List' size='10' style='height:{info.PreviewHeight}px'>");

            foreach(var f in info.GetFilesAsync().Result) {
                string name = f.RemoveStartingAt(YetaWF.Core.Image.ImageSupport.ImageSeparator);
                hb.Append($@"<option title = '{YetaWFManager.HtmlAttributeEncode(name)}' value='{YetaWFManager.HtmlAttributeEncode(f)}'>{YetaWFManager.HtmlEncode(name)}</option>");
            }

            hb.Append($@"
            </select>
        </div>
        <div class='t_preview'>
            <img src='{YetaWFManager.HtmlAttributeEncode(info.MakeImageUrl(model, info.PreviewWidth, info.PreviewHeight))}' alt='{this.__ResStr("preview", "Image Preview")}' />
        </div>
    </div>
    <div class='t_haveimage' {(string.IsNullOrWhiteSpace(model) ? "style='display:none'" : "")}>
        {await HtmlHelper.ForDisplayAsync(info, nameof(info.ClearImageButton))}
        {await HtmlHelper.ForDisplayAsync(info, nameof(info.RemoveImageButton))}
    </div>
    <div class='t_uploadarea'>
        {await HtmlHelper.ForEditAsync(info, nameof(info.FileUpload1))}
    </div>
</div>
<script>
    YetaWF_ImageRepository.initSelection('{ControlId}');
</script>");

            return hb.ToYHtmlString();
        }
    }
}
