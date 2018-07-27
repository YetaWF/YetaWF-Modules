/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.PageEdit.Controllers;
using YetaWF.Modules.PageEdit.Modules;

namespace YetaWF.Modules.PageEdit.Views {

    public class PageControlView : YetaWFView, IYetaWFView<PageControlModule, PageControlModuleController.PageControlModel> {

        public const string ViewName = "PageControl";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(PageControlModule module, PageControlModuleController.PageControlModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div id='{DivId}'>
    {PropertyListComponentBase.RenderTabStripStart(DivId)}");

            int tabEntry = 0;
            if (model.EditAuthorized) {
                hb.Append(PropertyListComponentBase.RenderTabEntry(DivId, this.__ResStr("tabNewPage", "New Page"), this.__ResStr("tabNewPageTT", "Add a new page to the site"), tabEntry));
                ++tabEntry;
            }
            if (!Manager.CurrentPage.Temporary) {
                if (model.EditAuthorized) {
                    hb.Append(PropertyListComponentBase.RenderTabEntry(DivId, this.__ResStr("tabNew", "New Module"), this.__ResStr("tabNewTT", "Add a new module to this page (creates a new module)"), tabEntry));
                    ++tabEntry;

                    hb.Append(PropertyListComponentBase.RenderTabEntry(DivId, this.__ResStr("tabOld", "Existing Module"), this.__ResStr("tabOldTT", "Add an existing module to this page (this does not copy the module)"), tabEntry));
                    ++tabEntry;
                    hb.Append(PropertyListComponentBase.RenderTabEntry(DivId, this.__ResStr("tabImportPage", "Import Page"), this.__ResStr("tabImportPageTT", "Import a page (creates a new page)"), tabEntry));
                    ++tabEntry;
                    hb.Append(PropertyListComponentBase.RenderTabEntry(DivId, this.__ResStr("tabImportModule", "Import Module"), this.__ResStr("tabImportModuleTT", "Import module data into this page (creates a new module)"), tabEntry));
                    ++tabEntry;
                    hb.Append(PropertyListComponentBase.RenderTabEntry(DivId, this.__ResStr("tabSkins", "Skins"), this.__ResStr("tabSkinsTT", "Change default skins used site wide"), tabEntry));
                    ++tabEntry;
                }
                hb.Append(PropertyListComponentBase.RenderTabEntry(DivId, this.__ResStr("tabLogin", "Login"), this.__ResStr("tabLoginTT", "Change site or log in as another user"), tabEntry));
                ++tabEntry;
            }
            hb.Append($@"
    {PropertyListComponentBase.RenderTabStripEnd(DivId)}");

            int panel = 0;
            if (model.EditAuthorized) {
                hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(DivId, panel, "t_addNewPage")}
        {await HtmlHelper.ForViewAsync($"{Package.AreaName}_AddNewPage", module, model.AddNewPageModel)}
    {PropertyListComponentBase.RenderTabPaneEnd(DivId, panel)}");
                ++panel;
            }

            if (!Manager.CurrentPage.Temporary) {
                if (model.EditAuthorized) {

                    hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(DivId, panel, "t_addNewMod")}
        {await HtmlHelper.ForViewAsync($"{Package.AreaName}_AddNewModule", module, model.AddNewModel)}
    {PropertyListComponentBase.RenderTabPaneEnd(DivId, panel)}");
                ++panel;

                    hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(DivId, panel, "t_addExistingMod")}
        {await HtmlHelper.ForViewAsync($"{Package.AreaName}_AddExistingModule", module, model.AddExistingModel)}
    {PropertyListComponentBase.RenderTabPaneEnd(DivId, panel)}");
                    ++panel;

                    hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(DivId, panel, "t_importPage")}
        {await HtmlHelper.ForViewAsync($"{Package.AreaName}_ImportPage", module, model.ImportPageModel)}
    {PropertyListComponentBase.RenderTabPaneEnd(DivId, panel)}");
                    ++panel;

                    hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(DivId, panel, "t_importMod")}
        {await HtmlHelper.ForViewAsync($"{Package.AreaName}_ImportModule", module, model.ImportModuleModel)}
    {PropertyListComponentBase.RenderTabPaneEnd(DivId, panel)}");
                    ++panel;

                    hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(DivId, panel, "t_addSkins")}
        {await HtmlHelper.ForViewAsync($"{Package.AreaName}_SkinSelection", module, model.SkinSelectionModel)}
    {PropertyListComponentBase.RenderTabPaneEnd(DivId, panel)}");
                    ++panel;
                }

                hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(DivId, panel, "t_Login")}
        {await HtmlHelper.ForViewAsync($"{Package.AreaName}_LoginSiteSelection", module, model.LoginSiteSelectionModel)}
    {PropertyListComponentBase.RenderTabPaneEnd(DivId, panel)}");
                ++panel;
            }

            hb.Append($@"
</div>
{await PropertyListComponentBase.RenderTabInitAsync(DivId, null)}");

            return hb.ToYHtmlString();
        }
    }
}
