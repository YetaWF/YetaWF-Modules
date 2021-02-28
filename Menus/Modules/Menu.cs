/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Menus.DataProvider;

namespace YetaWF.Modules.Menus.Modules {

    public class MenuModuleDataProvider : ModuleDefinitionDataProvider<Guid, MenuModule>, IInstallableModel { }

    [ModuleGuid("{51E5EB91-56CF-4ad6-A0D9-5C084FFD5D3F}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class MenuModule : ModuleDefinition, IModuleMenuAsync {

        private const int MaxLICssClass = 80;

        public MenuModule() {
            Name = this.__ResStr("modName", "Menu");
            Description = this.__ResStr("modSummary", "Simple and main page menus");
#pragma warning disable 0618 // Type or member is obsolete
            Menu = null;
#pragma warning restore 0618 // Type or member is obsolete
            MenuVersion = 0;
            HoverDelay = 500;
            ShowTitle = false;
            WantSearch = false;
            WantFocus = false;
            Print = false;
            SmallMenuMaxWidth = 750;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new MenuModuleDataProvider(); }

        [Data_Binary, CopyAttribute]
        [Obsolete("Do not use directly - use GetMenu()/SaveMenu() instead - preserved for data conversion (pre 1.1.1)")]
        public MenuList Menu { get; set; }

        [Data_NewValue]
        public long MenuVersion { get; set; }

        public void NewMenuVersion() { MenuVersion = MenuVersion+1; }

        public async Task<MenuList> GetMenuAsync() {
            using (MenuInfoDataProvider menuInfoDP = new MenuInfoDataProvider()) {
#pragma warning disable 0618 // Type or member is obsolete
                MenuList menu = Menu;
#pragma warning restore 0618 // Type or member is obsolete
                if (menu != null) {
                    await SaveMenuAsync(menu); // Legacy: the menu was saved as part of module definition, move it to MenuInfoDataProvider
                } else {
                    MenuInfo menuInfo = await menuInfoDP.GetItemAsync(ModuleGuid);
                    if (menuInfo != null)
                        menu = menuInfo.Menu;
                    else
                        menu = new MenuList();
                }
                return menu;
            }
        }
        public async Task SaveMenuAsync(MenuList newMenu) {
            using (MenuInfoDataProvider menuInfoDP = new MenuInfoDataProvider()) {
#pragma warning disable 0618 // Type or member is obsolete
                MenuList menu = Menu;
#pragma warning restore 0618 // Type or member is obsolete
                await menuInfoDP.ReplaceItemAsync(new MenuInfo {
                    ModuleGuid = ModuleGuid,
                    Menu = newMenu,
                });
                // get a fresh copy of the module definitions
                MenuModule menuMod = (MenuModule) await ModuleDefinition.LoadAsync(ModuleGuid);
                if (menuMod == null)
                    throw new InternalError("Menu module {0} was deleted", ModuleGuid);
                menuMod.NewMenuVersion();
                this.MenuVersion = menuMod.MenuVersion;
#pragma warning disable 0618 // Type or member is obsolete
                // the menu was saved as part of module definition, move it to MenuInfoDataProvider
                this.Menu = menuMod.Menu = null;// Legacy: clear menu saved as part of module definition
#pragma warning restore 0618 // Type or member is obsolete
                await menuMod.SaveAsync();// save module definition (without menu)
            }
        }

        [Category("General"), Caption("Edit Url"), Description("The Url used to edit this menu - If omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string EditUrl { get; set; }

        [Category("General"), Caption("Small Screen Maximum Size"), Description("Defines the largest screen size where the small menu is used - If the screen is wider, the large menu is shown.")]
        [UIHint("IntValue4"), Range(0, 999999), Required]
        [Data_NewValue]
        public int SmallMenuMaxWidth { get; set; }

        [Category("General"), Caption("<LI> Css Class"), Description("The optional Css class added to every <LI> tag in the menu")]
        [UIHint("Text40"), StringLength(MaxLICssClass)]
        public string LICssClass { get; set; }

        [Category("General"), Caption("Hover Delay"), Description("Specifies the delay (in milliseconds) before the menu is closed - Used to avoid accidental closure on leaving")]
        [UIHint("IntValue4"), Required, Range(0, 10000)]
        public int HoverDelay { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
            MenuEditModule mod = new MenuEditModule();
            menuList.New(mod.GetAction_Edit(EditUrl, ModuleGuid), location);
            return menuList;
        }
    }
}