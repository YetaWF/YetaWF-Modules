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

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class ModuleSelectionComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ModuleSelectionComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "ModuleSelection";
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        protected string GetModuleLink(Guid? model, bool force = false) {

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

    public class ModuleSelectionDisplayComponent : ModuleSelectionComponentBase, IYetaWFComponent<Guid?> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(Guid? model) {

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
            return hb.ToYHtmlString();
        }
    }

    public class ModuleSelectionEditComponent : ModuleSelectionComponentBase, IYetaWFComponent<Guid?> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class ModuleSelectionUINew {
            [UIHint("ModuleSelectionPackageNew"), Caption("Packages"), Description("Select one of the installed packages to list all available modules for the package")]
            public Guid? Package { get; set; }
            [Caption("Module"), Description("Select one of the available modules"), AdditionalMetadata("Disable1OrLess", false)]
            public Guid? Module { get; set; }
        }
        public class ModuleSelectionUIExisting {
            [UIHint("ModuleSelectionPackageExisting"), Caption("Packages"), Description("Select one of the installed packages to list all available modules for the package")]
            public Guid? Package { get; set; }
            [Caption("Module"), Description("Select one of the available modules"), AdditionalMetadata("Disable1OrLess", false)]
            public Guid? Module { get; set; }
        }

        public class Setup {
            public string AjaxUrl { get; set; }
            public string AjaxUrlComplete { get; set; }
        }

        public async Task<YHtmlString> RenderAsync(Guid? model) {

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
</div>
<script>
    new YetaWF_ComponentsHTML.ModuleSelectionEditComponent('{DivId}', {YetaWFManager.JsonSerialize(setup)});
</script>");

            return hb.ToYHtmlString();
        }
    }
}
