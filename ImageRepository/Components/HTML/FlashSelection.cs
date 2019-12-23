/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Extensions;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.ImageRepository.Components {

    public abstract class FlashSelectionComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(FlashSelectionComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "FlashSelection";

        public override Package GetPackage() { return ImageRepository.Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class FlashSelectionEditComponent : FlashSelectionComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddAddOnNamedAsync("YetaWF_ComponentsHTML", "google.com.swfobject");
            await base.IncludeAsync();
        }

        public async Task<string> RenderAsync(string model) {

            HtmlBuilder hb = new HtmlBuilder();

            FlashSelectionInfo info = GetSiblingProperty<FlashSelectionInfo>($"{PropertyName}_Info");
            string objId = UniqueId();

            hb.Append($@"
<div id='{ControlId}' class='yt_imagerepository_flashselection'>
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
            <div id='{objId}'>
                <p>{HE(__ResStr("noFlash", "Flash Not Installed"))}</p>
            </div>
        </div>
    </div>
    <div class='t_haveflash' {(string.IsNullOrWhiteSpace(model) ? "style='display:none'" : "")}>
        {await HtmlHelper.ForDisplayAsync(info, nameof(info.ClearImageButton))}
        {await HtmlHelper.ForDisplayAsync(info, nameof(info.RemoveImageButton))}
    </div>
    <div class='t_uploadarea'>
        {await HtmlHelper.ForEditAsync(info, nameof(info.FileUpload1))}
    </div>
</div>");

            Manager.ScriptManager.AddLast($@"
{BeginDocumentReady()}
    swfobject.embedSWF('{Utility.JserEncode(info.MakeFlashUrl(model))}', '{objId}', '{info.PreviewWidth}', '{info.PreviewHeight}', '9.0.0', false,
        null, {{ wmode: 'transparent', allowScriptAccess: 'true', quality:'high' }} );
{EndDocumentReady()}
new YetaWF_ImageRepository.FlashRepository('{ControlId}');");

            return hb.ToString();
        }
    }
}
