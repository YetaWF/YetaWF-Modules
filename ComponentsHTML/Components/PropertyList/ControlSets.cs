/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract partial class PropertyListComponentBase {

        /// <summary>
        /// Generate the control sets based on a model's ProcessIf attributes.
        /// </summary>
        /// <param name="model">The model for which the control set is generated.</param>
        /// <param name="id">The HTML id of the property list.</param>
        /// <returns>The data used client-side to show/hide properties and to enable/disable validation.</returns>
        public string GetControlSets(object model, string id) {

            List<PropertyListEntry> properties = GetPropertiesByCategory(model, null);
            ScriptBuilder sb = new ScriptBuilder();
            List<string> selectionControls = new List<string>();

            sb.Append("{");
            sb.Append("'Id':{0},", YetaWFManager.JsonSerialize(id));
            sb.Append("'Dependents':[");
            foreach (PropertyListEntry property in properties) {
                if (property.ProcIfAttr != null) {
                    if (!selectionControls.Contains(property.ProcIfAttr.Name))
                        selectionControls.Add(property.ProcIfAttr.Name);
                    sb.Append("{");
                    sb.Append("'Prop':{0},'ControlProp':{1},'Disable':{2},'Values':[",
                        YetaWFManager.JsonSerialize(property.Name), YetaWFManager.JsonSerialize(property.ProcIfAttr.Name), property.ProcIfAttr.Disable ? 1 : 0);
                    foreach (object obj in property.ProcIfAttr.Objects) {
                        int i = Convert.ToInt32(obj);
                        sb.Append("{0},", i);
                    }
                    sb.Append("]},");
                }
            }
            sb.Append("],");

            if (selectionControls.Count == 0) return null;

            sb.Append("'Controls':[");
            foreach (string selectionControl in selectionControls) {
                sb.Append("{0},", YetaWFManager.JsonSerialize(selectionControl));
            }
            sb.Append("],");
            sb.Append("}");
            return sb.ToString();
        }
    }
}
