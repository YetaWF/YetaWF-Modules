/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Menu.Endpoints;
using YetaWF.Modules.Menus.Modules;

namespace YetaWF.Modules.Menus.Views;

public class MenuEditView : YetaWFView, IYetaWFView2<MenuEditModule, MenuEditModule.MenuEditModel> {

    public const string ViewName = "MenuEdit";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public class Setup {
        public string TreeId { get; set; } = null!;
        public string DetailsId { get; set; } = null!;
        public string AjaxUrl { get; set; } = null!;
        public MenuEditModule.MenuEntry NewEntry { get; set; } = null!;
    }

    public async Task<string> RenderViewAsync(MenuEditModule module, MenuEditModule.MenuEditModel model) {

        HtmlBuilder hb = new HtmlBuilder();

        string treeId = DivId;
        string detailsId = UniqueId();

        Setup setup = new Setup {
            TreeId = treeId,
            DetailsId = detailsId,
            AjaxUrl = Utility.UrlFor(typeof(MenuEditModuleEndpoints), nameof(MenuEditModuleEndpoints.EntireMenu)),
            NewEntry = model.NewEntry,
        };

        hb.Append($@"
{await RenderBeginFormAsync()}
    <div>
        <div id='{treeId}' class='t_view'>
            <div class='t_treeview'>
                {await HtmlHelper.ForDisplayAsync(model, nameof(model.Menu))}
            </div>
        </div>
        <div id='{detailsId}' class='t_details'>
            {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
            {await FormButtonsAsync(new FormButton[] {
            new FormButton() { ButtonType = ButtonTypeEnum.Button, Name = "t_submit", Text = this.__ResStr("btnSave", "Save") },
            new FormButton() { ButtonType = ButtonTypeEnum.Button, Name = "t_reset", Text = this.__ResStr("btnReset", "Reset") },
            new FormButton() { ButtonType = ButtonTypeEnum.Button, Name = "t_insert", Text = this.__ResStr("btnInsert", "Insert Entry") },
            new FormButton() { ButtonType = ButtonTypeEnum.Button, Name = "t_add", Text = this.__ResStr("btnAdd", "Add Entry") },
            new FormButton() { ButtonType = ButtonTypeEnum.Button, Name = "t_delete", Text = this.__ResStr("btnDelete", "Delete") },
            new FormButton() { ButtonType = ButtonTypeEnum.Button, Name = "t_expandall", Text = this.__ResStr("btnExpandAll", "Expand All") },
            new FormButton() { ButtonType = ButtonTypeEnum.Button, Name = "t_collapseall", Text = this.__ResStr("btnCollapseAll", "Collapse All") },
            new FormButton() { ButtonType = ButtonTypeEnum.Cancel, Name = "t_return", Text = this.__ResStr("btnReturn", "Return") },
        })}
        </div>
        <div class='y_cleardiv'></div>
    </div>
{await RenderEndFormAsync()}");

        Manager.ScriptManager.AddLast($@"new YetaWF_Menus.MenuEditView({Utility.JsonSerialize(setup)});");

        return hb.ToString();
    }

    public async Task<string> RenderPartialViewAsync(MenuEditModule module, MenuEditModule.MenuEditModel model) {

        HtmlBuilder hb = new HtmlBuilder();
        hb.Append($@"
{await HtmlHelper.ForEditAsync(model, nameof(model.MenuGuid))}
{await HtmlHelper.ForEditAsync(model, nameof(model.MenuVersion))}
{await HtmlHelper.ForEditAsync(model, nameof(model.ModEntry))}");
        return hb.ToString();

    }
}
