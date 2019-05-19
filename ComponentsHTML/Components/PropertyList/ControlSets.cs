/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Models.Attributes;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract partial class PropertyListComponentBase {

        internal class ControlData {
            public string Id { get; set; }

            /// <summary>
            /// The controls in this collection affect other controls when changed.
            /// </summary>
            public List<string> Controls { get; set; }
            /// <summary>
            /// The controls in this collection are affected by controls in the Controls collection.
            /// </summary>
            public List<Dependent> Dependents { get; set; }

            public ControlData() {
                Dependents = new List<Dependent>();
            }
        }
        internal class Dependent {

            public string Prop { get; set; } // Name of property

            public List<ExprEntry> ProcessValues { get; set; } // If an entry matching all conditions is found the dependent property is processed
            public List<ExprEntry> HideValues { get; set; } // If an entry matching all conditions is found the dependent property is hidden

            public Dependent () {
                ProcessValues = new List<ExprEntry>();
                HideValues = new List<ExprEntry>();
            }
        }
        internal class ExprEntry {
            public ExprAttribute.OpEnum Op { get; set; }
            public bool Disable { get; set; }
            public List<ExprAttribute.Expr> ExprList { get; set; }
        }

        /// <summary>
        /// Generate the control sets based on a model's ProcessIfxxx attributes.
        /// </summary>
        /// <param name="model">The model for which the control set is generated.</param>
        /// <param name="id">The HTML id of the property list.</param>
        /// <returns>The data used client-side to show/hide properties and to enable/disable validation.</returns>
        internal ControlData GetControlSets(object model, string id) {

            ControlData cd = new ControlData();
            List<string> selectionControls = new List<string>();

            List<PropertyListEntry> properties = GetPropertiesByCategory(model, null);
            foreach (PropertyListEntry property in properties) {

                // Process
                if (property.ExprAttrs != null) {
                    foreach (ExprAttribute exprAttr in property.ExprAttrs) {
                        if (exprAttr.IsProcessAttribute || exprAttr.IsHideAttribute) {
                            if (exprAttr.ExprList != null && exprAttr.ExprList.Count > 0) {
                                Dependent dep = FindDependent(cd, property.Name);
                                ExprEntry exprEntry = new ExprEntry {
                                    Op = exprAttr.Op,
                                    Disable = exprAttr.Disable,
                                    ExprList = exprAttr.ExprList
                                };
                                if (exprAttr.IsProcessAttribute)
                                    dep.ProcessValues.Add(exprEntry);
                                else
                                    dep.HideValues.Add(exprEntry);
                                foreach (ExprAttribute.Expr expr in exprAttr.ExprList) {
                                    if (!selectionControls.Contains(expr.LeftProperty))
                                        selectionControls.Add(expr.LeftProperty);
                                    if (expr.IsRightProperty) {
                                        if (!selectionControls.Contains(expr.RightProperty))
                                            selectionControls.Add(expr.RightProperty);
                                    }
                                }
                            }
                        }
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
