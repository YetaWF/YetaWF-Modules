/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class HiddenComponentBase : YetaWFComponent {
        public const string TemplateName = "Hidden";
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class HiddenDisplayComponent : HiddenComponentBase, IYetaWFComponent<object> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(object model) {
            YTagBuilder tag = new YTagBuilder("input");
            FieldSetup(tag, FieldType.Normal);
            tag.MergeAttribute("type", "hidden");
            if (model != null && model.GetType().IsEnum) {
                model = (int)model;
            }
            tag.MergeAttribute("value", model == null ? "" : model.ToString());
            return Task.FromResult(tag.ToYHtmlString(YTagRenderMode.StartTag));
        }
    }
    public class HiddenEditComponent : HiddenComponentBase, IYetaWFComponent<object> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public Task<YHtmlString> RenderAsync(object model) {
            YTagBuilder tag = new YTagBuilder("input");
            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Normal);
            tag.MergeAttribute("type", "hidden");
            if (model != null && model.GetType().IsEnum) {
                model = (int)model;
            }
            tag.MergeAttribute("value", model == null ? "" : model.ToString());
            return Task.FromResult(tag.ToYHtmlString(YTagRenderMode.StartTag));
        }
    }
}
