/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Extensions;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.ImageRepository.Components {

    public abstract class ImageSelectionComponent : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ImageSelectionComponent), name, defaultValue, parms); }

        public const string TemplateName = "ImageSelection";

        public override Package GetPackage() { return ImageRepository.Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// This component is used by the YetaWF.ImageRepository package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class ImageSelectionEditComponent : ImageSelectionComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<string> RenderAsync(string model) {

            HtmlBuilder hb = new HtmlBuilder();

            ImageSelectionInfo info = GetSiblingProperty<ImageSelectionInfo>($"{PropertyName}_Info");

            hb.Append($@"
<div id='{ControlId}' class='yt_imagerepository_imageselection'>
    {await HtmlHelper.ForEditComponentAsync(Container, PropertyName, model, "Hidden", HtmlAttributes: new { __NoTemplate = true }, Validation: true)}
    <div class='t_imgarea'>
        <div class='t_list'>
            <select class='t_native' name='List' size='10' style='height:{info.PreviewHeight}px'>");

            string modelPlain = model?.RemoveStartingAt(YetaWF.Core.Image.ImageSupport.ImageSeparator);
            foreach (var f in await info.GetFilesAsync()) {
                string fPlain = f.RemoveStartingAt(YetaWF.Core.Image.ImageSupport.ImageSeparator);
                string sel = fPlain == modelPlain ? " selected" : "";
                hb.Append($@"<option title='{HAE(fPlain)}' value='{HAE(f)}' {sel}>{HE(fPlain)}</option>");
            }

            hb.Append($@"
            </select>
        </div>
        <div class='t_preview'>
            <img src='{HAE(info.MakeImageUrl(model, info.PreviewWidth, info.PreviewHeight))}' alt='{__ResStr("preview", "Image Preview")}' />
        </div>
    </div>
    <div class='t_haveimage' {(string.IsNullOrWhiteSpace(model) ? "style='display:none'" : "")}>
        {await HtmlHelper.ForDisplayAsync(info, nameof(info.ClearImageButton))}
        {await HtmlHelper.ForDisplayAsync(info, nameof(info.RemoveImageButton))}
    </div>
    <div class='t_uploadarea'>
        {await HtmlHelper.ForEditAsync(info, nameof(info.FileUpload1))}
    </div>
</div>");

            Manager.ScriptManager.AddLast($@"new YetaWF_ImageRepository.ImageRepository('{ControlId}');");

            return hb.ToString();
        }
    }
}
