/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the USState component implementation.
    /// </summary>
    public abstract class USStateComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(USStateComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "USState";

        /// <summary>
        /// Returns the package implementing the component.
        /// </summary>
        /// <returns>Returns the package implementing the component.</returns>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
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
    /// Displays the full state name of a US state given an abbreviation. If the model is null or the abbreviation is not found, nothing is rendered.
    /// </summary>
    /// <remarks>
    /// All states and their abbreviations are defined in ./CoreComponents/Core/Addons/_Templates/USState/USStates.txt.
    /// </remarks>
    /// <example>
    /// [Category("General"), Caption("State"), Description("The state of the mailing address")]
    /// [UIHint("USState"), ReadOnly]
    /// public string StateUS { get; set; }
    /// </example>
    public class USStateDisplayComponent : USStateComponentBase, IYetaWFComponent<string> {

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
        public async Task<string> RenderAsync(string model) {
            List<SelectionItem<string>> states = await USState.ReadStatesListAsync();
            if (model == null) model = "";
            string state = (from s in states where string.Compare(s.Value, model.ToUpper(), true) == 0 select s.Text).FirstOrDefault();
            return HE(state);
        }
    }

    /// <summary>
    /// Allows selection of a US state from a dropdown list. The model returns a 2 character abbreviation for the selected state.
    /// </summary>
    /// <remarks>
    /// All states and their abbreviations are defined in ./CoreComponents/Core/Addons/_Templates/USState/USStates.txt.
    /// </remarks>
    /// <example>
    /// [Caption("State"), Description("The state of the billing address")]
    /// [UIHint("USState"), StringLength(Invoice.MaxState), Trim]
    /// public string BillStateUS { get; set; }
    /// </example>
    [UsesAdditional("NoDefault", "bool", "false", "Defines whether a \"(select)\" entry is automatically added as the first entry, with a value of null")]
    public class USStateEditComponent : USStateComponentBase, IYetaWFComponent<string> {

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
        public async Task<string> RenderAsync(string model) {

            List<SelectionItem<string>> states = await USState.ReadStatesListAsync();

            bool useDefault = !PropData.GetAdditionalAttributeValue<bool>("NoDefault");
            if (useDefault) {
                states = (from s in states select s).ToList();//copy
                states.Insert(0, new SelectionItem<string> {
                    Text = __ResStr("default", "(select)"),
                    Tooltip = __ResStr("defaultTT", "Please make a selection"),
                    Value = "",
                });
            }
            return await DropDownListComponent.RenderDropDownListAsync(this, model, states, "yt_usstate");
        }
    }
}
