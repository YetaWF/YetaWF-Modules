/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class GridDeleteEntryComponent : YetaWFComponent {

        public const string TemplateName = "GridDeleteEntry";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    public class GridDeleteEntryEditComponent : GridDeleteEntryComponent, IYetaWFComponent<object> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(object model) {

            YTagBuilder tag = new YTagBuilder("span");
            FieldSetup(tag, FieldType.Anonymous);

            SkinImages skinImages = new SkinImages();
            string imageUrl = await skinImages.FindIcon_PackageAsync("#RemoveLight", Package);
            YTagBuilder tagImg = ImageHTML.BuildKnownImageYTag(imageUrl, alt: this.__ResStr("altRemove", "Remove"));
            tagImg.MergeAttribute("name", "DeleteAction", true);
            tag.InnerHtml = tagImg.ToString(YTagRenderMode.StartTag);

            return tag.ToYHtmlString(YTagRenderMode.Normal);
        }
    }
}
