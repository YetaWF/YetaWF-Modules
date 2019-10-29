﻿/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

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
        public async Task<string> RenderContainerAsync(object model) {
            return await RenderPropertyListTabbedAsync(model, true);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(object model) {
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
        public async Task<string> RenderContainerAsync(object model) {
            return await RenderPropertyListTabbedAsync(model, false);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(object model) {
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

        internal async Task<string> RenderPropertyListTabbedAsync(object model, bool readOnly) {

            List<string> categories = GetCategories(model);
            if (categories.Count <= 1) // if there is only one category, show as regular property list
                return await RenderPropertyListAsync(model, readOnly);

            PropertyList.PropertyListSetup setup = await PropertyListComponentBase.GetPropertyListSetupAsync(model, categories);
            categories = setup.CategoryOrder;

            HtmlBuilder hb = new HtmlBuilder();
            Type modelType = model.GetType();

            ClassData classData = ObjectSupport.GetClassData(modelType);
            RenderHeader(hb, classData);

            string divId = Manager.UniqueId();
            switch (setup.Style) {
                default:
                case PropertyList.PropertyListStyleEnum.Tabbed:
                    hb.Append($@"
<div id='{divId}' class='yt_propertylist t_tabbed {(readOnly ? "t_display" : "t_edit")}'>");
                    break;
                case PropertyList.PropertyListStyleEnum.Boxed:
                    await Manager.AddOnManager.AddAddOnNamedAsync(AreaRegistration.CurrentPackage.AreaName, "masonry.desandro.com");
                    hb.Append($@"
<div id='{divId}' class='yt_propertylist t_boxed {(readOnly ? "t_display" : "t_edit")}'>");
                    break;
                case PropertyList.PropertyListStyleEnum.BoxedWithCategories:
                    await Manager.AddOnManager.AddAddOnNamedAsync(AreaRegistration.CurrentPackage.AreaName, "masonry.desandro.com");
                    hb.Append($@"
<div id='{divId}' class='yt_propertylist t_boxedcat {(readOnly ? "t_display" : "t_edit")}'>");
                    break;
            }

            hb.Append(await RenderHiddenAsync(model));

            bool showVariables = YetaWF.Core.Localize.UserSettings.GetProperty<bool>("ShowVariables");

            switch (setup.Style) {
                default:
                case PropertyList.PropertyListStyleEnum.Tabbed:
                    // tabstrip
                    hb.Append(RenderTabStripStart(divId));
                    break;
                case PropertyList.PropertyListStyleEnum.BoxedWithCategories:
                case PropertyList.PropertyListStyleEnum.Boxed:
                    break;
            }
            int tabEntry = 0;
            foreach (string category in categories) {
                string cat = category;
                if (classData.Categories.ContainsKey(cat))
                    cat = classData.Categories[cat];
                switch (setup.Style) {
                    default:
                    case PropertyList.PropertyListStyleEnum.Tabbed:
                        hb.Append(RenderTabEntry(divId, cat, "", tabEntry));
                        break;
                    case PropertyList.PropertyListStyleEnum.BoxedWithCategories:
                    case PropertyList.PropertyListStyleEnum.Boxed:
                        break;
                }
                ++tabEntry;
            }
            switch (setup.Style) {
                default:
                case PropertyList.PropertyListStyleEnum.Tabbed:
                    hb.Append(RenderTabStripEnd(divId));
                    break;
                case PropertyList.PropertyListStyleEnum.BoxedWithCategories:
                case PropertyList.PropertyListStyleEnum.Boxed:
                    break;
            }

            // panels
            int panel = 0;
            foreach (string category in categories) {
                switch (setup.Style) {
                    default:
                    case PropertyList.PropertyListStyleEnum.Tabbed:
                        hb.Append(RenderTabPaneStart(divId, panel));
                        hb.Append(await RenderListAsync(model, category, showVariables, readOnly));
                        hb.Append(RenderTabPaneEnd(divId, panel));
                        break;
                    case PropertyList.PropertyListStyleEnum.BoxedWithCategories: {
                            string stat = "";
                            if (setup.ExpandableList.Contains(category))
                                stat = (setup.InitialExpanded == category) ? " t_propexpandable t_propexpanded" : " t_propexpandable t_propcollapsed";
                            hb.Append($"<div class='t_proptable{stat} t_cat t_boxpanel-{GetCategoryNormalized(category)}'>");
                            hb.Append($"<div class='t_boxlabel'>{category}</div>");
                            if (setup != null && setup.ExpandableList.Contains(category))
                                hb.Append($"<div class='t_boxexpcoll t_show'></div>");
                            hb.Append(await RenderListAsync(model, category, showVariables, readOnly));
                            hb.Append($"</div>");
                            break;
                        }
                    case PropertyList.PropertyListStyleEnum.Boxed: {
                            string stat = "";
                            if (setup.ExpandableList.Contains(category))
                                stat = (setup.InitialExpanded == category) ? " t_propexpandable t_propexpanded" : " t_propexpandable t_propcollapsed";
                            hb.Append($"<div class='t_proptable {stat} t_cat t_boxpanel-{GetCategoryNormalized(category)}'>");
                            if (setup != null && setup.ExpandableList.Contains(category))
                                hb.Append($"<div class='t_boxexpcoll t_show'></div>");
                            hb.Append(await RenderListAsync(model, category, showVariables, readOnly));
                            hb.Append($"</div>");
                        }
                        break;
                }
                ++panel;
            }

            switch (setup.Style) {
                default:
                case PropertyList.PropertyListStyleEnum.Tabbed:
                    hb.Append($@"
</div>
{await RenderTabInitAsync(divId, model)}");
                    break;
                case PropertyList.PropertyListStyleEnum.BoxedWithCategories:
                case PropertyList.PropertyListStyleEnum.Boxed:
                    hb.Append($@"
</div>");

                    break;
            }

            RenderFooter(hb, classData);

            ControlData cd = null;
            if (!readOnly)
                cd = GetControlSets(model, divId);
            if (setup.ExpandableList != null) {
                // normalize category names for javascript
                setup.ExpandableList = (from l in setup.ExpandableList select GetCategoryNormalized(l)).ToList();
            }
            if (setup.InitialExpanded != null)
                setup.InitialExpanded = GetCategoryNormalized(setup.InitialExpanded);
            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.PropertyListComponent('{divId}', {Utility.JsonSerialize(setup)}, {Utility.JsonSerialize(cd)});");
            return hb.ToString();
        }

        private string GetCategoryNormalized(string category) {
            string cat = reCategory.Replace(category, "");
            return cat.ToLower();
        }
        Regex reCategory = new Regex("[^0-9A-Za-z]", RegexOptions.Compiled);

        internal async Task<string> RenderPropertyListAsync(object model, bool ReadOnly) {

            HtmlBuilder hb = new HtmlBuilder();
            Type modelType = model.GetType();
            ClassData classData = ObjectSupport.GetClassData(modelType);
            RenderHeader(hb, classData);

            bool showVariables = YetaWF.Core.Localize.UserSettings.GetProperty<bool>("ShowVariables");

            // property table
            HtmlBuilder hbProps = new HtmlBuilder();
            string divId = Manager.UniqueId();
            hbProps.Append($@"
<div id='{divId}' class='yt_propertylist {(ReadOnly ? "t_display" : "t_edit")}'>
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
                hb.Append(hbProps.ToString());
            }
            RenderFooter(hb, classData);

            ControlData cd = GetControlSets(model, divId);
            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.PropertyListComponent('{divId}', {Utility.JsonSerialize(new PropertyList.PropertyListSetup())}, {Utility.JsonSerialize(cd)});");

            return hb.ToString();
        }
    }
}