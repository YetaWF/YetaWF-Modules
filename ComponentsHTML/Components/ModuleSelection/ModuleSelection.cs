/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Addons;
using YetaWF.Modules.ComponentsHTML.Controllers;
#if MVC6
#else
using System.Web.Mvc;
#endif

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

        internal string GetModuleLink(Guid? model, bool force = false) {

            if (!force) {
                if (model == null || model == Guid.Empty) return "";
            }
            YTagBuilder tag = new YTagBuilder("a");

            tag.MergeAttribute("href", ModuleDefinition.GetModulePermanentUrl(model ?? Guid.Empty));
            tag.MergeAttribute("target", "_blank");
            tag.MergeAttribute("rel", "nofollow noopener noreferrer");
            tag.Attributes.Add(Basics.CssTooltip, __ResStr("linkTT", "Click to preview the module in a new window - not all modules can be displayed correctly and may require additional parameters"));

            tag.InnerHtml = tag.InnerHtml + ImageHTML.BuildKnownIcon("#ModulePreview", sprites: Info.PredefSpriteIcons);
            return tag.ToString(YTagRenderMode.Normal);
        }
    }

    /// <summary>
    /// Implementation of the ModuleSelection display component.
    /// </summary>
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

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append(@"
<div class='yt_moduleselection t_display'>");

            ModuleDefinition mod = null;
            if (model != null && model != Guid.Empty)
                mod = await ModuleDefinition.LoadAsync((Guid)model, AllowNone: true);

            string modName;
            if (mod == null) {
                if (model != null && model == Guid.Empty)
                    modName = __ResStr("noLinkNone", "(none)");
                else
                    modName = __ResStr("noLink", "(not found - {0})", ((Guid)model).ToString());
            } else {
                Package package = Package.GetPackageFromType(mod.GetType());
                modName = __ResStr("name", "{0} - {1}", package.Name, mod.Name);
            }

            YTagBuilder tag = new YTagBuilder("div");
            tag.AddCssClass("t_select");
            tag.SetInnerText(modName);
            hb.Append(tag.ToString(YTagRenderMode.Normal));

            if (mod != null) {
                tag = new YTagBuilder("div");
                tag.AddCssClass("t_link");
                tag.InnerHtml = GetModuleLink(model);
                hb.Append(tag.ToString(YTagRenderMode.Normal));
            }

            tag = new YTagBuilder("div");
            tag.AddCssClass("t_description");
            if (mod == null)
                tag.InnerHtml = "&nbsp;";
            else
                tag.SetInnerText(mod.Description.ToString());
            hb.Append(tag.ToString(YTagRenderMode.Normal));

            hb.Append(@"
</div>");
            return hb.ToString();
        }
    }

    /// <summary>
    /// Implementation of the ModuleSelection edit component.
    /// </summary>
    public class ModuleSelectionEditComponent : ModuleSelectionComponentBase, IYetaWFComponent<Guid?> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        internal class ModuleSelectionUINew {
            [UIHint("ModuleSelectionPackageNew"), Caption("Packages"), Description("Select one of the installed packages to list all available modules for the package")]
            public Guid? Package { get; set; }
            [Caption("Module"), Description("Select one of the available modules"), AdditionalMetadata("Disable1OrLess", false)]
            public Guid? Module { get; set; }
        }
        internal class ModuleSelectionUIExisting {
            [UIHint("ModuleSelectionPackageExisting"), Caption("Packages"), Description("Select one of the installed packages to list all available modules for the package")]
            public Guid? Package { get; set; }
            [Caption("Module"), Description("Select one of the available modules"), AdditionalMetadata("Disable1OrLess", false)]
            public Guid? Module { get; set; }
        }

        internal class Setup {
            public string AjaxUrl { get; set; }
            public string AjaxUrlComplete { get; set; }
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(Guid? model) {

            HtmlBuilder hb = new HtmlBuilder();

            bool newMods = PropData.GetAdditionalAttributeValue("New", false);

            ModuleSelectionUINew uiNew = null;
            ModuleSelectionUIExisting uiExisting = null;
            if (newMods) {
                uiNew = new ModuleSelectionUINew {
                    Package = model,
                    Module = model,
                };
            } else {
                uiExisting = new ModuleSelectionUIExisting {
                    Package = model,
                    Module = model,
                };
            }
            Setup setup = new Setup {
                AjaxUrl = YetaWFManager.UrlFor(typeof(ModuleSelectionController), newMods ? nameof(ModuleSelectionController.GetPackageModulesNew) : nameof(ModuleSelectionController.GetPackageModulesDesigned)),
                AjaxUrlComplete = YetaWFManager.UrlFor(typeof(ModuleSelectionController), nameof(ModuleSelectionController.GetPackageModulesDesignedFromGuid)),
            };

            hb.Append($@"
<div id='{DivId}' class='yt_moduleselection t_edit'>");

            using (Manager.StartNestedComponent(FieldName)) {

                if (newMods) {

                    hb.Append($@"
    <div class='t_packages'>
        {await HtmlHelper.ForLabelAsync(uiNew, nameof(uiNew.Package))}
        {await HtmlHelper.ForEditAsync(uiNew, nameof(uiNew.Package))}
    </div>");

                } else {

                    hb.Append($@"

    <div class='t_packages'>
        {await HtmlHelper.ForLabelAsync(uiExisting, nameof(uiExisting.Package))}
        {await HtmlHelper.ForEditAsync(uiExisting, nameof(uiExisting.Package))}
    </div>");
                }

            }

            hb.Append($@"
    <div class='t_select'>");

            if (newMods) {

                hb.Append($@"
        {await HtmlHelper.ForLabelAsync(uiNew, nameof(uiNew.Module))}
        {await HtmlHelper.ForEditAsAsync(uiNew, nameof(uiNew.Module), FieldName, uiNew, nameof(uiNew.Module), model, "ModuleSelectionModuleNew", HtmlAttributes: HtmlAttributes)}");

            } else {

                hb.Append($@"
        {await HtmlHelper.ForLabelAsync(uiExisting, nameof(uiExisting.Module))}
        {await HtmlHelper.ForEditAsAsync(uiExisting, nameof(uiExisting.Module), FieldName, uiExisting, nameof(uiExisting.Module), model, "ModuleSelectionModuleExisting", HtmlAttributes: HtmlAttributes)}");

            }

            hb.Append($@"
    </div>
    <div class='t_link'>
        {GetModuleLink(model, force: true)}
    </div>
    <div class='t_description'>
    </div>
</div>");

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.ModuleSelectionEditComponent('{DivId}', {YetaWFManager.JsonSerialize(setup)});");

            return hb.ToString();
        }
    }
}
