/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the FileUpload1 component implementation.
    /// </summary>
    public abstract class FileUpload1ComponentBase : YetaWFComponent {

        internal const string TemplateName = "FileUpload1";

        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// Allows uploading a file. The model defines various attributes for the FileUpload1 component. SaveURL and RemoveURL define the actions taken to save and remove an uploaded file.
    /// </summary>
    /// <remarks>
    /// The uploaded file is a temporary file, which will be removed automatically within a specified time frame, defined by the YetaWF.ImageRepository package.
    ///
    /// The uploaded file is processed by the controller defined by SaveURL(YetaWF.Core.Views.Shared.FileUpload1).
    /// </remarks>
    /// <example>
    /// [Caption("ZIP File"), Description("The ZIP file containing the module to be imported (creates a new module) ")]
    /// [UIHint("FileUpload1"), Required]
    /// public FileUpload1 UploadFile { get; set; }
    ///
    /// UploadFile = new FileUpload1 {
    ///     SelectButtonText = this.__ResStr("btnImport", "Import Module Data..."),
    ///     SaveURL = YetaWFManager.UrlFor(typeof(PageControlModuleController), nameof(PageControlModuleController.ImportPackage), new { __ModuleGuid = mod.ModuleGuid }),
    ///     RemoveURL = YetaWFManager.UrlFor(typeof(PageControlModuleController), nameof(PageControlModuleController.RemovePackage), new { __ModuleGuid = mod.ModuleGuid }),
    ///     SerializeForm = true,
    /// };
    /// </example>
    public class FileUpload1EditComponent : FileUpload1ComponentBase, IYetaWFComponent<FileUpload1?>, IYetaWFContainer<FileUpload1?> {

        internal class Setup {
            public string SaveUrl { get; set; } = null!;
            public string RemoveUrl { get; set; } = null!;
            public bool SerializeForm { get; set; }// serialize all form data when uploading a file
        }

        internal class UI {
            [UIHint("ProgressBar"), ReadOnly]
            public float ProgressBar { get; set; }
            public float ProgressBar_Min { get { return 0; } }
            public float ProgressBar_Max { get { return 100; } }
        }

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(FileUpload1? model) {
            return RenderContainerAsync(model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderContainerAsync(FileUpload1? model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (model == null)
                throw new System.ArgumentNullException(nameof(FileUpload1));

            Setup setup = new Setup {
                SaveUrl = model.SaveURL,
                RemoveUrl = model.RemoveURL,
                SerializeForm = model.SerializeForm,
            };
            UI ui = new UI();

            hb.Append($@"
<div class='yt_fileupload1' id='{ControlId}' data-saveurl='{HAE(model.SaveURL)}' data-removeurl='{HAE(model.RemoveURL)}'>
    <input type='button' class='y_button t_upload' value='{HAE(model.SelectButtonText)}' title='{HAE(model.SelectButtonTooltip)}' />
    <div class='t_drop'>{HAE(model.DropFilesText)}</div>
    {await HtmlHelper.ForDisplayAsync(ui, nameof(ui.ProgressBar))}");

            if (!IsContainerComponent) {
                hb.Append($@"
    <input type='hidden' name='{FieldName}' style='display:none' />");
            }

            hb.Append($@"
    <input type='file' name='__filename' class='t_filename t_button' style='display:none' />
</div>");

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.FileUpload1Component('{ControlId}', {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }
    }
}
