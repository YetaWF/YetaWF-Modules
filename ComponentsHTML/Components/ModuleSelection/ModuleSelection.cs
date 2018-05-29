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
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class ModuleSelectionComponentBase : YetaWFComponent {

        public const string TemplateName = "ModuleSelection";
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    //public class ModuleSelectionDisplayComponent : ModuleSelectionComponentBase, IYetaWFComponent<string> {

    //    public override ComponentType GetComponentType() { return ComponentType.Display; }

    //    public async Task<YHtmlString> RenderAsync(string model) {

    //        HtmlBuilder hb = new HtmlBuilder();

    //        hb.Append("<div class='yt_url t_display'>");

    //        string hrefUrl;
    //        if (!TryGetSiblingProperty($"{PropertyName}_Url", out hrefUrl))
    //            hrefUrl = model;

    //        if (string.IsNullOrWhiteSpace(hrefUrl)) {
    //            // no link
    //            YTagBuilder tag = new YTagBuilder("span");
    //            FieldSetup(tag, FieldType.Anonymous);

    //            string cssClass = PropData.GetAdditionalAttributeValue("CssClass", "");
    //            if (!string.IsNullOrWhiteSpace(cssClass))
    //                tag.AddCssClass(Manager.AddOnManager.CheckInvokedCssModule(cssClass));

    //            tag.SetInnerText(model);
    //            hb.Append(tag.ToString(YTagRenderMode.Normal));

    //        } else {
    //            // link
    //            YTagBuilder tag = new YTagBuilder("a");
    //            FieldSetup(tag, FieldType.Anonymous);

    //            string cssClass = PropData.GetAdditionalAttributeValue("CssClass", "");
    //            if (!string.IsNullOrWhiteSpace(cssClass))
    //                tag.AddCssClass(Manager.AddOnManager.CheckInvokedCssModule(cssClass));

    //            tag.MergeAttribute("href", hrefUrl);
    //            tag.MergeAttribute("target", "_blank");
    //            tag.MergeAttribute("rel", "nofollow noopener noreferrer");
    //            string text;
    //            if (!TryGetSiblingProperty($"{PropertyName}_Text", out text))
    //                text = model;
    //            tag.SetInnerText(text);

    //            // image
    //            Package currentPackage = YetaWF.Core.Controllers.AreaRegistration.CurrentPackage;
    //            SkinImages skinImages = new SkinImages();
    //            string imageUrl = await skinImages.FindIcon_TemplateAsync("UrlRemote.png", currentPackage, "Url");
    //            YTagBuilder tagImg = ImageHelper.BuildKnownImageYTag(imageUrl, alt: this.__ResStr("altText", "Remote Url"));

    //            tag.InnerHtml = tag.InnerHtml + tagImg.ToString(YTagRenderMode.StartTag);
    //            hb.Append(tag.ToString(YTagRenderMode.Normal));
    //        }
    //        hb.Append("</div>");
    //        return hb.ToYHtmlString();
    //    }
    //}
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

            // link
            YTagBuilder tag = new YTagBuilder("a");

            tag.MergeAttribute("href", ModuleDefinition.GetModulePermanentUrl(model));
            tag.MergeAttribute("target", "_blank");
            tag.MergeAttribute("rel", "nofollow noopener noreferrer");
            tag.Attributes.Add(Basics.CssTooltip, this.__ResStr("linkTT", "Click to preview the module in a new window - not all modules can be displayed correctly and may require additional parameters"));

            // image
            SkinImages skinImages = new SkinImages();
            string imageUrl = await skinImages.FindIcon_TemplateAsync("ModulePreview.png", Package, "ModuleSelection");
            YTagBuilder tagImg = ImageHelper.BuildKnownImageYTag(imageUrl, alt: this.__ResStr("linkAlt", "Preview"));

            tag.InnerHtml = tag.InnerHtml + tagImg.ToString(YTagRenderMode.StartTag);
            string link = tag.ToString(YTagRenderMode.Normal);

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
        {link}
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
