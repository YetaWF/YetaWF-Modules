/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the ListOfStringsPre component implementation.
    /// </summary>
    public abstract class ListOfStringsPreComponentBase : YetaWFComponent {

        internal const string TemplateName = "ListOfStringsPre";

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
    /// Renders a list of strings within &lt;pre&gt; ... &lt;/pre&gt; tags. Each string is on its own line within the tags.
    /// </summary>
    /// <example>
    /// [Category("Variables"), Caption("Panes"), Description("The panes defined by the page skin")]
    /// [UIHint("ListOfStringsPre"), ReadOnly]
    /// public List&lt;string&gt; Panes { get; set; }
    /// </example>
    public class ListOfStringsPreDisplayComponent : ListOfStringsPreComponentBase, IYetaWFComponent<List<string>> {

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

            hb.Append($@"
<pre class='yt_listofstringspre t_display'>");

            if (model != null) {
                foreach (var s in model) {
                    hb.Append(Utility.HtmlEncode(s));
                    hb.Append("\r\n");
                }
            }
            hb.Append(@"</pre>");

            return Task.FromResult(hb.ToString());
        }
    }
}
