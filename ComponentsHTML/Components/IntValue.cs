/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Displays the specified value formatted as an integer value. Should be used for up to 2 digits. If the specified value is null, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Caption("Average Char. Width"), Description("The average character width, calculated using the current skin")]
    /// [UIHint("IntValue2"), ReadOnly]
    /// public int Width { get; set; }
    /// </example>
    public class IntValue2DisplayComponent : IntValueDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IntValue2DisplayComponent() : base("IntValue2", "yt_intvalue2") { }
    }
    /// <summary>
    /// Allows entry of an integer value. Should be used for up to 2 digits.
    /// </summary>
    /// <remarks>
    /// The RangeAttribute can be used to define the lowest and highest allowable values.
    ///
    /// </remarks>
    /// <example>
    /// [Caption("Days"), Description("The number of days a backup is saved - once a backup has been saved for the specified number of days, it is deleted")]
    /// [UIHint("IntValue2"), Range(1, 99), Required]
    /// public int Days { get; set; }
    /// </example>
    public class IntValue2EditComponent : IntValueEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IntValue2EditComponent() : base("IntValue2", "yt_intvalue2") { }
    }
    /// <summary>
    /// Displays the specified value formatted as an integer value. Should be used for up to 4 digits. If the specified value is null, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Caption("Average Char. Width"), Description("The average character width, calculated using the current skin")]
    /// [UIHint("IntValue4"), ReadOnly]
    /// public int Width { get; set; }
    /// </example>
    public class IntValue4DisplayComponent : IntValueDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IntValue4DisplayComponent() : base("IntValue4", "yt_intvalue4") { }
    }
    /// <summary>
    /// Allows entry of an integer value. Should be used for up to 4 digits.
    /// </summary>
    /// <remarks>
    /// The RangeAttribute can be used to define the lowest and highest allowable values.
    ///
    /// </remarks>
    /// <example>
    /// [Caption("Days"), Description("The number of days a backup is saved - once a backup has been saved for the specified number of days, it is deleted")]
    /// [UIHint("IntValue4"), Range(1, 9999), Required]
    /// public int Days { get; set; }
    /// </example>
    public class IntValue4EditComponent : IntValueEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IntValue4EditComponent() : base("IntValue4", "yt_intvalue4") { }
    }
    /// <summary>
    /// Displays the specified value formatted as an integer value. Should be used for up to 6 digits. If the specified value is null, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Caption("Average Char. Width"), Description("The average character width, calculated using the current skin")]
    /// [UIHint("IntValue4"), ReadOnly]
    /// public int Width { get; set; }
    /// </example>
    public class IntValue6DisplayComponent : IntValueDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IntValue6DisplayComponent() : base("IntValue6", "yt_intvalue6") { }
    }
    /// <summary>
    /// Allows entry of an integer value. Should be used for up to 6 digits.
    /// </summary>
    /// <remarks>
    /// The RangeAttribute can be used to define the lowest and highest allowable values.
    ///
    /// </remarks>
    /// <example>
    /// [Caption("Days"), Description("The number of days a backup is saved - once a backup has been saved for the specified number of days, it is deleted")]
    /// [UIHint("IntValue6"), Range(1, 999999), Required]
    /// public int Days { get; set; }
    /// </example>
    public class IntValue6EditComponent : IntValueEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IntValue6EditComponent() : base("IntValue6", "yt_intvalue6") { }
    }
    /// <summary>
    /// Displays the specified value formatted as an integer value. There is no width limitation. If the specified value is null, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Caption("Average Char. Width"), Description("The average character width, calculated using the current skin")]
    /// [UIHint("IntValue"), ReadOnly]
    /// public int Width { get; set; }
    /// </example>
    public class IntValueDisplayComponent : IntValueDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IntValueDisplayComponent() : base("IntValue", "yt_intvalue") { }
    }
    /// <summary>
    /// Allows entry of an integer value. There is no width limitation.
    /// </summary>
    /// <remarks>
    /// The RangeAttribute can be used to define the lowest and highest allowable values.
    ///
    /// </remarks>
    /// <example>
    /// [Caption("Days"), Description("The number of days a backup is saved - once a backup has been saved for the specified number of days, it is deleted")]
    /// [UIHint("IntValue"), Range(1, 99999999), Required]
    /// public int Days { get; set; }
    /// </example>
    public class IntValueEditComponent : IntValueEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IntValueEditComponent() : base("IntValue", "yt_intvalue") { }
    }

    /// <summary>
    /// Base class for the IntValue display component implementation.
    /// </summary>
    public abstract class IntValueDisplayComponentBase : YetaWFComponent, IYetaWFComponent<int>, IYetaWFComponent<int?> {

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

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        internal string TemplateName { get; set; }
        internal string TemplateClass { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="templateName">The template name.</param>
        /// <param name="templateClass">The CSS class representing the component.</param>
        public IntValueDisplayComponentBase(string templateName, string templateClass) {
            TemplateName = templateName;
            TemplateClass = templateClass;
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(int model) {
            return await RenderAsync((int?)model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(int? model) {
            if (model == null) return Task.FromResult<string>(null);
            return Task.FromResult(HE(model.ToString()));
        }
    }
    /// <summary>
    /// Base class for the IntValue edit component implementation.
    /// </summary>
    [UsesAdditional("Step", "int", "1", "The increment/decrement used when clicking on the up/down arrows of the edit control.")]
    [UsesSibling("_PlaceHolder", "string", "Defines the placeholder text shown when control contents are empty.")]
    public abstract class IntValueEditComponentBase : YetaWFComponent, IYetaWFComponent<int>, IYetaWFComponent<int?> {

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

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        internal string TemplateName { get; set; }
        internal string TemplateClass { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="templateName">The template name.</param>
        /// <param name="templateClass">The CSS class representing the component.</param>
        public IntValueEditComponentBase(string templateName, string templateClass) {
            TemplateName = templateName;
            TemplateClass = templateClass;
        }

        /// <inheritdoc/>
        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddTemplateAsync(AreaRegistration.CurrentPackage.AreaName, "Number", ComponentType.Edit);
            await base.IncludeAsync();
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(int model) {
            return await RenderAsync((int?) model);
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(int? model) {

            TryGetSiblingProperty<string>($"{PropertyName}_PlaceHolder", out string placeHolder);

            NumberSetup setup = new NumberSetup {
                Min = 0,
                Max = 999999999,
                Step = PropData.GetAdditionalAttributeValue<int>("Step", 1),
                Digits = 0,
                Locale = MultiString.ActiveLanguage,
            };

            // handle min/max
            RangeAttribute rangeAttr = PropData.TryGetAttribute<RangeAttribute>();
            if (rangeAttr != null) {
                setup.Min = (int)rangeAttr.Minimum;
                setup.Max = (int)rangeAttr.Maximum;
            }

            placeHolder = string.IsNullOrWhiteSpace(placeHolder) ? string.Empty : $" placeholder={HAE(placeHolder)}";

            string tags = $@"<div class='yt_number_container'><input type='text'{FieldSetup(Validation ? FieldType.Validated : FieldType.Normal)} id='{ControlId}' class='{TemplateClass} t_edit yt_intvalue_base{GetClasses()}' maxlength='20' value='{model?.ToString()}'{placeHolder}></div>";

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.IntValueEditComponent('{ControlId}', {Utility.JsonSerialize(setup)});");

            return Task.FromResult(tags);
        }
    }
}
