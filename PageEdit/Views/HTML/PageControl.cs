/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.PageEdit.Addons;
using YetaWF.Modules.PageEdit.Controllers;
using YetaWF.Modules.PageEdit.Modules;

namespace YetaWF.Modules.PageEdit.Views {

    public class PageControlView : YetaWFView, IYetaWFView<PageControlModule, PageControlModuleController.PageControlModel> {

        public const string ViewName = "PageControl";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(PageControlModule module, PageControlModuleController.PageControlModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            // <div id="yPageControlDiv">
            //  action button (with id tid_pagecontrolbutton)
            //  module html...
            // </div>
            YTagBuilder tag = new YTagBuilder("div");
            tag.Attributes.Add("id", "yPageControlDiv");

            ModuleAction action = await module.GetAction_PageControlAsync();
            tag.InnerHtml = await action.RenderAsButtonIconAsync("tid_pagecontrolbutton");
            hb.Append(tag.ToString(YTagRenderMode.Normal));

            bool canEdit = model.EditAuthorized;
            bool canImportPage = false;
            bool canImportModule = false;
            bool canPageAdd = false;
            bool canModuleExistingAdd = false;
            bool canModuleNewAdd = false;
            bool canChangeSiteSkins = false;
            if (canEdit) {
                canImportPage = await Resource.ResourceAccess.IsResourceAuthorizedAsync(CoreInfo.Resource_PageImport);
                canImportModule = await Resource.ResourceAccess.IsResourceAuthorizedAsync(CoreInfo.Resource_ModuleImport);
                canPageAdd = await Resource.ResourceAccess.IsResourceAuthorizedAsync(CoreInfo.Resource_PageAdd);
                canModuleExistingAdd = await Resource.ResourceAccess.IsResourceAuthorizedAsync(CoreInfo.Resource_ModuleExistingAdd);
                canModuleNewAdd = await Resource.ResourceAccess.IsResourceAuthorizedAsync(CoreInfo.Resource_ModuleNewAdd);
                canChangeSiteSkins = await Resource.ResourceAccess.IsResourceAuthorizedAsync(CoreInfo.Resource_SiteSkins);
            }
            bool canOtherUserLogin = !YetaWFManager.Deployed || await Resource.ResourceAccess.IsResourceAuthorizedAsync(CoreInfo.Resource_OtherUserLogin);

            string id = Info.PageControlMod;

            hb.Append($@"
<div id='{id}'>
    {PropertyListComponentBase.RenderTabStripStart(id)}");

            int tabEntry = 0;
            if (canEdit) {
                if (canPageAdd) {
                    hb.Append(PropertyListComponentBase.RenderTabEntry(id, this.__ResStr("tabNewPage", "New Page"), this.__ResStr("tabNewPageTT", "Add a new page to the site"), tabEntry));
                    ++tabEntry;
                }
            }
            if (!Manager.CurrentPage.Temporary) {
                if (canEdit) {
                    if (canModuleNewAdd) {
                        hb.Append(PropertyListComponentBase.RenderTabEntry(id, this.__ResStr("tabNew", "New Module"), this.__ResStr("tabNewTT", "Add a new module to this page (creates a new module)"), tabEntry));
                        ++tabEntry;
                    }
                    if (canModuleExistingAdd) {
                        hb.Append(PropertyListComponentBase.RenderTabEntry(id, this.__ResStr("tabOld", "Existing Module"), this.__ResStr("tabOldTT", "Add an existing module to this page (this does not copy the module)"), tabEntry));
                        ++tabEntry;
                    }
                    if (canImportPage) {
                        hb.Append(PropertyListComponentBase.RenderTabEntry(id, this.__ResStr("tabImportPage", "Import Page"), this.__ResStr("tabImportPageTT", "Import a page (creates a new page)"), tabEntry));
                        ++tabEntry;
                    }
                    if (canImportModule) {
                        hb.Append(PropertyListComponentBase.RenderTabEntry(id, this.__ResStr("tabImportModule", "Import Module"), this.__ResStr("tabImportModuleTT", "Import module data into this page (creates a new module)"), tabEntry));
                        ++tabEntry;
                    }
                    if (canChangeSiteSkins) {
                        hb.Append(PropertyListComponentBase.RenderTabEntry(id, this.__ResStr("tabSkins", "Skins"), this.__ResStr("tabSkinsTT", "Change default skins used site wide"), tabEntry));
                        ++tabEntry;
                    }
                }
            }
            if (canOtherUserLogin) {
                hb.Append(PropertyListComponentBase.RenderTabEntry(id, this.__ResStr("tabLogin", "Login"), this.__ResStr("tabLoginTT", "Change site or log in as another user"), tabEntry));
                ++tabEntry;
            }
            hb.Append($@"
    {PropertyListComponentBase.RenderTabStripEnd(id)}");

            if (tabEntry == 0)
                return "&nbsp;";

            int panel = 0;
            if (canEdit) {
                if (canPageAdd) {
                    hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(id, panel, "t_addNewPage")}
        {await HtmlHelper.ForViewAsync($"{Package.AreaName}_AddNewPage", module, model.AddNewPageModel)}
    {PropertyListComponentBase.RenderTabPaneEnd(id, panel)}");
                    ++panel;
                }
            }

            if (!Manager.CurrentPage.Temporary) {
                if (canEdit) {

                    if (canModuleNewAdd) {
                        hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(id, panel, "t_addNewMod")}
        {await HtmlHelper.ForViewAsync($"{Package.AreaName}_AddNewModule", module, model.AddNewModel)}
    {PropertyListComponentBase.RenderTabPaneEnd(id, panel)}");
                        ++panel;
                    }
                    if (canModuleExistingAdd) {
                        hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(id, panel, "t_addExistingMod")}
        {await HtmlHelper.ForViewAsync($"{Package.AreaName}_AddExistingModule", module, model.AddExistingModel)}
    {PropertyListComponentBase.RenderTabPaneEnd(id, panel)}");
                        ++panel;
                    }
                    if (canImportPage) {
                        hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(id, panel, "t_importPage")}
        {await HtmlHelper.ForViewAsync($"{Package.AreaName}_ImportPage", module, model.ImportPageModel)}
    {PropertyListComponentBase.RenderTabPaneEnd(id, panel)}");
                        ++panel;
                    }
                    if (canImportModule) {
                        hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(id, panel, "t_importMod")}
        {await HtmlHelper.ForViewAsync($"{Package.AreaName}_ImportModule", module, model.ImportModuleModel)}
    {PropertyListComponentBase.RenderTabPaneEnd(id, panel)}");
                        ++panel;
                    }
                    if (canChangeSiteSkins) {
                        hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(id, panel, "t_addSkins")}
        {await HtmlHelper.ForViewAsync($"{Package.AreaName}_SkinSelection", module, model.SkinSelectionModel)}
    {PropertyListComponentBase.RenderTabPaneEnd(id, panel)}");
                        ++panel;
                    }
                }

                if (canOtherUserLogin) {
                    hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(id, panel, "t_Login")}
        {await HtmlHelper.ForViewAsync($"{Package.AreaName}_LoginSiteSelection", module, model.LoginSiteSelectionModel)}
    {PropertyListComponentBase.RenderTabPaneEnd(id, panel)}");
                    ++panel;
                }
            }

            hb.Append($@"
    {await HtmlHelper.ForDisplayAsync(model, nameof(model.Actions))}
</div>
{await PropertyListComponentBase.RenderTabInitAsync(id, null)}");

            return hb.ToString();
        }
    }
}
