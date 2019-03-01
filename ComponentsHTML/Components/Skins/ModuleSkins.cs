/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the ModuleSkins component implementation.
    /// </summary>
    public abstract class ModuleSkinsComponentBase : YetaWFComponent {

        internal const string TemplateName = "ModuleSkins";

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
    /// Implementation of the ModuleSkins display component.
    /// </summary>
    public class ModuleSkinsDisplayComponent : ModuleSkinsComponentBase, IYetaWFComponent<SerializableList<SkinDefinition>> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        internal class ModuleSkinUI {

            [UIHint("ModuleSkinName"), ResourceRedirect(nameof(FileNameCaption)), Description("The name of the skin collection")]
            public string FileName { get; set; } // may be null for site default
            public ModuleSkinList FileName_ModuleSkinList { get; set; }
            public string FileNameCaption { get; set; }
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<YHtmlString> RenderAsync(SerializableList<SkinDefinition> model) {

            HtmlBuilder hb = new HtmlBuilder();

            int index = 0;
            SkinAccess skinAccess = new SkinAccess();

            hb.Append($@"
<div id='{DivId}' class='yt_moduleskins t_display'>");

            foreach (SkinCollectionInfo skinDef in (from s in skinAccess.GetAllSkinCollections() orderby s.CollectionDescription select s).ToList()) {
                hb.Append($@"
    <div class='t_collection'>");

                SkinDefinition sd = (from s in model where s.Collection == skinDef.CollectionName select s).FirstOrDefault();
                string fileName = sd?.FileName;

                ModuleSkinUI ms = new ModuleSkinUI {
                    FileName = fileName,
                    FileNameCaption = skinDef.CollectionDescription,
                    FileName_ModuleSkinList = skinAccess.GetAllModuleSkins(skinDef.CollectionName),
                };

                using (Manager.StartNestedComponent($@"{FieldName}[{index}]")) {
                    hb.Append(await HtmlHelper.ForLabelAsync(ms, nameof(ms.FileName)));
                    hb.Append(await HtmlHelper.ForDisplayAsync(ms, nameof(ms.FileName)));
                }
                index++;
                hb.Append($@"
    </div>");
            }

            hb.Append($@"
</div>");

            return hb.ToYHtmlString();
        }
    }

    /// <summary>
    /// Implementation of the ModuleSkins edit component.
    /// </summary>
    public class ModuleSkinsEditComponent : ModuleSkinsComponentBase, IYetaWFComponent<SerializableList<SkinDefinition>> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        internal class ModuleSkinUI {

            [UIHint("Hidden")]
            public string Collection { get; set; }

            [UIHint("ModuleSkinName"), ResourceRedirect(nameof(FileNameCaption)), Description("The name of the skin collection")]
            public string FileName { get; set; } // may be null for site default
            public ModuleSkinList FileName_ModuleSkinList { get; set; }
            public string FileNameCaption { get; set; }
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<YHtmlString> RenderAsync(SerializableList<SkinDefinition> model) {

            HtmlBuilder hb = new HtmlBuilder();

            int index = 0;
            SkinAccess skinAccess = new SkinAccess();

            hb.Append($@"
<div id='{DivId}' class='yt_moduleskins t_edit'>");

            foreach (SkinCollectionInfo skinDef in (from s in skinAccess.GetAllSkinCollections() orderby s.CollectionDescription select s).ToList()) {
                hb.Append($@"
    <div class='t_collection'>");

                SkinDefinition sd = (from s in model where s.Collection == skinDef.CollectionName select s).FirstOrDefault();
                string fileName = sd?.FileName;

                ModuleSkinUI ms = new ModuleSkinUI {
                    Collection = skinDef.CollectionName,
                    FileName = fileName,
                    FileNameCaption = skinDef.CollectionDescription,
                    FileName_ModuleSkinList = skinAccess.GetAllModuleSkins(skinDef.CollectionName),
                };

                using (Manager.StartNestedComponent($@"{FieldName}[{index}]")) {
                    hb.Append(await HtmlHelper.ForDisplayAsync(ms, nameof(ms.Collection)));
                    hb.Append(await HtmlHelper.ForLabelAsync(ms, nameof(ms.FileName)));
                    hb.Append(await HtmlHelper.ForEditAsync(ms, nameof(ms.FileName)));
                    ValidationMessage(HtmlHelper, FieldNamePrefix, nameof(ms.FileName));
                }

                index++;
                hb.Append($@"
    </div>");
            }

            hb.Append($@"
</div>");

            return hb.ToYHtmlString();
        }
    }
}
