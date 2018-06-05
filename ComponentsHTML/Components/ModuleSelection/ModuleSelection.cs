﻿using System;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class ModuleSelectionComponentBase : YetaWFComponent {

        public const string TemplateName = "ModuleSelection";
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        protected async Task<string> GetModuleLink(Guid model) {

            YTagBuilder tag = new YTagBuilder("a");

            tag.MergeAttribute("href", ModuleDefinition.GetModulePermanentUrl(model));
            tag.MergeAttribute("target", "_blank");
            tag.MergeAttribute("rel", "nofollow noopener noreferrer");
            tag.Attributes.Add(Basics.CssTooltip, this.__ResStr("linkTT", "Click to preview the module in a new window - not all modules can be displayed correctly and may require additional parameters"));

            // image
            SkinImages skinImages = new SkinImages();
            string imageUrl = await skinImages.FindIcon_TemplateAsync("ModulePreview.png", Package, "ModuleSelection");
            YTagBuilder tagImg = ImageHTML.BuildKnownImageYTag(imageUrl, alt: this.__ResStr("linkAlt", "Preview"));

            tag.InnerHtml = tag.InnerHtml + tagImg.ToString(YTagRenderMode.StartTag);
            return tag.ToString(YTagRenderMode.Normal);
        }
    }

    public class ModuleSelectionDisplayComponent : ModuleSelectionComponentBase, IYetaWFComponent<Guid> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(Guid model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append(@"
<div class='yt_moduleselection t_display'>");

            ModuleDefinition mod = null;
            if (model != Guid.Empty)
                mod = await ModuleDefinition.LoadAsync(model, AllowNone: true);

            string modName;
            if (mod == null) {
                if (model == Guid.Empty)
                    modName = this.__ResStr("noLinkNone", "(none)");
                else
                    modName = this.__ResStr("noLink", "(not found - {0})", model.ToString());
            } else {
                Package package = Package.GetPackageFromType(mod.GetType());
                modName = this.__ResStr("name", "{0} - {1}", package.Name, mod.Name);
            }

            YTagBuilder tag = new YTagBuilder("div");
            tag.AddCssClass("t_select");
            tag.SetInnerText(modName);
            hb.Append(tag.ToString(YTagRenderMode.Normal));

            if (mod != null) {
                tag = new YTagBuilder("div");
                tag.AddCssClass("t_link");
                tag.InnerHtml = await GetModuleLink(model);
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

    public class ModuleSelectionEditComponent : ModuleSelectionComponentBase, IYetaWFComponent<Guid> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class ModuleSelectionUINew {

            [UIHint("Hidden")]
            public string AjaxUrl { get; set; }
            [UIHint("Hidden")]
            public string AjaxUrlComplete { get; set; }

            [UIHint("ModuleSelectionPackageNew"), Caption("Packages"), Description("Select one of the installed packages to list all available modules for the package")]
            public Guid Package { get; set; }
            [Caption("Module"), Description("Select one of the available modules")]
            public Guid Module { get; set; }
        }
        public class ModuleSelectionUIExisting {

            [UIHint("Hidden")]
            public string AjaxUrl { get; set; }
            [UIHint("Hidden")]
            public string AjaxUrlComplete { get; set; }

            [UIHint("ModuleSelectionPackageExisting"), Caption("Packages"), Description("Select one of the installed packages to list all available modules for the package")]
            public Guid Package { get; set; }
            [Caption("Module"), Description("Select one of the available modules")]
            public Guid Module { get; set; }
        }

        public async Task<YHtmlString> RenderAsync(Guid model) {

            HtmlBuilder hb = new HtmlBuilder();

            bool newMods = PropData.GetAdditionalAttributeValue("New", false);

            ModuleSelectionUINew uiNew = null;
            ModuleSelectionUIExisting uiExisting = null;
            if (newMods) {
                uiNew = new ModuleSelectionUINew {
                    AjaxUrl = YetaWFManager.UrlFor(typeof(ModuleSelectionController), newMods ? nameof(ModuleSelectionController.GetPackageModulesNew) : nameof(ModuleSelectionController.GetPackageModulesDesigned)),
                    AjaxUrlComplete = YetaWFManager.UrlFor(typeof(ModuleSelectionController), nameof(ModuleSelectionController.GetPackageModulesDesignedFromGuid)),
                    Package = model,
                    Module = model,
                };
            } else {
                uiExisting = new ModuleSelectionUIExisting {
                    AjaxUrl = YetaWFManager.UrlFor(typeof(ModuleSelectionController), newMods ? nameof(ModuleSelectionController.GetPackageModulesNew) : nameof(ModuleSelectionController.GetPackageModulesDesigned)),
                    AjaxUrlComplete = YetaWFManager.UrlFor(typeof(ModuleSelectionController), nameof(ModuleSelectionController.GetPackageModulesDesignedFromGuid)),
                    Package = model,
                    Module = model,
                };
            }

            hb.Append($@"
<div id='{DivId}' class='yt_moduleselection t_edit' data-name='{FieldName}'>");

            using (Manager.StartNestedComponent(FieldName)) {

                if (newMods) {
                    hb.Append(await HtmlHelper.ForDisplayAsync(uiNew, nameof(uiNew.AjaxUrl)));
                    hb.Append($@"
    <div class='t_packages'>");
                    hb.Append(await HtmlHelper.ForLabelAsync(uiNew, nameof(uiNew.Package)));
                    hb.Append(await HtmlHelper.ForEditAsync(uiNew, nameof(uiNew.Package)));
                } else {
                    hb.Append(await HtmlHelper.ForDisplayAsync(uiExisting, nameof(uiExisting.AjaxUrl)));
                    hb.Append(await HtmlHelper.ForDisplayAsync(uiExisting, nameof(uiExisting.AjaxUrlComplete)));
                    hb.Append($@"
    <div class='t_packages'>");
                    hb.Append(await HtmlHelper.ForLabelAsync(uiExisting, nameof(uiExisting.Package)));
                    hb.Append(await HtmlHelper.ForEditAsync(uiExisting, nameof(uiExisting.Package)));
                }
            }

            hb.Append($@"
    </div>
    <div class='t_select'>");

            if (newMods) {
                hb.Append(await HtmlHelper.ForLabelAsync(uiNew, nameof(uiNew.Module)));
                hb.Append(await HtmlHelper.ForEditComponentAsync(Container, PropertyName, model, "ModuleSelectionModuleNew", HtmlAttributes: HtmlAttributes, Validation: Validation));
            } else {
                hb.Append(await HtmlHelper.ForLabelAsync(uiExisting, nameof(uiExisting.Module)));
                hb.Append(await HtmlHelper.ForEditComponentAsync(Container, PropertyName, model, "ModuleSelectionModuleExisting", HtmlAttributes: HtmlAttributes, Validation: Validation));
            }

            hb.Append($@"
    </div>
    <div class='t_link'>
        {await GetModuleLink(model)}
    </div>
    <div class='t_description'>
    </div>
</div>
<script>
    YetaWF_ModuleSelection.init('{DivId}')
</script>");

            return hb.ToYHtmlString();
        }
    }
}