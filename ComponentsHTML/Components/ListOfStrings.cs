/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the ListOfStrings component implementation.
    /// </summary>
    public abstract class ListOfStringsComponentBase : YetaWFComponent {

        internal const string TemplateName = "ListOfStrings";

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
    /// Renders a list of strings, joined using the specified delimiter.
    /// </summary>
    /// <example>
    /// [Category("Variables"), Caption("Panes"), Description("The panes defined by the page skin")]
    /// [UIHint("ListOfStrings"), ReadOnly]
    /// public List&lt;string&gt; Panes { get; set; }
    /// </example>
    [UsesAdditional("Delimiter", "string", ", ", "Defines the delimiter used between the strings as they are rendered. The default is \", \". The delimiter can consist of HTML tags.")]
    public class ListOfStringsDisplayComponent : ListOfStringsComponentBase, IYetaWFComponent<List<string>> {

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
        public Task<string> RenderAsync(List<string> model) {
            HtmlBuilder hb = new HtmlBuilder();

            string delim = PropData.GetAdditionalAttributeValue("Delimiter", ", ");

            hb.Append($@"
<div class='yt_listofstrings t_display'>");

            bool first = true;
            if (model != null) {
                foreach (var s in model) {
                    if (first)
                        first = false;
                    else
                        hb.Append(delim);
                    hb.Append(Utility.HE(s));
                }
            }
            hb.Append(@"
</div>");

            return Task.FromResult(hb.ToString());
        }
    }
}
