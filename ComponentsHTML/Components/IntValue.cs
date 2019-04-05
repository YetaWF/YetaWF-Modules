/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Implementation of the IntValue2 display component.
    /// </summary>
    public class IntValue2DisplayComponent : IntValueDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IntValue2DisplayComponent() : base("IntValue2", "yt_intvalue2") { }
    }
    /// <summary>
    /// Implementation of the IntValue2 edit component.
    /// </summary>
    public class IntValue2EditComponent : IntValueEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IntValue2EditComponent() : base("IntValue2", "yt_intvalue2") { }
    }
    /// <summary>
    /// Implementation of the IntValue4 display component.
    /// </summary>
    public class IntValue4DisplayComponent : IntValueDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IntValue4DisplayComponent() : base("IntValue4", "yt_intvalue4") { }
    }
    /// <summary>
    /// Implementation of the IntValue4 edit component.
    /// </summary>
    public class IntValue4EditComponent : IntValueEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IntValue4EditComponent() : base("IntValue4", "yt_intvalue4") { }
    }
    /// <summary>
    /// Implementation of the IntValue6 display component.
    /// </summary>
    public class IntValue6DisplayComponent : IntValueDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IntValue6DisplayComponent() : base("IntValue6", "yt_intvalue6") { }
    }
    /// <summary>
    /// Implementation of the IntValue6 edit component.
    /// </summary>
    public class IntValue6EditComponent : IntValueEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IntValue6EditComponent() : base("IntValue6", "yt_intvalue6") { }
    }
    /// <summary>
    /// Implementation of the IntValue display component.
    /// </summary>
    public class IntValueDisplayComponent : IntValueDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IntValueDisplayComponent() : base("IntValue", "yt_intvalue") { }
    }
    /// <summary>
    /// Implementation of the IntValue edit component.
    /// </summary>
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
    public abstract class IntValueEditComponentBase : YetaWFComponent, IYetaWFComponent<int>, IYetaWFComponent<int?> {

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

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            await KendoUICore.AddFileAsync("kendo.userevents.min.js");
            await KendoUICore.AddFileAsync("kendo.numerictextbox.min.js");
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

            HtmlBuilder hb = new HtmlBuilder();

            YTagBuilder tag = new YTagBuilder("input");
            string id = MakeId(tag);
            tag.AddCssClass(TemplateClass);
            tag.AddCssClass("t_edit");
            tag.AddCssClass("yt_intvalue_base");
            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Normal);

            tag.MergeAttribute("maxlength", "20");

            if (model != null)
                tag.MergeAttribute("value", ((int)model).ToString());

            // handle min/max
            int min = 0, max = 999999999;
            RangeAttribute rangeAttr = PropData.TryGetAttribute<RangeAttribute>();
            if (rangeAttr != null) {
                min = (int)rangeAttr.Minimum;
                max = (int)rangeAttr.Maximum;
            }
            string noEntry = PropData.GetAdditionalAttributeValue<string>("NoEntry", null);
            int step = PropData.GetAdditionalAttributeValue<int>("Step", 1);

            hb.Append($@"
{tag.ToString(YTagRenderMode.StartTag)}");

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.IntValueEditComponent('{id}', {{ Min: {min}, Max: {max}, Step: {step}, NoEntryText: '{JE(noEntry??"")}' }});");

            return Task.FromResult(hb.ToString());
        }
    }
}
