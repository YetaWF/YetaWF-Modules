/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.ResponseFilter;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Renders a container. This is used to render a container from a view (used by client side to generate containers).
    /// </summary>
    [PrivateComponent]
    public class ContainerDataContainer : YetaWFComponent, IYetaWFContainer<ContainerDataContainer.ContainerData> {

        /// <summary>
        /// Defines the container data to render the ContainerView.
        /// </summary>
        public class ContainerData {
            /// <summary>
            /// The type of the container.
            /// </summary>
            public ComponentType Type { get; set; }
            /// <summary>
            /// The container model.
            /// </summary>
            public object Model { get; set; } = null!;
            /// <summary>
            /// The UIHint used to render the container.
            /// </summary>
            public string UIHint { get; set; } = null!;
            /// <summary>
            /// The HTML field prefix used.
            /// </summary>
            public string FieldPrefix { get; set; } = null!;
        }
        /// <summary>
        /// The result of the rendered ContainerView.
        /// </summary>
        public class ContainerDataResult {
            /// <summary>
            /// The rendered HTML.
            /// </summary>
            public string HTML { get; set; } = null!;
            /// <summary>
            /// Defines the current unique id counters.
            /// </summary>
            public YetaWFManager.UniqueIdInfo UniqueIdCounters { get; set; } = null!;
        }

        /// <summary>
        /// Defines the component's name.
        /// </summary>
        public const string TemplateName = "ContainerData";

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

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

        private class TreePartialResult {
            public int Records { get; set; }
            public string HTML { get; set; } = null!;
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderContainerAsync(ContainerDataContainer.ContainerData model) {

            string data;

            using (Manager.StartNestedComponent(model.FieldPrefix)) {

                // render the component
                if (model.Type == ComponentType.Display)
                    data = await HtmlHelper.ForDisplayContainerAsync(model.Model, model.UIHint);
                else
                    data = await HtmlHelper.ForEditContainerAsync(model.Model, model.UIHint);
            }

            // because this is returned as json (ultimately), we need to do whitespace compression here otherwise
            // we get a mismatch between original server generated tree and dynamic updates via client side
            data = WhiteSpaceResponseFilter.Compress(data);

            data += Manager.ScriptManager.RenderEndofPageScripts();// portions generated by components

            ContainerDataResult result = new ContainerDataResult {
                HTML = data,
                UniqueIdCounters = Manager.UniqueIdCounters,
            };
            return Utility.JsonSerialize(result);
        }
    }
}
