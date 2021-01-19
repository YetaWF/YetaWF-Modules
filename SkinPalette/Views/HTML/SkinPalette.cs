/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SkinPalette#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.SkinPalette.Controllers;
using YetaWF.Modules.SkinPalette.Modules;

namespace YetaWF.Modules.SkinPalette.Views {

    public class EditView : YetaWFView, IYetaWFView<SkinPaletteModule, SkinPaletteModuleController.Model> {

        public const string ViewName = "SkinPalette";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public class Entry {
            public string CSSVarName { get; set; } = null!;
            public string ModelName { get; set; } = null!;
            public string UIHint { get; set; } = null!;
        }

        public class Setup {
            public List<Entry> Properties { get; set; }
            public Setup() { Properties = new List<Entry>(); }
        }

        public async Task<string> RenderViewAsync(SkinPaletteModule module, SkinPaletteModuleController.Model model) {

            await Manager.AddOnManager.AddAddOnNamedAsync(module.AreaName, ViewName);

            HtmlBuilder hb = new HtmlBuilder();

            List<FormButton> buttons = new List<FormButton>();
            buttons.Add(new FormButton() { ButtonType = ButtonTypeEnum.Apply, Text = this.__ResStr("apply", "Apply"), Title = this.__ResStr("applyTT", "Apply all changes to the current skin") });

            hb.Append($@"
<a class='t_palette y_button_outline y_button' {Basics.CssTooltip}='{HAE(this.__ResStr("coll", "Skin Palette - Change skin settings and generate new CSS variables for use in skins"))}' href='javascript: void(0);' rel='nofollow' data-button=''>
    {SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-palette")}
</a>
<div class='t_contents' style='display:none'>
    {await RenderBeginFormAsync()}
        {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await RenderEndFormAsync()}
</div>");
            //{await FormButtonsAsync(buttons)}

            // Build lookup table for component
            Setup setup = new Setup {
                Properties = GetLookupTable(model)
            };
            Manager.ScriptManager.AddLast($@"new YetaWF_SkinPalette.SkinPaletteModule('{module.ModuleHtmlId}', {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }

        private List<Entry> GetLookupTable(SkinPaletteModuleController.Model model) {
            if (_LookupTable == null) {
                List<Entry> list = new List<Entry>();
                List<PropertyData> props = ObjectSupport.GetPropertyData(model.GetType());
                foreach (PropertyData prop in props) {
                    string? caption = prop.Caption;
                    if (caption != null) {
                        if (caption.StartsWith("--") && prop.UIHint != null) {
                            list.Add(new Entry {
                                CSSVarName = caption,
                                ModelName = prop.Name,
                                UIHint = prop.UIHint,
                            });
                        }
                    }
                }
                _LookupTable = list;
            }
            return _LookupTable;
        }
        private static List<Entry>? _LookupTable = null;

        /// <summary>
        /// Renders the view's partial view.
        /// A partial view is the portion of the view between &lt;form&gt; and &lt;/form&gt; tags.
        /// </summary>
        /// <param name="module">The module on behalf of which the partial view is rendered.</param>
        /// <param name="model">The model being rendered by the partial view.</param>
        /// <returns>The HTML representing the partial view.</returns>
        public async Task<string> RenderPartialViewAsync(SkinPaletteModule module, object model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
            return hb.ToString();

        }
    }
}
