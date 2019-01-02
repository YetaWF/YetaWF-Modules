/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class SkinCollectionComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(SkinCollectionComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "SkinCollection";
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class SkinCollectionDisplayComponent : SkinCollectionComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(string model) {

            // get all available skins
            SkinAccess skinAccess = new SkinAccess();

            string desc = (from skinColl in skinAccess.GetAllSkinCollections() where model == skinColl.CollectionName select skinColl.CollectionDescription).FirstOrDefault();
            if (desc == null) {
                bool useDefault = !PropData.GetAdditionalAttributeValue("NoDefault", false);
                if (useDefault)
                    desc = __ResStr("siteDef", "(Site Default)");
            }
            return Task.FromResult(new YHtmlString(string.IsNullOrWhiteSpace(desc) ? "&nbsp;" : HE(desc)));
        }
    }

    public class SkinCollectionEditComponent : SkinCollectionComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(string model) {

            // get all available skins
            SkinAccess skinAccess = new SkinAccess();
            List<SelectionItem<string>> list = (from skinColl in skinAccess.GetAllSkinCollections() orderby skinColl.CollectionDescription select new SelectionItem<string>() {
                Text = skinColl.CollectionDescription,
                Value = skinColl.CollectionName,
            }).ToList();
            bool useDefault = !PropData.GetAdditionalAttributeValue("NoDefault", false);
            if (useDefault)
                list.Insert(0, new SelectionItem<string> {
                    Text = __ResStr("siteDef", "(Site Default)"),
                    Tooltip = __ResStr("siteDefTT", "Use the site defined default skin"),
                    Value = "",
                });
            // display the skins in a drop down
            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_skinselection");
        }
    }
}
