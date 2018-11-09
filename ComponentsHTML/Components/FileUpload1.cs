/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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

        public class Setup {
            public string SaveUrl { get; set; }
            public string RemoveUrl { get; set; }
            public bool SerializeForm { get; set; }// serialize all form data when uploading a file
        }

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "github.com.danielm.uploader");
            await base.IncludeAsync();
        }
        public Task<YHtmlString> RenderAsync(FileUpload1 model) {
            return RenderContainerAsync(model);
        }
        public Task<YHtmlString> RenderContainerAsync(FileUpload1 model) {

            UseSuppliedIdAsControlId();
                
            HtmlBuilder hb = new HtmlBuilder();

            Setup setup = new Setup {
                SaveUrl = model.SaveURL,
                RemoveUrl = model.RemoveURL,
                SerializeForm = model.SerializeForm,
            };

            hb.Append($@"
<div class='yt_fileupload1' id='{ControlId}' data-saveurl='{HAE(model.SaveURL)}' data-removeurl='{HAE(model.RemoveURL)}'>
    <input type='button' class='t_upload' value='{HAE(model.SelectButtonText)}' title='{HAE(model.SelectButtonTooltip)}' />
    <div class='t_drop'>{HAE(model.DropFilesText)}</div>
    <div class='t_progressbar'></div>
    <input type='file' name='__filename' class='t_filename' style='display:none' />
</div>
<script>
    new YetaWF_ComponentsHTML.FileUpload1Component('{ControlId}', {YetaWFManager.JsonSerialize(setup)});
</script>");

            return Task.FromResult(hb.ToYHtmlString());
        }

    }
}
