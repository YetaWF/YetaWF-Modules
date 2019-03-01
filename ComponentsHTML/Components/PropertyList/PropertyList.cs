/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Implementation of the PropertyList display component.
    /// </summary>
    public partial class PropertyListDisplayComponent : PropertyListComponentBase, IYetaWFContainer<object>, IYetaWFComponent<object> {

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
        public async Task<YHtmlString> RenderContainerAsync(object model) {
            return await RenderPropertyListTabbedAsync(model, true);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<YHtmlString> RenderAsync(object model) {
            using (Manager.StartNestedComponent($"{FieldName}")) {
                return await RenderPropertyListTabbedAsync(model, true);
            }
        }
    }

    /// <summary>
    /// Implementation of the PropertyList edit component.
    /// </summary>
    public partial class PropertyListEditComponent : PropertyListComponentBase, IYetaWFContainer<object>, IYetaWFComponent<object> {

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
        public async Task<YHtmlString> RenderContainerAsync(object model) {
            return await RenderPropertyListTabbedAsync(model, false);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<YHtmlString> RenderAsync(object model) {
            using (Manager.StartNestedComponent($"{FieldName}")) {
                return await RenderPropertyListTabbedAsync(model, false);
            }
        }
    }

    /// <summary>
    /// Base class for the PropertyList component implementation.
    /// </summary>
    public abstract partial class PropertyListComponentBase : YetaWFComponent {

        internal const string TemplateName = "PropertyList";

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

        internal async Task<YHtmlString> RenderPropertyListTabbedAsync(object model, bool readOnly) {

            List<string> categories = GetCategories(model);
            if (categories.Count <= 1) // if there is only one tab, show as regular property list
                return await RenderPropertyListAsync(model, readOnly);

            HtmlBuilder hb = new HtmlBuilder();
            Type modelType = model.GetType();

            ClassData classData = ObjectSupport.GetClassData(modelType);
            RenderHeader(hb, classData);

            string divId = Manager.UniqueId();
            hb.Append($@"
<div id='{divId}' class='yt_propertylisttabbed {(readOnly ? "t_display" : "t_edit")}'>");

            hb.Append(await RenderHiddenAsync(model));

            bool showVariables = YetaWF.Core.Localize.UserSettings.GetProperty<bool>("ShowVariables");

            // tabstrip
            hb.Append(RenderTabStripStart(divId));
            int tabEntry = 0;
            foreach (string category in categories) {
                string cat = category;
                if (classData.Categories.ContainsKey(cat))
                    cat = classData.Categories[cat];
                hb.Append(RenderTabEntry(divId, cat, "", tabEntry));
                ++tabEntry;
            }
            hb.Append(RenderTabStripEnd(divId));

            // panels
            int panel = 0;
            foreach (string category in categories) {
                hb.Append(RenderTabPaneStart(divId, panel));
                hb.Append(await RenderListAsync(model, category, showVariables, readOnly));
                hb.Append(RenderTabPaneEnd(divId, panel));
                ++panel;
            }

            hb.Append($@"
</div>
{await RenderTabInitAsync(divId, model)}");

            RenderFooter(hb, classData);

            if (!readOnly) {
                ControlData cd = GetControlSets(model, divId);
                if (cd != null) {
                    hb.Append($@"
<script>
    new YetaWF_ComponentsHTML.PropertyListComponent('{divId}', {YetaWFManager.JsonSerialize(cd)});
</script>");
                }
            }
            return hb.ToYHtmlString();
        }

        internal async Task<YHtmlString> RenderPropertyListAsync(object model, bool ReadOnly) {

            HtmlBuilder hb = new HtmlBuilder();
            Type modelType = model.GetType();
            ClassData classData = ObjectSupport.GetClassData(modelType);
            RenderHeader(hb, classData);

            bool showVariables = YetaWF.Core.Localize.UserSettings.GetProperty<bool>("ShowVariables");

            // property table
            HtmlBuilder hbProps = new HtmlBuilder();
            string divId = Manager.UniqueId();
            hbProps.Append($@"
<div id='{divId}' class='yt_propertylist t_table {(ReadOnly ? "t_display" : "t_edit")}'>
   {await RenderHiddenAsync(model)}
   {await RenderListAsync(model, null, showVariables, ReadOnly)}
</div>");

            if (!string.IsNullOrWhiteSpace(classData.Legend)) {
                YTagBuilder tagFieldSet = new YTagBuilder("fieldset");
                YTagBuilder tagLegend = new YTagBuilder("legend");
                tagLegend.SetInnerText(classData.Legend);
                tagFieldSet.InnerHtml = tagLegend.ToString(YTagRenderMode.Normal) + hbProps.ToString();
                hb.Append(tagFieldSet.ToString(YTagRenderMode.Normal));
            } else {
                hb.Append(hbProps.ToHtmlString());
            }
            RenderFooter(hb, classData);

            if (!ReadOnly) {
                ControlData cd = GetControlSets(model, divId);
                if (cd != null) {
                    hb.Append($@"
<script>
    new YetaWF_ComponentsHTML.PropertyListComponent('{divId}', {YetaWFManager.JsonSerialize(cd)});
</script>");
                }
            }

            return hb.ToYHtmlString();
        }
    }
}
