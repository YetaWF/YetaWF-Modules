/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class BooleanTextComponent : YetaWFComponent {

        public const string TemplateName = "BooleanText";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class BooleanTextDisplayComponent : BooleanTextComponent, IYetaWFComponent<bool?> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(bool model) {
            return await RenderAsync((bool?)model);
        }
        public Task<YHtmlString> RenderAsync(bool? model) {

            YTagBuilder tag = new YTagBuilder("input");
            tag.AddCssClass("yt_booleantext");
            tag.AddCssClass("t_display");
            FieldSetup(tag, FieldType.Anonymous);
            tag.Attributes.Add("type", "checkbox");
            tag.Attributes.Add("disabled", "disabled");
            if (model != null && (bool)model)
                tag.Attributes.Add("checked", "checked");

            string text;
            if (TryGetSiblingProperty($"{PropertyName}_Text", out text))
                return Task.FromResult(new YHtmlString(tag.ToString(YTagRenderMode.StartTag) + HE(text)));
            else
                return Task.FromResult(tag.ToYHtmlString(YTagRenderMode.StartTag));
        }
    }
    public class BooleanTextEditComponent : BooleanTextComponent, IYetaWFComponent<bool>, IYetaWFComponent<bool?> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(bool model) {
            return await RenderAsync((bool?) model);
        }
        public Task<YHtmlString> RenderAsync(bool? model) {

            YTagBuilder tag = new YTagBuilder("input");
            tag.AddCssClass("yt_booleantext");
            tag.AddCssClass("t_edit");
            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Anonymous);
            tag.Attributes.Add("type", "checkbox");
            tag.Attributes.Add("value", "true");
            if (model != null && (bool)model)
                tag.Attributes.Add("checked", "checked");

            // add a hidden field so we always get "something" for check boxes (that means we have to deal with duplicates names)
            YTagBuilder tagHidden = new YTagBuilder("input");
            FieldSetup(tagHidden, FieldType.Normal);
            tagHidden.Attributes.Add("type", "hidden");
            tagHidden.Attributes.Add("value", "false");

            string text;
            if (TryGetSiblingProperty($"{PropertyName}_Text", out text))
                return Task.FromResult(new YHtmlString(tag.ToString(YTagRenderMode.StartTag) + tagHidden.ToString(YTagRenderMode.StartTag) + HE(text)));
            else
                return Task.FromResult(new YHtmlString(tag.ToYHtmlString(YTagRenderMode.StartTag) + tagHidden.ToString(YTagRenderMode.StartTag)));
        }
    }
}
