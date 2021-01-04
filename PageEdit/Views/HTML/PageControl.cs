/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
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

        internal class UI {
            [UIHint("Tabs")]
            public TabsDefinition TabsDef { get; set; }
        }

        public async Task<string> RenderViewAsync(PageControlModule module, PageControlModuleController.PageControlModel model) {

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


            UI ui = new UI {
                TabsDef = new TabsDefinition()
            };
            if (canEdit) {
                if (canPageAdd) {
                    ui.TabsDef.Tabs.Add(new TabEntry {
                        Caption = this.__ResStr("tabNewPage", "New Page"),
                        ToolTip = this.__ResStr("tabNewPageTT", "Add a new page to the site"),
                        PaneCssClasses = "t_addNewPage",
                        RenderPaneAsync = async (int tabIndex) => {
                            return (await HtmlHelper.ForViewAsync($"{Package.AreaName}_AddNewPage", module, model.AddNewPageModel)).ToString();
                        },
                    });
                }
            }
            if (!Manager.CurrentPage.Temporary) {
                if (canEdit) {
                    if (canModuleNewAdd) {
                        ui.TabsDef.Tabs.Add(new TabEntry {
                            Caption = this.__ResStr("tabNew", "New Module"),
                            ToolTip = this.__ResStr("tabNewTT", "Add a new module to this page (creates a new module)"),
                            PaneCssClasses = "t_addNewMod",
                            RenderPaneAsync = async (int tabIndex) => {
                                return (await HtmlHelper.ForViewAsync($"{Package.AreaName}_AddNewModule", module, model.AddNewModel)).ToString();
                            },
                        });
                    }
                    if (canModuleExistingAdd) {
                        ui.TabsDef.Tabs.Add(new TabEntry {
                            Caption = this.__ResStr("tabOld", "Existing Module"),
                            ToolTip = this.__ResStr("tabOldTT", "Add an existing module to this page (this does not copy the module)"),
                            PaneCssClasses = "t_addExistingMod",
                            RenderPaneAsync = async (int tabIndex) => {
                                return (await HtmlHelper.ForViewAsync($"{Package.AreaName}_AddExistingModule", module, model.AddExistingModel)).ToString();
                            },
                        });
                    }
                    if (canImportPage) {
                        ui.TabsDef.Tabs.Add(new TabEntry {
                            Caption = this.__ResStr("tabImportPage", "Import Page"),
                            ToolTip = this.__ResStr("tabImportPageTT", "Import a page (creates a new page)"),
                            PaneCssClasses = "t_importPage",
                            RenderPaneAsync = async (int tabIndex) => {
                                return (await HtmlHelper.ForViewAsync($"{Package.AreaName}_ImportPage", module, model.ImportPageModel)).ToString();
                            },
                        });
                    }
                    if (canImportModule) {
                        ui.TabsDef.Tabs.Add(new TabEntry {
                            Caption = this.__ResStr("tabImportModule", "Import Module"),
                            ToolTip = this.__ResStr("tabImportModuleTT", "Import module data into this page (creates a new module)"),
                            PaneCssClasses = "t_importMod",
                            RenderPaneAsync = async (int tabIndex) => {
                                return (await HtmlHelper.ForViewAsync($"{Package.AreaName}_ImportModule", module, model.ImportModuleModel)).ToString();
                            },
                        });
                    }
                    if (canChangeSiteSkins) {
                        ui.TabsDef.Tabs.Add(new TabEntry {
                            Caption = this.__ResStr("tabSkins", "Skins"),
                            ToolTip = this.__ResStr("tabSkinsTT", "Change default skins used site wide"),
                            PaneCssClasses = "t_addSkins",
                            RenderPaneAsync = async (int tabIndex) => {
                                return (await HtmlHelper.ForViewAsync($"{Package.AreaName}_SkinSelection", module, model.SkinSelectionModel)).ToString();
                            },
                        });
                    }
                }
            }
            if (canOtherUserLogin) {
                ui.TabsDef.Tabs.Add(new TabEntry {
                    Caption = this.__ResStr("tabLogin", "Login"),
                    ToolTip = this.__ResStr("tabLoginTT", "Change site or log in as another user"),
                    PaneCssClasses = "t_login",
                    RenderPaneAsync = async (int tabIndex) => {
                        return (await HtmlHelper.ForViewAsync($"{Package.AreaName}_LoginSiteSelection", module, model.LoginSiteSelectionModel)).ToString();
                    },
                });
            }

            if (ui.TabsDef.Tabs.Count == 0)
                return "&nbsp;";

            // icon used: fas-tools
            string toolTip = this.__ResStr("pageControlTT", "Control Panel - Add new or existing modules, add new pages, switch to edit mode, access page settings and other site management tasks");

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"
<div id='yPageControlDiv'>
    <a class='t_controlpanel y_button_outline y_button' {Basics.CssTooltip}='{HAE(toolTip)}' href='javascript: void(0);' rel='nofollow' data-button=''>
        <svg aria-hidden='true' focusable='false' role='img' viewBox='0 0 512 512'>
            <path fill='currentColor' d='M501.1 395.7L384 278.6c-23.1-23.1-57.6-27.6-85.4-13.9L192 158.1V96L64 0 0 64l96 128h62.1l106.6 106.6c-13.6 27.8-9.2 62.3 13.9 85.4l117.1 117.1c14.6 14.6 38.2 14.6 52.7 0l52.7-52.7c14.5-14.6 14.5-38.2 0-52.7zM331.7 225c28.3 0 54.9 11 74.9 31l19.4 19.4c15.8-6.9 30.8-16.5 43.8-29.5 37.1-37.1 49.7-89.3 37.9-136.7-2.2-9-13.5-12.1-20.1-5.5l-74.4 74.4-67.9-11.3L334 98.9l74.4-74.4c6.6-6.6 3.4-17.9-5.7-20.2-47.4-11.7-99.6.9-136.6 37.9-28.5 28.5-41.9 66.1-41.2 103.6l82.1 82.1c8.1-1.9 16.5-2.9 24.7-2.9zm-103.9 82l-56.7-56.7L18.7 402.8c-25 25-25 65.5 0 90.5s65.5 25 90.5 0l123.6-123.6c-7.6-19.9-9.9-41.6-5-62.7zM64 472c-13.2 0-24-10.8-24-24 0-13.3 10.7-24 24-24s24 10.7 24 24c0 13.2-10.7 24-24 24z'></path>
        </svg>
    </a>
</div>
<div id='{id}'>
    {await HtmlHelper.ForDisplayAsync(ui, nameof(ui.TabsDef), HtmlAttributes: new { __NoTemplate = true })}
    {Globals.LazyHTMLOptimization /*Fix Firefox bug, where actions aren't broken into multiple lines, by leaving white space between actions*/}
    {await HtmlHelper.ForDisplayAsync(model, nameof(model.Actions))}
    {Globals.LazyHTMLOptimizationEnd}
</div>");

            return hb.ToString();
        }
    }
}
