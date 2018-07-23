/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class SkinNamePopupComponent : YetaWFComponent {

        public const string TemplateName = "SkinNamePopup";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class SkinNamePopupDisplayComponent : SkinNamePopupComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(string model) {

            // get all available popup skins for this collection
            SkinAccess skinAccess = new SkinAccess();
            string collection = GetSiblingProperty<string>($"{PropertyName}_Collection");
            PageSkinList skinList = skinAccess.GetAllPopupSkins(collection);

            string desc = (from skin in skinList where skin.FileName == model select skin.Name).FirstOrDefault();
            if (desc == null)
                desc = skinList.First().Name;
            return Task.FromResult(new YHtmlString(string.IsNullOrWhiteSpace(desc) ? "&nbsp;" : HE(desc)));
        }
    }

    public class SkinNamePopupEditComponent : SkinNamePopupComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(string model) {

            // get all available page skins for this collection
            SkinAccess skinAccess = new SkinAccess();
            string collection = GetSiblingProperty<string>($"{PropertyName}_Collection");
            PageSkinList skinList = skinAccess.GetAllPopupSkins(collection);
            List<SelectionItem<string>> list = (from skin in skinList orderby skin.Description select new SelectionItem<string>() {
                Text = skin.Name,
                Tooltip = skin.Description,
                Value = skin.FileName,
            }).ToList();
            // display the skins in a drop down
            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_skinname");
        }
        internal static YHtmlString RenderReplacementSkinsForCollection(string skinCollection) {
            SkinAccess skinAccess = new SkinAccess();
            PageSkinList skinList = skinAccess.GetAllPageSkins(skinCollection);
            List<SelectionItem<string>> list = (from skin in skinList orderby skin.Description select new SelectionItem<string>() {
                Text = skin.Name,
                Tooltip = skin.Description,
                Value = skin.FileName,
            }).ToList();
            // render a new dropdown list
            return DropDownListEditComponentBase<string>.RenderDataSource(list, null);
        }
    }
}
