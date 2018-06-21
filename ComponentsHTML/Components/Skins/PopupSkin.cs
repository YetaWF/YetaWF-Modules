/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class PopupSkinComponentBase : YetaWFComponent {

        public const string TemplateName = "PopupSkin";
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class PopupSkinDisplayComponent : PopupSkinComponentBase, IYetaWFComponent<SkinDefinition> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public class PopupSkinUI {
            [Caption("Skin Collection"), Description("The name of the skin collection")]
            [StringLength(SkinDefinition.MaxCollection)]
            [UIHint("SkinCollection")]
            public string Collection { get; set; } // may be null for site default
            [Caption("Skin Name"), Description("The name of the skin")]
            [StringLength(SkinDefinition.MaxSkinFile)]
            [UIHint("SkinNamePopup")]
            public string FileName { get; set; } // may be null for site default
            public string FileName_Collection { get { return Collection; } }
        }

        public async Task<YHtmlString> RenderAsync(SkinDefinition model) {

            HtmlBuilder hb = new HtmlBuilder();

            PopupSkinUI ps = new PopupSkinUI {
                Collection = model.Collection,
                FileName = model.FileName,
            };

            using (Manager.StartNestedComponent(FieldName)) {

                hb.Append($@"
<div id='{ControlId}' class='yt_popupskin t_display'>
    <div class='t_collection'>
        {await HtmlHelper.ForLabelAsync(ps, nameof(ps.Collection))}
        {await HtmlHelper.ForDisplayAsync(ps, nameof(ps.Collection))}
    </div>
    <div class='t_skin'>
        {await HtmlHelper.ForLabelAsync(ps, nameof(ps.FileName))}
        {await HtmlHelper.ForDisplayAsync(ps, nameof(ps.FileName))}
    </div>
</div>");
            }
            return hb.ToYHtmlString();
        }
    }
    public class PopupSkinEditComponent : PopupSkinComponentBase, IYetaWFComponent<SkinDefinition> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class PopupSkinUI {
            [UIHint("Hidden")]
            public string AjaxUrl { get; set; }

            [Caption("Skin Collection"), Description("The name of the skin collection")]
            [StringLength(SkinDefinition.MaxCollection)]
            [UIHint("SkinCollection")]
            public string Collection { get; set; } // may be null for site default
            [Caption("Skin Name"), Description("The name of the skin")]
            [StringLength(SkinDefinition.MaxSkinFile)]
            [UIHint("SkinNamePopup")]
            public string FileName { get; set; } // may be null for site default
            public string FileName_Collection { get { return Collection; } }
        }

        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddTemplateFromUIHintAsync("PageSkin");
            await base.IncludeAsync();
        }

        public async Task<YHtmlString> RenderAsync(SkinDefinition model) {

            HtmlBuilder hb = new HtmlBuilder();

            PopupSkinUI ps = new PopupSkinUI {
                AjaxUrl = YetaWFManager.UrlFor(typeof(SkinController), nameof(SkinController.GetPopupPageSkins)),
                Collection = model.Collection,
                FileName = model.FileName,
            };

            using (Manager.StartNestedComponent(FieldName)) {

                hb.Append($@"
<div id='{ControlId}' class='yt_popupskin t_edit'>
    {(await HtmlHelper.ForDisplayAsync(ps, nameof(ps.AjaxUrl))).ToString()}
    <div class='t_collection'>
        {await HtmlHelper.ForLabelAsync(ps, nameof(ps.Collection))}
        {await HtmlHelper.ForEditAsync(ps, nameof(ps.Collection))}
    </div>
    <div class='t_skin'>
        {await HtmlHelper.ForLabelAsync(ps, nameof(ps.FileName))}
        {await HtmlHelper.ForEditAsync(ps, nameof(ps.FileName))}
    </div>
</div>
<script>
    YetaWF_Template_PageSkin.popupInit('{ControlId}');
</script>");

            }
            return hb.ToYHtmlString();
        }
    }
}
