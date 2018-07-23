/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class FileUpload1Component : YetaWFComponent {

        public const string TemplateName = "FileUpload1";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class FileUpload1EditComponent : FileUpload1Component, IYetaWFComponent<FileUpload1>, IYetaWFContainer<FileUpload1> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.Domain, Package.Product, "github.com.danielm.uploader");
            await base.IncludeAsync();
        }
        public Task<YHtmlString> RenderAsync(FileUpload1 model) {
            return RenderContainerAsync(model);
        }
        public Task<YHtmlString> RenderContainerAsync(FileUpload1 model) {
            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
                <div class='yt_fileupload1' id='{ControlId}' 
                        data-saveurl='{YetaWFManager.HtmlAttributeEncode(model.SaveURL)}' data-removeurl='{YetaWFManager.HtmlAttributeEncode(model.RemoveURL)}'>
                    <input type='button' class='t_upload' value='{YetaWFManager.HtmlAttributeEncode(model.SelectButtonText)}' title='{YetaWFManager.HtmlAttributeEncode(model.SelectButtonTooltip)}' />
                    <div class='t_drop'>{YetaWFManager.HtmlAttributeEncode(model.DropFilesText)}</div>
                    <div class='t_progressbar'></div>
                    <input type='file' name='__filename' class='t_filename' style='display:none' />
                </div>

                <script>
                    YetaWF_FileUpload1.init('{ControlId}', {YetaWFManager.JsonSerialize(model.SerializeForm)});
                </script>");

            return Task.FromResult(hb.ToYHtmlString());
        }

    }
}
