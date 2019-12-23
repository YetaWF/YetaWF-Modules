/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract partial class PropertyListComponentBase {

        internal class PropertyListEntry {

            public PropertyListEntry(string name, object value, string uiHint, bool editable, bool restricted, string textAbove, string textBelow, bool suppressEmpty,
                    List<ExprAttribute> exprAttrs,
                    SubmitFormOnChangeAttribute.SubmitTypeEnum submit) {
                Name = name; Value = value; Editable = editable;
                Restricted = restricted;
                TextAbove = textAbove;
                TextBelow = textBelow;
                UIHint = uiHint;
                ExprAttrs = exprAttrs;
                SuppressEmpty = suppressEmpty;
                SubmitType = submit;
            }
            public object Value { get; private set; }
            public string Name { get; private set; }
            public string TextAbove { get; private set; }
            public string TextBelow { get; private set; }
            public bool Editable { get; private set; }
            public bool Restricted { get; private set; }
            public string UIHint { get; private set; }
            public bool SuppressEmpty { get; private set; }
            public SubmitFormOnChangeAttribute.SubmitTypeEnum SubmitType { get; private set; }
            public List<ExprAttribute> ExprAttrs { get; set; }
        };

        // returns all properties for an object that have a description, in sorted order
        private IEnumerable<PropertyData> GetProperties(Type objType) {
            return from property in ObjectSupport.GetPropertyData(objType)
                    where property.Description != null  // This means it has to be a DescriptionAttribute (not a resource redirect)
                    orderby property.Order
                    select property;
        }

        internal static List<PropertyListEntry> GetHiddenProperties(object obj) {
            List<PropertyListEntry> properties = new List<PropertyListEntry>();
            List<PropertyData> props = ObjectSupport.GetPropertyData(obj.GetType());
            foreach (var prop in props) {
                if (!prop.PropInfo.CanRead) continue;
                if (prop.UIHint != "Hidden")
                    continue;
                properties.Add(new PropertyListEntry(prop.Name, prop.GetPropertyValue<object>(obj), "Hidden", false, false, null, null, false, null, SubmitFormOnChangeAttribute.SubmitTypeEnum.None));
            }
            return properties;
        }

        internal static async Task<PropertyList.PropertyListSetup> GetPropertyListSetupAsync(object obj, List<string> categories) {

            Type setupType = obj.GetType();
            if (obj as ModuleDefinition != null)
                setupType = typeof(ModuleDefinition);

            Type objType = obj.GetType();
            PropertyList.PropertyListSetup setup = await PropertyList.LoadPropertyListDefinitionsAsync(setupType);
            if (setup.ExplicitDefinitions) {
                // Invoke __PropertyListSetupAsync
                MethodInfo miAsync = objType.GetMethod("__PropertyListSetupAsync", new Type[] { typeof(PropertyList.PropertyListSetup) });
                if (miAsync != null) {
                    Task methRetvalTask = (Task)miAsync.Invoke(obj, new object[] { setup });
                    await methRetvalTask;
                }

                // sanity checking
                if (YetaWFManager.DiagnosticsMode) {
                    // verify expandable list
                    foreach (string cat in setup.ExpandableList) {
                        if (!categories.Contains(cat))
                            throw new InternalError($"Unknown category {cat} is used in {nameof(PropertyList.PropertyListSetup.ExpandableList)} for {objType.FullName}");
                    }
                    // verify initial expanded
                    if (setup.InitialExpanded != null) {
                        if (!categories.Contains(setup.InitialExpanded))
                            throw new InternalError($"Unknown category {setup.InitialExpanded} is used in {nameof(PropertyList.PropertyListSetup.InitialExpanded)} for {objType.FullName}");
                    }
                    // verify styles
                    int startWidth = 0;
                    foreach (PropertyList.PropertyListColumnDef colDef in setup.ColumnStyles) {
                        if (colDef.MinWindowSize < startWidth)
                            throw new InternalError($"Column styles in {nameof(PropertyList.PropertyListSetup.ColumnStyles)} are not in ascending order, entry with {nameof(PropertyList.PropertyListColumnDef.MinWindowSize)} = {colDef.MinWindowSize} is out of order");
                        startWidth = colDef.MinWindowSize;
                    }
                    // verify order
                    foreach (string cat in setup.CategoryOrder) {
                        if (!categories.Contains(cat))
                            throw new InternalError($"Unknown category {cat} is used in {nameof(PropertyList.PropertyListSetup.ExpandableList)} for {objType.FullName}");
                    }
                    // verify order
                    List<string> missing = categories.ToList();
                    foreach (string cat in setup.CategoryOrder) {
                        if (!categories.Contains(cat))
                            throw new InternalError($"Category {cat} listed in category order doesn't exist");
                        missing.Remove(cat);
                    }
                    setup.CategoryOrder.AddRange(missing);//add remaining categories to list
                }
            } else {
                setup.CategoryOrder = categories;
            }
            return setup;
        }

        // Returns all categories implemented by this object - these are decorated with the [CategoryAttribute]
        internal List<string> GetCategories(object obj) {

            // get all properties that are shown
            Type objType = obj.GetType();
            List<PropertyData> props = GetProperties(objType).ToList();

            // get the list of categories
            List<string> categories = new List<string>();
            foreach (PropertyData prop in props)
                categories.AddRange(prop.Categories);
            categories = categories.Distinct().ToList();

            // order (if there is a CategoryOrder property)
            PropertyInfo piCat = ObjectSupport.TryGetProperty(objType, "CategoryOrder");
            if (piCat != null) {
                List<string> orderedCategories = (List<string>)piCat.GetValue(obj);
                List<string> allCategories = new List<string>();
                // verify that all returned categories in the list of ordered categories actually exist
                foreach (var oCat in orderedCategories) {
                    if (categories.Contains(oCat))
                        allCategories.Add(oCat);
                    //else
                    //throw new InternalError("No properties exist in category {0} found in CategoryOrder for type {1}.", oCat, obj.GetType().Name);
                }
                // if any are missing, add them to the end of the list
                foreach (var cat in categories) {
                    if (!allCategories.Contains(cat))
                        allCategories.Add(cat);
                }
                categories = allCategories;
            }
            return categories;
        }
        internal List<PropertyListEntry> GetPropertiesByCategory(object obj, string category) {

            List<PropertyListEntry> properties = new List<PropertyListEntry>();
            Type objType = obj.GetType();
            var props = GetProperties(objType);
            foreach (var prop in props) {
                if (!string.IsNullOrWhiteSpace(category) && !prop.Categories.Contains(category))
                    continue;

                if (ExprAttribute.IsSuppressed(prop.ExprValidationAttributes, obj))
                    continue;// suppress this as requested

                bool editable = prop.PropInfo.CanWrite;
                if (editable) {
                    if (prop.ReadOnly)
                        editable = false;
                }
                SuppressEmptyAttribute suppressEmptyAttr = null;
                suppressEmptyAttr = prop.TryGetAttribute<SuppressEmptyAttribute>();

                SubmitFormOnChangeAttribute submitFormOnChangeAttr = null;
                submitFormOnChangeAttr = prop.TryGetAttribute<SubmitFormOnChangeAttribute>();

                bool restricted = false;
                if (YetaWFManager.IsDemo) {
                    ExcludeDemoModeAttribute exclDemoAttr = prop.TryGetAttribute<ExcludeDemoModeAttribute>();
                    if (exclDemoAttr != null)
                        restricted = true;
                }
                properties.Add(
                    new PropertyListEntry(prop.Name, prop.GetPropertyValue<object>(obj), prop.UIHint, editable, restricted, prop.TextAbove, prop.TextBelow,
                        suppressEmptyAttr != null, prop.ExprValidationAttributes,
                        submitFormOnChangeAttr != null ? submitFormOnChangeAttr.Value : SubmitFormOnChangeAttribute.SubmitTypeEnum.None)
                );
            }
            return properties;
        }
    }
}