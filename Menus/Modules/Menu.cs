/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

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
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Menus.DataProvider;

namespace YetaWF.Modules.Menus.Modules;

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
        Orientation = MenuComponentBase.OrientationEnum.Horizontal;
        SmallMenuMaxWidth = 750;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new MenuModuleDataProvider(); }

    [Data_Binary, CopyAttribute]
    [Obsolete("Do not use directly - use GetMenu()/SaveMenu() instead - preserved for data conversion (pre 1.1.1)")]
    public MenuList? Menu { get; set; }

    [Data_NewValue]
    public long MenuVersion { get; set; }

    public void NewMenuVersion() { MenuVersion++; }

    public async Task<MenuList> GetMenuAsync() {
        using (MenuInfoDataProvider menuInfoDP = new MenuInfoDataProvider()) {
#pragma warning disable 0618 // Type or member is obsolete
            MenuList? menu = Menu;
#pragma warning restore 0618 // Type or member is obsolete
            if (menu != null) {
                await SaveMenuAsync(menu); // Legacy: the menu was saved as part of module definition, move it to MenuInfoDataProvider
            } else {
                MenuInfo? menuInfo = await menuInfoDP.GetItemAsync(ModuleGuid);
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
            MenuList? menu = Menu;
#pragma warning restore 0618 // Type or member is obsolete
            await menuInfoDP.ReplaceItemAsync(new MenuInfo {
                ModuleGuid = ModuleGuid,
                Menu = newMenu,
            });
            // get a fresh copy of the module definitions
            MenuModule? menuMod = (MenuModule?) await ModuleDefinition.LoadAsync(ModuleGuid);
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
    [Data_NewValue]
    public string? EditUrl { get; set; }

    [Category("General"), Caption("Orientation"), Description("Defines the orientation of the menu.")]
    [UIHint("Enum")]
    [Data_NewValue]
    public MenuComponentBase.OrientationEnum Orientation { get; set; }

    [Category("General"), Caption("Vertical Menu Width"), Description("Defines width of the menu (Vertical Orientation only).")]
    [UIHint("IntValue4"), Range(0, 9999)]
    [ProcessIf(nameof(Orientation), MenuComponentBase.OrientationEnum.Vertical, Disable = true), RequiredIf(nameof(Orientation), MenuComponentBase.OrientationEnum.Vertical)]
    [Data_NewValue]
    public int VerticalWidth { get; set; }

    [Category("General"), Caption("Small Screen Maximum Size"), Description("Defines the largest screen size where the small menu is used - If the screen is wider, the large menu is shown.")]
    [UIHint("IntValue4"), Range(0, 999999), Required]
    [Data_NewValue]
    public int SmallMenuMaxWidth { get; set; }

    [Category("General"), Caption("<LI> Css Class"), Description("The optional Css class added to every <LI> tag in the menu")]
    [UIHint("Text40"), StringLength(MaxLICssClass)]
    [Data_NewValue]
    public string? LICssClass { get; set; }

    [Category("General"), Caption("Hover Delay"), Description("Specifies the delay (in milliseconds) before the menu is closed - Used to avoid accidental closure on leaving")]
    [UIHint("IntValue4"), Range(0, 10000)]
    [ProcessIf(nameof(Orientation), MenuComponentBase.OrientationEnum.Horizontal, Disable = true), RequiredIf(nameof(Orientation), MenuComponentBase.OrientationEnum.Horizontal)]
    [Data_NewValue]
    public int HoverDelay { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
        List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
        MenuEditModule mod = new MenuEditModule();
        menuList.New(mod.GetAction_Edit(EditUrl, ModuleGuid), location);
        return menuList;
    }

    public class MenuModel {
        [UIHint("Menu")]
        public MenuComponentBase.MenuData Menu { get; set; } = null!;
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        MenuModel model = new MenuModel {
            Menu = new MenuComponentBase.MenuData {
                MenuList = await GetMenu(),
                HoverDelay = HoverDelay,
                CssClass = CssClass,
                Orientation = Orientation,
                VerticalWidth = VerticalWidth,
                SmallMenuMaxWidth = SmallMenuMaxWidth,
                OpenOnHover = true,
                HorizontalAlign = MenuComponentBase.HorizontalAlignEnum.Right,
                WantArrows = true,
            },
        };
        model.Menu.MenuList.LICssClass = LICssClass;
        return await RenderAsync(model);
    }

    // Even with caching the main menu is a performance hit due to deserialization (turns out our Simple format is faster than JSON).
    // Researched caching HTML which doesn't work due to missing side effect like CSS loading (megamenu) when menu is not rendered.
    // Fortunately there is no performance hit when navigating within a SPA or for static pages.
    // The first full page load is also not a problem. It's mainly an issue when opening an additional tab in the browser (e.g., duplicate tab)
    // or stress testing the site with full page loads.

    /// <summary>
    /// Builds the menu for the current user based on all available authorizations.
    /// </summary>
    /// <param name="menu">The entire menu (includes entries that will be removed if they're not available/permitted for the current user.</param>
    /// <param name="moduleGuid">The module that owns the menu. Used as cache key.</param>
    /// <returns>A copy of the menu reduced to just the entries that are available/permitted for the current user.</returns>
    /// <remarks>
    /// The menu is cached in session settings because it is a costly operation to determine permissions for all entries.
    /// The full menu is only evaluated when switching between edit/view mode, when the user logs on/off or when the menu contents have changed.
    /// </remarks>
    protected async Task<MenuList> GetMenu(bool External = false) {
        MenuList.SavedCacheInfo? info = MenuList.GetCache(ModuleGuid);
        if (info == null || info.EditMode != Manager.EditMode || info.UserId != Manager.UserId || info.MenuVersion != MenuVersion) {
            info = new MenuList.SavedCacheInfo {
                EditMode = Manager.EditMode,
                UserId = Manager.UserId,
                Menu = await (await GetMenuAsync()).GetUserMenuAsync(),
                MenuVersion = MenuVersion,
            };

            // Add optional external links (this is a custom hack, not an official feature)
            if (External)
                await AddExternalLinksAsync(info.Menu);

            MenuList.SetCache(ModuleGuid, info);
        }
        return info.Menu;
    }

    private async Task AddExternalLinksAsync(List<ModuleAction> origList) {
        if (ExternalList == null) {
            List<ModuleAction> list = new List<ModuleAction>();
            string? listFile = WebConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "ExternalList");
            if (!string.IsNullOrWhiteSpace(listFile)) {
                List<string> lines = await FileSystem.FileSystemProvider.ReadAllLinesAsync(listFile);
                foreach (string line in lines) {
                    if (!string.IsNullOrWhiteSpace(line) && !line.Trim().StartsWith("#")) {
                        string[] parts = line.Trim().Split(new char[] { ',' }, 2);
                        if (parts.Length != 2)
                            throw new InternalError($"File {listFile} has an invalid menu entry - {line}");
                        string menu = parts[0].Trim();
                        string url = parts[1].Trim();
                        list.Add(new ModuleAction {
                            LinkText = menu,
                            MenuText = menu,
                            Mode = ModuleAction.ActionModeEnum.Any,
                            Url = url,
                            Style = ModuleAction.ActionStyleEnum.NewWindow,
                        });
                    }
                }
            }
            ExternalList = list;
        }
        origList.AddRange(ExternalList);
    }
    private static List<ModuleAction>? ExternalList { get; set; }
}