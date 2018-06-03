using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract partial class PropertyListComponentBase {

        public class PropertyListEntry {

            public PropertyListEntry(string name, object value, string uiHint, bool editable, bool restricted, string textAbove, string textBelow, bool suppressEmpty, ProcessIfAttribute procIfAttr, SubmitFormOnChangeAttribute.SubmitTypeEnum submit) {
                Name = name; Value = value; Editable = editable;
                Restricted = restricted;
                TextAbove = textAbove; TextBelow = textBelow;
                UIHint = uiHint;
                ProcIfAttr = procIfAttr;
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
            public ProcessIfAttribute ProcIfAttr { get; set; }
        };

        // returns all properties for an object that have a description, in sorted order
        private IEnumerable<PropertyData> GetProperties(Type objType) {
            return from property in ObjectSupport.GetPropertyData(objType)
                    where property.Description != null  // This means it has to be a DescriptionAttribute (not a resource redirect)
                    orderby property.Order
                    select property;
        }

        public static List<PropertyListEntry> GetHiddenProperties(object obj) {
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

        // Returns all categories implemented by this object - these are decorated with the [CategoryAttribute]
        public List<string> GetCategories(object obj) {

            // get all properties that are shown
            List<PropertyData> props = GetProperties(obj.GetType()).ToList();

            // get the list of categories
            List<string> categories = new List<string>();
            foreach (PropertyData prop in props)
                categories.AddRange(prop.Categories);
            categories = categories.Distinct().ToList();

            // order (if there is a CategoryOrder property)
            PropertyInfo piCat = ObjectSupport.TryGetProperty(obj.GetType(), "CategoryOrder");
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
        public List<PropertyListEntry> GetPropertiesByCategory(object obj, string category) {

            List<PropertyListEntry> properties = new List<PropertyListEntry>();
            Type objType = obj.GetType();
            var props = GetProperties(objType);
            foreach (var prop in props) {
                if (!string.IsNullOrWhiteSpace(category) && !prop.Categories.Contains(category))
                    continue;
                SuppressIfEqualAttribute supp = prop.TryGetAttribute<SuppressIfEqualAttribute>();
                if (supp != null) { // possibly suppress this property
                    if (supp.IsEqual(obj))
                        continue;// suppress this as requested
                }
                SuppressIfNotEqualAttribute suppn = prop.TryGetAttribute<SuppressIfNotEqualAttribute>();
                if (suppn != null) { // possibly suppress this property
                    if (suppn.IsNotEqual(obj))
                        continue;// suppress this as requested
                }
                bool editable = prop.PropInfo.CanWrite;
                if (editable) {
                    if (prop.ReadOnly)
                        editable = false;
                }
                SuppressEmptyAttribute suppressEmptyAttr = null;
                suppressEmptyAttr = prop.TryGetAttribute<SuppressEmptyAttribute>();

                SubmitFormOnChangeAttribute submitFormOnChangeAttr = null;
                submitFormOnChangeAttr = prop.TryGetAttribute<SubmitFormOnChangeAttribute>();

                ProcessIfAttribute procIfAttr = null;
                procIfAttr = prop.TryGetAttribute<ProcessIfAttribute>();

                bool restricted = false;
                if (Manager.IsDemo) {
                    ExcludeDemoModeAttribute exclDemoAttr = prop.TryGetAttribute<ExcludeDemoModeAttribute>();
                    if (exclDemoAttr != null)
                        restricted = true;
                }
                properties.Add(new PropertyListEntry(prop.Name, prop.GetPropertyValue<object>(obj), prop.UIHint, editable, restricted, prop.TextAbove, prop.TextBelow, suppressEmptyAttr != null, procIfAttr, submitFormOnChangeAttr != null ? submitFormOnChangeAttr.Value : SubmitFormOnChangeAttribute.SubmitTypeEnum.None));
            }
            return properties;
        }
    }
}