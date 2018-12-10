/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Models.Attributes;

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
            public bool Disable { get; set; } // defines whether the control is disabled instead of hidden

            public List<ValueEntry> Values { get; set; }

            public Dependent () {
                Values = new List<ValueEntry>();
            }

            public class ValueEntry {
                public string ControlProp { get; set; } // name of controlling property
                public enum ValueTypeEnum {
                    EqualIntValue = 0,
                    EqualStringValue = 1,
                    EqualNull = 100,
                    EqualNonNull = 101,
                }
                public ValueTypeEnum ValueType { get; set; }
                public object ValueObject { get; set; }
            }
        }


        /// <summary>
        /// Generate the control sets based on a model's ProcessIfxxx attributes.
        /// </summary>
        /// <param name="model">The model for which the control set is generated.</param>
        /// <param name="id">The HTML id of the property list.</param>
        /// <returns>The data used client-side to show/hide properties and to enable/disable validation.</returns>
        public ControlData GetControlSets(object model, string id) {

            ControlData cd = new ControlData();
            List<string> selectionControls = new List<string>();

            List<PropertyListEntry> properties = GetPropertiesByCategory(model, null);
            foreach (PropertyListEntry property in properties) {
                if (property.ProcIfAttrs != null) {
                    foreach (ProcessIfAttribute procIfAttr in property.ProcIfAttrs) {
                        if (!selectionControls.Contains(procIfAttr.Name))
                            selectionControls.Add(procIfAttr.Name);
                        Dependent dep = FindDependent(cd, property.Name);
                        dep.Disable = procIfAttr.Disable;
                        List<int> intValues = new List<int>();
                        List<string> stringValues = new List<string>();
                        foreach (object obj in procIfAttr.Objects) {
                            if (obj.GetType() == typeof(string)) {
                                stringValues.Add((string)obj);
                            } else {
                                int val = Convert.ToInt32(obj);
                                intValues.Add(val);
                            }
                        }
                        if (intValues.Count > 0) {
                            dep.Values.Add(new Dependent.ValueEntry {
                                ControlProp = procIfAttr.Name,
                                ValueType = Dependent.ValueEntry.ValueTypeEnum.EqualIntValue,
                                ValueObject = intValues,
                            });
                        }
                        if (stringValues.Count > 0) {
                            dep.Values.Add(new Dependent.ValueEntry {
                                ControlProp = procIfAttr.Name,
                                ValueType = Dependent.ValueEntry.ValueTypeEnum.EqualStringValue,
                                ValueObject = stringValues,
                            });
                        }
                    }
                    foreach (ProcessIfSuppliedAttribute procIfSuppliedAttr in property.ProcIfSuppliedAttrs) {
                        if (!selectionControls.Contains(procIfSuppliedAttr.Name))
                            selectionControls.Add(procIfSuppliedAttr.Name);
                        Dependent dep = FindDependent(cd, property.Name);
                        dep.Disable = procIfSuppliedAttr.Disable;
                        dep.Values.Add(new Dependent.ValueEntry {
                            ControlProp = procIfSuppliedAttr.Name,
                            ValueType = Dependent.ValueEntry.ValueTypeEnum.EqualNonNull,
                        });
                    }
                    foreach (ProcessIfNotSuppliedAttribute procIfNotSuppliedAttr in property.ProcIfNotSuppliedAttrs) {
                        if (!selectionControls.Contains(procIfNotSuppliedAttr.Name))
                            selectionControls.Add(procIfNotSuppliedAttr.Name);
                        Dependent dep = FindDependent(cd, property.Name);
                        dep.Disable = procIfNotSuppliedAttr.Disable;
                        dep.Values.Add(new Dependent.ValueEntry {
                            ControlProp = procIfNotSuppliedAttr.Name,
                            ValueType = Dependent.ValueEntry.ValueTypeEnum.EqualNull,
                        });
                    }
                }
            }

            if (selectionControls.Count == 0) return null;

            cd.Controls = selectionControls;
            cd.Id = id;

            return cd;
        }

        private Dependent FindDependent(ControlData cd, string name) {
            Dependent dep = (from d in cd.Dependents where d.Prop == name select d).FirstOrDefault();
            if (dep == null) {
                dep = new Dependent {
                    Prop = name,
                };
                cd.Dependents.Add(dep);
            }
            return dep;
        }
    }
}
