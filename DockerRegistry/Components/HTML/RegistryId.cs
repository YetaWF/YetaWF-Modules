/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DockerRegistry#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.DockerRegistry.DataProvider;
using YetaWF.Modules.DockerRegistry.Modules;

namespace YetaWF.Modules.DockerRegistry.Components {

    /// <summary>
    /// Base class for the Registry component implementation.
    /// </summary>
    public abstract class RegistryIdComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(RegistryIdComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "RegistryId";

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
    /// Implementation of the Registry display component.
    /// </summary>
    public class RegistryIdDisplayComponent : RegistryIdComponentBase, IYetaWFComponent<int> {

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
        public async Task<string> RenderAsync(int model) {
            HtmlBuilder hb = new HtmlBuilder();
            YTagBuilder tag = new YTagBuilder("div");
            tag.AddCssClass("yt_softelvdm_dockerregistry_registryid");
            tag.AddCssClass("t_display");
            FieldSetup(tag, FieldType.Anonymous);
            using (RegistryEntryDataProvider regDP = new RegistryEntryDataProvider()) {
                RegistryEntry reg = null;
                if (model != 0)
                    reg = await regDP.GetItemAsync(model);
                string text;
                if (reg != null) {
                    text = reg.RegistryURL;
                } else {
                    text = __ResStr("delt", "(None)");
                }
                tag.SetInnerText(text);
            }
            hb.Append(tag.ToString(YTagRenderMode.Normal));
            return hb.ToString();
        }
    }

    /// <summary>
    /// Implementation of the Registry edit component.
    /// </summary>
    public class RegistryIdEditComponent : RegistryIdComponentBase, IYetaWFComponent<int> {

        internal class RegistryUI {
            public RegistryUI() { }

            [UIHint("DropDownListInt"), SelectionRequired]
            public int RegistryId { get; set; }
            public List<SelectionItem<int>> RegistryId_List { get; set; }

            [UIHint("ModuleAction"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.IconsOnly), ReadOnly]
            public ModuleAction Edit { get; set; }
            [UIHint("ModuleAction"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.IconsOnly), ReadOnly]
            public ModuleAction Add { get; set; }
            [UIHint("ModuleAction"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.IconsOnly), ReadOnly]
            public ModuleAction Remove { get; set; }
        }

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(int model) {
            HtmlBuilder hb = new HtmlBuilder();

            using (RegistryEntryDataProvider regDP = new RegistryEntryDataProvider()) {
                List<DataProviderSortInfo> sort = null;
                sort = DataProviderSortInfo.Join(sort, new DataProviderSortInfo { Field = nameof(RegistryEntry.RegistryName), Order = DataProviderSortInfo.SortDirection.Ascending });
                DataProviderGetRecords<RegistryEntry> regs = await regDP.GetItemsAsync(0, 0, sort, null);
                List<SelectionItem<int>> list = new List<SelectionItem<int>>();
                foreach (RegistryEntry reg in regs.Data) {
                    list.Add(new SelectionItem<int> { Text = reg.RegistryName, Tooltip = reg.RegistryURL, Value = reg.Id });
                }
                AddRegistryEntryModule addMod = new AddRegistryEntryModule();
                EditRegistryEntryModule editMod = new EditRegistryEntryModule();
                RegistryUI regUI = new RegistryUI {
                    RegistryId = model,
                    RegistryId_List = list,
                    Add = addMod.GetAction_Add(),
                    Edit = list.Count == 0 ? null : editMod.GetAction_Edit(model),
                    Remove = list.Count == 0 ? null : addMod.GetAction_Remove(model),
                };

                if (model == 0) {
                    if (list.Count == 0)
                        list.Insert(0, new SelectionItem<int> { Text = __ResStr("none", "(None)"), Value = 0 });
                    else
                        list.Insert(0, new SelectionItem<int> { Text = __ResStr("select", "(Select)"), Value = 0 });
                }

                hb.Append($@"
<div id='{ControlId}' class='yt_softelvdm_dockerregistry_registryid t_edit'>
    {await HtmlHelper.ForEditAsync(regUI, nameof(RegistryUI.RegistryId))}
    {await HtmlHelper.ForDisplayAsync(regUI, nameof(RegistryUI.Edit))}
    {await HtmlHelper.ForDisplayAsync(regUI, nameof(RegistryUI.Add))}
    {await HtmlHelper.ForDisplayAsync(regUI, nameof(RegistryUI.Remove))}
</div>");

            }
            return hb.ToString();
        }
    }
}
