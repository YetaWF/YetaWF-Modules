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
        {await RenderBeginFormAsync()}
            {await PartialForm(async () => new YHtmlString(await HtmlHelper.ForEditContainerAsync(model.AddNewPageModel, "PropertyList")), UsePartialFormCss: false)}
            <div class='t_detailsbuttons yNoPrint'>
                <input type='submit' value='{this.__ResStr("addNewPage", "Add")}' />
            </div>
        {await RenderEndFormAsync()}
    {PropertyListComponentBase.RenderTabPaneEnd(DivId, panel)}");
                ++panel;
            }

            if (!Manager.CurrentPage.Temporary) {
                if (model.EditAuthorized) {

                    hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(DivId, panel, "t_addNewMod")}
        {await RenderBeginFormAsync()}
            {await PartialForm(async () => new YHtmlString(await HtmlHelper.ForEditContainerAsync(model.AddNewModel, "PropertyList")), UsePartialFormCss: false)}
            <div class='t_detailsbuttons yNoPrint'>
                <input type='submit' value='{this.__ResStr("addNewModule", "Add")}' />
            </div>
        {await RenderEndFormAsync()}
    {PropertyListComponentBase.RenderTabPaneEnd(DivId, panel)}");
                ++panel;

                    hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(DivId, panel, "t_addExistingMod")}
        {await RenderBeginFormAsync()}
            {await PartialForm(async () => new YHtmlString(await HtmlHelper.ForEditContainerAsync(model.AddExistingModel, "PropertyList")), UsePartialFormCss: false)}
            <div class='t_detailsbuttons yNoPrint'>
                <input type='submit' value='{this.__ResStr("addOldModule", "Add")}' />
            </div>
        {await RenderEndFormAsync()}
    {PropertyListComponentBase.RenderTabPaneEnd(DivId, panel)}");
                    ++panel;

                    hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(DivId, panel, "t_importPage")}
        {await RenderBeginFormAsync()}
            {await PartialForm(async () => new YHtmlString(await HtmlHelper.ForEditContainerAsync(model.ImportPageModel, "PropertyList")), UsePartialFormCss: false)}
        {await RenderEndFormAsync()}
    {PropertyListComponentBase.RenderTabPaneEnd(DivId, panel)}");
                    ++panel;

                    hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(DivId, panel, "t_importMod")}
        {await RenderBeginFormAsync()}
            {await PartialForm(async () => new YHtmlString(await HtmlHelper.ForEditContainerAsync(model.ImportModuleModel, "PropertyList")), UsePartialFormCss: false)}
        {await RenderEndFormAsync()}
    {PropertyListComponentBase.RenderTabPaneEnd(DivId, panel)}");
                    ++panel;

                    hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(DivId, panel, "t_addSkins")}
        {await RenderBeginFormAsync()}
            {await PartialForm(async () => new YHtmlString(await HtmlHelper.ForEditContainerAsync(model.SkinSelectionModel, "PropertyList")), UsePartialFormCss: false)}
            <div class='t_detailsbuttons yNoPrint'>
                <input type='submit' value='{this.__ResStr("saveSkins", "Save & Display")}' />
            </div>
        {await RenderEndFormAsync()}
    {PropertyListComponentBase.RenderTabPaneEnd(DivId, panel)}");
                    ++panel;
                }

                hb.Append($@"
    {PropertyListComponentBase.RenderTabPaneStart(DivId, panel, "t_Login")}
        {await RenderBeginFormAsync()}
            {await PartialForm(async () => new YHtmlString(await HtmlHelper.ForEditContainerAsync(model.LoginSiteSelectionModel, "PropertyList")), UsePartialFormCss: false)}
        {await RenderEndFormAsync()}
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
