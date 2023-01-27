/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Endpoints;
using YetaWF.Modules.ComponentsHTML.Addons;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the ModuleSelection component implementation.
    /// </summary>
    public abstract class ModuleSelectionComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ModuleSelectionComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "ModuleSelection";

        /// <summary>
        /// Returns the package implementing the component.
        /// </summary>
        /// <returns>Returns the package implementing the component.</returns>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <summary>
        /// Returns the component name.
        /// </summary>
        /// <returns>Returns the component name.</returns>
        /// <remarks>Components in packages whose product name starts with "Component" use the exact name returned by GetTemplateName when used in UIHint attributes. These are considered core components.
        /// Components in other packages use the package's area name as a prefix. E.g., the UserId component in the YetaWF.Identity package is named "YetaWF_Identity_UserId" when used in UIHint attributes.
        ///
        /// The GetTemplateName method returns the component name without area name prefix in all cases.</remarks>
        public override string GetTemplateName() { return TemplateName; }

        internal string GetModuleLink(Guid? model, bool force = false) {

            if (!force) {
                if (model == null || model == Guid.Empty) return string.Empty;
            }

            string tt = __ResStr("linkTT", "Click to preview the module in a new window - not all modules can be displayed correctly and may require additional parameters");

            string image = ImageHTML.BuildKnownIcon("#ModulePreview", sprites: Info.PredefSpriteIcons);
            return $"<a href='{ModuleDefinition.GetModulePermanentUrl(model ?? Guid.Empty)}' target='_blank' rel='nofollow noopener noreferrer' {Basics.CssTooltip}='{HAE(tt)}'>{image}</a>";
        }
    }

    /// <summary>
    /// Displays module information based on the model. The model defines the module Guid for which information is displayed.
    /// </summary>
    /// <example>
    /// [Caption("Selected Module"), Description("The current module")]
    /// [UIHint("ModuleSelection"), ReadOnly]
    /// public Guid Module { get; set; }
    /// </example>
    public class ModuleSelectionDisplayComponent : ModuleSelectionComponentBase, IYetaWFComponent<Guid?> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(Guid? model) {

            ModuleDefinition? mod = null;
            if (model != null && model != Guid.Empty)
                mod = await ModuleDefinition.LoadAsync((Guid)model, AllowNone: true);

            string modName;
            if (mod == null) {
                if (model == null || model == Guid.Empty)
                    modName = __ResStr("noLinkNone", "(none)");
                else
                    modName = __ResStr("noLink", "(not found - {0})", ((Guid)model).ToString());
            } else {
                Package package = Package.GetPackageFromType(mod.GetType());
                modName = __ResStr("name", "{0} - {1}", package.Name, mod.Name);
            }

            string modDiv = string.Empty;
            if (mod != null)
                modDiv = $"<div class='t_link'>{GetModuleLink(model)}</div>";

            string desc;
            if (mod == null)
                desc = "&nbsp;";
            else
                desc = HE(mod.Description.ToString());

            return $@"
<div class='yt_moduleselection t_display'>
    <div class='t_select'>{HE(modName)}</div>
    {modDiv}
    <div class='t_description'>{desc}</div>
</div>";

        }
    }

    /// <summary>
    /// Allows selection of a new or existing module using a dropdown list. The model defines the new or existing module Guid.
    /// An entry "(select)" with value Guid.Empty is always inserted as the first item in the dropdown list.
    /// </summary>
    /// <example>
    /// [Caption("New Module"), Description("The new module to be added")]
    /// [UIHint("ModuleSelection"), AdditionalMetadata("New", true), Required]
    /// public Guid SelectedModule { get; set; }
    /// </example>
    [UsesAdditional("New", "bool", "false", "Defines whether selection of new modules (to be created) is possible in which case all available modules are shown in the dropdown list. Otherwise only existing, designed modules are listed.")]
    [UsesAdditional("EditSettings", "bool", "false", "Defines whether a link to edit module settings is available")]
    public class ModuleSelectionEditComponent : ModuleSelectionComponentBase, IYetaWFComponent<Guid?> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        internal class ModuleSelectionUINew {
            [Caption("Packages"), Description("Select one of the installed packages to list all available modules for the package")]
            [UIHint("ModuleSelectionPackageNew")]
            public Guid? Package { get; set; }
            [UIHint("ModuleSelectionModuleNew")]
            [Caption("Module"), Description("Select one of the available modules"), AdditionalMetadata("Disable1OrLess", false)]
            public Guid? Module { get; set; }
        }
        internal class ModuleSelectionUIExisting {
            [Caption("Packages"), Description("Select one of the installed packages to list all available modules for the package")]
            [UIHint("ModuleSelectionPackageExisting")]
            public Guid? Package { get; set; }
            [UIHint("ModuleSelectionModuleExisting")]
            [Caption("Module"), Description("Select one of the available modules"), AdditionalMetadata("Disable1OrLess", false)]
            public Guid? Module { get; set; }
            [UIHint("ModuleAction"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.NormalLinks)]
            public ModuleAction? ModuleAction { get; set; }
        }

        internal class Setup {
            public string AjaxUrl { get; set; } = null!;
            public string AjaxUrlComplete { get; set; } = null!;
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(Guid? model) {

            HtmlBuilder hb = new HtmlBuilder();

            bool newMods = PropData.GetAdditionalAttributeValue("New", false);
            bool useEditMod = false;
            Guid? editGuid = null;

            ModuleSelectionUINew? uiNew = null;
            ModuleSelectionUIExisting? uiExisting = null;
            if (newMods) {
                uiNew = new ModuleSelectionUINew {
                    Package = model,
                    Module = model,
                };
            } else {
                uiExisting = new ModuleSelectionUIExisting {
                    Package = model,
                    Module = model,
                    ModuleAction = null,
                };
            }
            Setup setup = new Setup {
                AjaxUrl = Utility.UrlFor(typeof(ModuleSelectionEndpoints), newMods ? ModuleSelectionEndpoints.GetPackageModulesNew : nameof(ModuleSelectionEndpoints.GetPackageModulesDesigned)),
                AjaxUrlComplete = Utility.UrlFor(typeof(ModuleSelectionEndpoints), nameof(ModuleSelectionEndpoints.GetPackageModulesDesignedFromGuid)),
            };

            hb.Append($@"
<div id='{DivId}' class='yt_moduleselection t_edit'>
    {await HtmlHelper.ForEditComponentAsync(Container, PropertyName, model, "Hidden", HtmlAttributes: new { __NoTemplate = true })}");

            using (Manager.StartNestedComponent(FieldName)) {

                if (newMods) {

                    hb.Append($@"
    <div class='t_packages'>
        {await HtmlHelper.ForLabelAsync(uiNew!, nameof(uiNew.Package))}
        {await HtmlHelper.ForEditAsync(uiNew!, nameof(uiNew.Package))}
    </div>");

                } else {

                    hb.Append($@"

    <div class='t_packages'>
        {await HtmlHelper.ForLabelAsync(uiExisting!, nameof(uiExisting.Package))}
        {await HtmlHelper.ForEditAsync(uiExisting!, nameof(uiExisting.Package))}
    </div>");
                }

                hb.Append($@"
    <div class='t_select'>");

                if (newMods) {

                    hb.Append($@"
        {await HtmlHelper.ForLabelAsync(uiNew!, nameof(uiNew.Module))}
        {await HtmlHelper.ForEditAsync(uiNew!, nameof(uiNew.Module))}");

                } else {

                    hb.Append($@"
        {await HtmlHelper.ForLabelAsync(uiExisting!, nameof(uiExisting.Module))}
        {await HtmlHelper.ForEditAsync(uiExisting!, nameof(uiExisting.Module))}");

                    useEditMod = PropData.GetAdditionalAttributeValue("EditSettings", false);

                }
            }

            hb.Append($@"
    </div>
    <div class='t_link'>
        {GetModuleLink(model, force: true)}
    </div>
    <div class='t_description'>
    </div>");

            if (useEditMod) {
                ModuleDefinition? modSettings = await ModuleDefinition.LoadAsync(Manager.CurrentSite.ModuleEditingServices, AllowNone: true);
                if (modSettings != null) {
                    uiExisting!.ModuleAction = await modSettings.GetModuleActionAsync("SettingsGenerate", editGuid);// force moduleaction

                    hb.Append($@"
    <div class='t_editsettings'>
        {await HtmlHelper.ForDisplayAsync(uiExisting, nameof(uiExisting.ModuleAction))}
    </div>");
                }
            }

            hb.Append($@"
</div>");

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.ModuleSelectionEditComponent('{DivId}', {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }
    }
}
