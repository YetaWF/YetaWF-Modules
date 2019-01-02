/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class ModuleSkinsComponentBase : YetaWFComponent {

        public const string TemplateName = "ModuleSkins";
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class ModuleSkinsDisplayComponent : ModuleSkinsComponentBase, IYetaWFComponent<SerializableList<SkinDefinition>> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public class ModuleSkinUI {

            [UIHint("ModuleSkinName"), ResourceRedirect(nameof(FileNameCaption)), Description("The name of the skin collection")]
            public string FileName { get; set; } // may be null for site default
            public ModuleSkinList FileName_ModuleSkinList { get; set; }
            public string FileNameCaption { get; set; }
        }

        public async Task<YHtmlString> RenderAsync(SerializableList<SkinDefinition> model) {

            HtmlBuilder hb = new HtmlBuilder();

            int index = 0;
            SkinAccess skinAccess = new SkinAccess();

            hb.Append($@"
<div id='{DivId}' class='yt_moduleskins t_display'>");

            foreach (SkinCollectionInfo skinDef in (from s in skinAccess.GetAllSkinCollections() orderby s.CollectionDescription select s).ToList()) {
                hb.Append($@"
    <div class='t_collection'>");

                SkinDefinition sd = (from s in model where s.Collection == skinDef.CollectionName select s).FirstOrDefault();
                string fileName = sd?.FileName;

                ModuleSkinUI ms = new ModuleSkinUI {
                    FileName = fileName,
                    FileNameCaption = skinDef.CollectionDescription,
                    FileName_ModuleSkinList = skinAccess.GetAllModuleSkins(skinDef.CollectionName),
                };

                using (Manager.StartNestedComponent($@"{FieldName}[{index}]")) {
                    hb.Append(await HtmlHelper.ForLabelAsync(ms, nameof(ms.FileName)));
                    hb.Append(await HtmlHelper.ForDisplayAsync(ms, nameof(ms.FileName)));
                }
                index++;
                hb.Append($@"
    </div>");
            }

            hb.Append($@"
</div>");

            return hb.ToYHtmlString();
        }
    }
    public class ModuleSkinsEditComponent : ModuleSkinsComponentBase, IYetaWFComponent<SerializableList<SkinDefinition>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class ModuleSkinUI {

            [UIHint("Hidden")]
            public string Collection { get; set; }

            [UIHint("ModuleSkinName"), ResourceRedirect(nameof(FileNameCaption)), Description("The name of the skin collection")]
            public string FileName { get; set; } // may be null for site default
            public ModuleSkinList FileName_ModuleSkinList { get; set; }
            public string FileNameCaption { get; set; }
        }

        public async Task<YHtmlString> RenderAsync(SerializableList<SkinDefinition> model) {

            HtmlBuilder hb = new HtmlBuilder();

            int index = 0;
            SkinAccess skinAccess = new SkinAccess();

            hb.Append($@"
<div id='{DivId}' class='yt_moduleskins t_edit'>");

            foreach (SkinCollectionInfo skinDef in (from s in skinAccess.GetAllSkinCollections() orderby s.CollectionDescription select s).ToList()) {
                hb.Append($@"
    <div class='t_collection'>");

                SkinDefinition sd = (from s in model where s.Collection == skinDef.CollectionName select s).FirstOrDefault();
                string fileName = sd?.FileName;

                ModuleSkinUI ms = new ModuleSkinUI {
                    Collection = skinDef.CollectionName,
                    FileName = fileName,
                    FileNameCaption = skinDef.CollectionDescription,
                    FileName_ModuleSkinList = skinAccess.GetAllModuleSkins(skinDef.CollectionName),
                };

                using (Manager.StartNestedComponent($@"{FieldName}[{index}]")) {
                    hb.Append(await HtmlHelper.ForDisplayAsync(ms, nameof(ms.Collection)));
                    hb.Append(await HtmlHelper.ForLabelAsync(ms, nameof(ms.FileName)));
                    hb.Append(await HtmlHelper.ForEditAsync(ms, nameof(ms.FileName)));
                    ValidationMessage(HtmlHelper, FieldNamePrefix, nameof(ms.FileName));
                }

                index++;
                hb.Append($@"
    </div>");
            }

            hb.Append($@"
</div>");

            return hb.ToYHtmlString();
        }
    }
}
