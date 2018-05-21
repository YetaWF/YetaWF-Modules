using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class IntValue2DisplayComponent : IntValueDisplayComponentBase { public IntValue2DisplayComponent() : base("IntValue2", "yt_intvalue2") { } }
    public class IntValue2EditComponent : IntValueEditComponentBase { public IntValue2EditComponent() : base("IntValue2", "yt_intvalue2") { } }
    public class IntValue4DisplayComponent : IntValueDisplayComponentBase { public IntValue4DisplayComponent() : base("IntValue4", "yt_intvalue4") { } }
    public class IntValue4EditComponent : IntValueEditComponentBase { public IntValue4EditComponent() : base("IntValue4", "yt_intvalue4") { } }
    public class IntValue6DisplayComponent : IntValueDisplayComponentBase { public IntValue6DisplayComponent() : base("IntValue6", "yt_intvalue6") { } }
    public class IntValue6EditComponent : IntValueEditComponentBase { public IntValue6EditComponent() : base("IntValue6", "yt_intvalue6") { } }
    public class IntValueDisplayComponent : IntValueDisplayComponentBase { public IntValueDisplayComponent() : base("IntValue", "yt_intvalue") { } }
    public class IntValueEditComponent : IntValueEditComponentBase { public IntValueEditComponent() : base("IntValue", "yt_intvalue") { } }

    public abstract class IntValueDisplayComponentBase : YetaWFComponent, IYetaWFComponent<int>, IYetaWFComponent<int?> {

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public string TemplateName { get; set; }
        public string TemplateClass { get; set; }

        public IntValueDisplayComponentBase(string templateName, string templateClass) {
            TemplateName = templateName;
            TemplateClass = templateClass;
        }

        public async Task<YHtmlString> RenderAsync(int model) {
            return await RenderAsync((int?)model);
        }
        public Task<YHtmlString> RenderAsync(int? model) {
            if (model == null) return Task.FromResult(new YHtmlString(""));
            return Task.FromResult(new YHtmlString(model.ToString()));
        }
    }
    public abstract class IntValueEditComponentBase : YetaWFComponent, IYetaWFComponent<int>, IYetaWFComponent<int?> {

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public string TemplateName { get; set; }
        public string TemplateClass { get; set; }

        public IntValueEditComponentBase(string templateName, string templateClass) {
            TemplateName = templateName;
            TemplateClass = templateClass;
        }

        public override async Task IncludeAsync() {
            await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.userevents.min.js");
            await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.numerictextbox.min.js");
            await base.IncludeAsync();
        }
        public async Task<YHtmlString> RenderAsync(int model) {
            return await RenderAsync((int?) model);
        }
        public Task<YHtmlString> RenderAsync(int? model) {

            YTagBuilder tag = new YTagBuilder("input");
            tag.AddCssClass(TemplateClass);
            tag.AddCssClass("t_edit");
            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Normal);

            tag.MergeAttribute("maxlength", "20");

            // handle min/max
            RangeAttribute rangeAttr = PropData.TryGetAttribute<RangeAttribute>();
            if (rangeAttr != null) {
                tag.MergeAttribute("data-min", ((int)rangeAttr.Minimum).ToString("D"));
                tag.MergeAttribute("data-max", ((int)rangeAttr.Maximum).ToString("D"));
            }
            string noEntry = PropData.GetAdditionalAttributeValue<string>("NoEntry", null);
            if (!string.IsNullOrWhiteSpace(noEntry))
                tag.MergeAttribute("data-noentry", noEntry);
            int step = PropData.GetAdditionalAttributeValue<int>("Step", 1);
            tag.MergeAttribute("data-step", step.ToString());

            if (model != null)
                tag.MergeAttribute("value", ((int)model).ToString());

            return Task.FromResult(tag.ToYHtmlString(YTagRenderMode.SelfClosing));
        }
    }
}
