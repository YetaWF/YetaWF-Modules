/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract partial class PropertyListComponentBase {

        public class ControlData {
            public string Id { get; set; }

            public List<string> Controls { get; set; }
            public List<Dependent> Dependents { get; set; }

            public ControlData() {
                Dependents = new List<Dependent>();
            }
        }
        public class Dependent {
            public string Prop { get; set; } // Name of property
            public string ControlProp { get; set; } // name of controlling property (ProcIf)
            public bool Disable { get; set; } // defines wheter the control is disabled instead of hidden
            public List<int> IntValues { get; set; }

            public Dependent () {
                IntValues = new List<int>();
            }
        }


        /// <summary>
        /// Generate the control sets based on a model's ProcessIf attributes.
        /// </summary>
        /// <param name="model">The model for which the control set is generated.</param>
        /// <param name="id">The HTML id of the property list.</param>
        /// <returns>The data used client-side to show/hide properties and to enable/disable validation.</returns>
        public ControlData GetControlSets(object model, string id) {

            ControlData cd = new ControlData();
            List<string> selectionControls = new List<string>();

            List<PropertyListEntry> properties = GetPropertiesByCategory(model, null);
            foreach (PropertyListEntry property in properties) {
                if (property.ProcIfAttr != null) {
                    if (!selectionControls.Contains(property.ProcIfAttr.Name))
                        selectionControls.Add(property.ProcIfAttr.Name);
                    Dependent dep = new Dependent {
                        Prop = property.Name,
                        ControlProp = property.ProcIfAttr.Name,
                        Disable = property.ProcIfAttr.Disable,
                    };
                    foreach (object obj in property.ProcIfAttr.Objects) {
                        int i = Convert.ToInt32(obj);
                        dep.IntValues.Add(i);
                    }
                    cd.Dependents.Add(dep);
                }
            }

            if (selectionControls.Count == 0) return null;

            cd.Controls = selectionControls;
            cd.Id = id;

            return cd;
        }
    }
}
