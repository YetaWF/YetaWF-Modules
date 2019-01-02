/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components
{
    /// <summary>
    /// A hidden field that this rendered without field name, usually used for lists which require just fieldprefix[index] without .fieldname
    /// </summary>
    public abstract class GridValueComponentBase : YetaWFComponent {
        public const string TemplateName = "GridValue";
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class GridValueDisplayComponent : GridValueComponentBase, IYetaWFComponent<object> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(object model) {
            YTagBuilder tag = new YTagBuilder("input");
            FieldSetup(tag, FieldType.Normal);
            tag.MergeAttribute("name", FieldNamePrefix, true);
            tag.MergeAttribute("type", "hidden");
            tag.MergeAttribute("value", model == null ? "" : model.ToString());
            return Task.FromResult(tag.ToYHtmlString(YTagRenderMode.StartTag));
        }
    }
    public class GridValueEditComponent : GridValueComponentBase, IYetaWFComponent<object> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public Task<YHtmlString> RenderAsync(object model) {
            YTagBuilder tag = new YTagBuilder("input");
            FieldSetup(tag, FieldType.Normal);
            tag.MergeAttribute("name", FieldNamePrefix, true);
            tag.MergeAttribute("type", "hidden");
            tag.MergeAttribute("value", model == null ? "" : model.ToString());
            return Task.FromResult(tag.ToYHtmlString(YTagRenderMode.StartTag));
        }
    }
}
