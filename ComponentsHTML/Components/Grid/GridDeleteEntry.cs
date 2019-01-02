/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class GridDeleteEntryComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(GridDeleteEntryComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "GridDeleteEntry";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    public class GridDeleteEntryDisplayComponent : GridDeleteEntryComponentBase, IYetaWFComponent<object> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(object model) {

            YTagBuilder tag = new YTagBuilder("span");
            FieldSetup(tag, FieldType.Anonymous);
            tag.InnerHtml = ImageHTML.BuildKnownIcon("#RemoveLight", title: __ResStr("altRemove", "Remove"), name: "DeleteAction");

            return Task.FromResult(tag.ToYHtmlString(YTagRenderMode.Normal));
        }
    }
}
