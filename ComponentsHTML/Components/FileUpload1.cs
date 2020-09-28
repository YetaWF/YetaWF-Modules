/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the FileUpload1 component implementation.
    /// </summary>
    public abstract class FileUpload1ComponentBase : YetaWFComponent {

        internal const string TemplateName = "FileUpload1";

        /// <summary>
        /// Returns the package implementing the component.
        /// </summary>
        /// <returns>Returns the package implementing the component.</returns>
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        /// <summary>
        /// Returns the component name.
        /// </summary>
        /// <returns>Returns the component name.</returns>
        /// <remarks>Components in packages whose product name starts with "Component" use the exact name returned by GetTemplateName when used in UIHint attributes. These are considered core components.
        /// Components in other packages use the package's area name as a prefix. E.g., the UserId component in the YetaWF.Identity package is named "YetaWF_Identity_UserId" when used in UIHint attributes.
        ///
        /// The GetTemplateName method returns the component name without area name prefix in all cases.</remarks>
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
    public class FileUpload1EditComponent : FileUpload1ComponentBase, IYetaWFComponent<FileUpload1>, IYetaWFContainer<FileUpload1> {

        internal class Setup {
            public string SaveUrl { get; set; }
            public string RemoveUrl { get; set; }
            public bool SerializeForm { get; set; }// serialize all form data when uploading a file
        }

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            await JqueryUICore.UseAsync();
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "github.com.danielm.uploader");
            await base.IncludeAsync();
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(FileUpload1 model) {
            return RenderContainerAsync(model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderContainerAsync(FileUpload1 model) {

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
    <div class='t_progressbar'></div>");

            if (!IsContainerComponent) {
                hb.Append($@"
    <input type='hidden' name='{FieldName}' style='display:none' />");
            }

            hb.Append($@"
    <input type='file' name='__filename' class='t_filename' style='display:none' />
</div>");

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.FileUpload1Component('{ControlId}', {Utility.JsonSerialize(setup)});");

            return Task.FromResult(hb.ToString());
        }
    }
}
