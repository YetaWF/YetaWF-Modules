/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using System;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Menus.Modules;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Core;
using System.Collections.Generic;
using System.Linq;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Menus.Controllers {

    public class MenuEditModuleController : ControllerImpl<YetaWF.Modules.Menus.Modules.MenuEditModule> {

        public class MenuEntry : TreeEntry {

            [Caption("Entry Type"), Description("The type of the menu entry")]
            [UIHint("Enum")]
            public ModuleAction.MenuEntryType EntryType {
                get {
                    if (Separator) return ModuleAction.MenuEntryType.Separator;
                    if (string.IsNullOrWhiteSpace(Url) && SubModule == null) return ModuleAction.MenuEntryType.Parent;
                    return ModuleAction.MenuEntryType.Entry;
                }
                set {
                    switch (value) {
                        case ModuleAction.MenuEntryType.Parent:
                            Url = "";
                            Separator = false;
                            break;
                        case ModuleAction.MenuEntryType.Separator:
                            Separator = true;
                            break;
                        default:
                            break;
                    }
                }
            }

            public bool Separator { get; set; } // gap (if used, all other properties are ignored)

            [Caption("Url"), Description("The Url")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
            [RequiredIf(nameof(EntryType), (int)ModuleAction.MenuEntryType.Entry)]
            [StringLength(Globals.MaxUrl), Trim]
            public string Url { get; set; } // The Url to cause this action

            [Caption("SubModule"),
                Description("The submodule is displayed as a complete submenu - " +
                "If a submodule is defined it replaces the entire submenu. " +
                "Submodules should not display forms as any popup message due to invalid input would close the submenu. " +
                "This is best used to display formatted links or images, etc. with a Text module. " +
                "Submodules are only supported with Bootstrap skins and are ignored on non-Bootstrap skins")]
            [UIHint("ModuleSelection")]
            public Guid? SubModule { get; set; }

            [Caption("Menu Text"), Description("The text shown for this menu entry when used as a menu entry")]
            [UIHint("MultiString20"), StringLength(ModuleAction.MaxMenuText), RequiredIfNotAttribute("EntryType", (int)ModuleAction.MenuEntryType.Separator)]
            public MultiString MenuText { get; set; }

            [Caption("Link Text"), Description("The text shown for this menu entry when used as a link")]
            [UIHint("MultiString20"), StringLength(ModuleAction.MaxLinkText), RequiredIfNotAttribute("EntryType", (int)ModuleAction.MenuEntryType.Separator)]
            public MultiString LinkText { get; set; }

            [Caption("Image URL"), Description("The URL of the image shown for this entry")]
            [UIHint("Text80"), StringLength(Globals.MaxUrl)]
            public string ImageUrlFinal { get; set; }

            [Caption("Tooltip"), Description("The tooltip for this entry")]
            [UIHint("MultiString40"), StringLength(ModuleAction.MaxTooltip)]
            public MultiString Tooltip { get; set; }

            [Caption("Legend"), Description("The legend for this entry")]
            [UIHint("MultiString40"), StringLength(ModuleAction.MaxLegend)]
            public MultiString Legend { get; set; }

            [Caption("Enabled"), Description("Defines whether this entry is enabled - Disabled entries are automatically hidden in menus")]
            [UIHint("Boolean")]
            public bool Enabled { get; set; }

            [Caption("Css Class"), Description("The optional CSS class added to the action")]
            [UIHint("Text40"), StringLength(ModuleAction.MaxCssClass), Trim]
            public string CssClass { get; set; }

            [Caption("Style"), Description("Defines how this action affects the current window")]
            [UIHint("Enum"), RequiredIfAttribute("EntryType", (int)ModuleAction.MenuEntryType.Entry)]
            public ModuleAction.ActionStyleEnum Style { get; set; } // how the action affects the current window

            [Caption("Mode"), Description("Defines where this entry is available")]
            [UIHint("Enum"), RequiredIfAttribute("EntryType", (int)ModuleAction.MenuEntryType.Entry)]
            public ModuleAction.ActionModeEnum Mode { get; set; } // in which page mode the action is available

            [Caption("Category"), Description("The type of action this entry takes")]
            [UIHint("Enum"), RequiredIfAttribute("EntryType", (int)ModuleAction.MenuEntryType.Entry)]
            public ModuleAction.ActionCategoryEnum Category { get; set; } // the type of action taken

            [Caption("Limit To Role"), Description("Defines which role must be present for this action to be shown - This is normally only used for specialized entries which should only be shown in some cases but are available (by Url) to other roles also - This setting is ignored for superusers")]
            [UIHint("YetaWF_Identity_RoleId"), AdditionalMetadata("ShowDefault", true)]
            public int LimitToRole { get; set; } // the type of action taken

            [Caption("Ignore Authorization"), Description("Defines whether the target page's authorization is ignored - Actions are only visible if the user has sufficient authorization to perform the action - This can be used to force display of actions even when there is insufficient authoriation - For anonymous users this forces the user to log in first - This setting is ignored for links to other sites")]
            [UIHint("Boolean")]
            public bool AuthorizationIgnore { get; set; }

            [Caption("Confirmation Text"), Description("The confirmation text displayed before the action takes place")]
            [UIHint("MultiString"), StringLength(ModuleAction.MaxConfirmationText)]
            public MultiString ConfirmationText { get; set; } // confirmation popup text before action takes place

            [Caption("Please Wait"), Description("If specified, the \"Please Wait\" dialog is shown when the action is selected - Only available for actions with Style=Normal")]
            [UIHint("MultiString"), StringLength(ModuleAction.MaxPleaseWaitText)]
            public MultiString PleaseWaitText { get; set; }

            [Caption("Save Return Url"), Description("Defines whether this action will preserve the origin list (past Urls visited) and save the current Url - This is typically used for actions that display a form with a Cancel or Return button, which would return to the current Url if Save Return Url is selected")]
            [UIHint("Boolean")]
            public bool SaveReturnUrl { get; set; }

            [Caption("Add to Origin List"), Description("Defines whether the current Url will be added to the origin list so we can return to the current Url - This is used in conjunction with the Save Return Url property - If the Save Return Url property is false, the AddToOriginList property is ignored")]
            [UIHint("Boolean")]
            public bool AddToOriginList { get; set; }

            [Caption("Needs Module Context"), Description("The whether module context is required for authorization purposes - This is defined by the action")]
            [UIHint("Boolean")]
            public bool NeedsModuleContext { get; set; }

            [Caption("Don't Follow"), Description("Defines whether search engines and bots follow this link (select to disable)")]
            [UIHint("Boolean")]
            public bool DontFollow { get; set; }

            // Menu Editing

            [UIHint("String"), ReadOnly]
            public override string Text { get { return MenuText.ToString(); } }

            public MenuEntry() {
                MenuText = new MultiString();
                LinkText = new MultiString();
                Tooltip = new MultiString();
                Legend = new MultiString();
                ConfirmationText = new MultiString();
                PleaseWaitText = new MultiString();
            }
        }

        public class MenuEditModel {

            public MenuEntry NewEntry { get; set; } // one new menu entry (used client-side)

            [UIHint("Tree")]
            public List<MenuEntry> Menu { get; set; }
            public TreeDefinition Menu_TreeDefinition { get; set; }

            [UIHint("Hidden")]
            public Guid MenuGuid { get; set; }
            [UIHint("Hidden")]
            public long MenuVersion { get; set; }

            [UIHint("PropertyList")]
            public MenuEntry ModEntry { get; set; }

            public MenuEditModel() {
                Menu_TreeDefinition = new TreeDefinition {
                    RecordType = typeof(ModuleAction),
                    ShowHeader = false,
                    DragDrop = true,
                    UseSkinFormatting = true,
                    JSONData = true,
                };
                ModEntry = new MenuEntry();
            }
        }

        [AllowGet]
        public async Task<ActionResult> MenuEdit(Guid menuGuid) {
            MenuModule modMenu = (MenuModule) await ModuleDefinition.LoadAsync(menuGuid);
            if (modMenu == null)
                throw new InternalError("Can't find menu module {0}", menuGuid);

            MenuList origMenu = await modMenu.GetMenuAsync();

            MenuEditModel model = new MenuEditModel {
                Menu = GetMenuEntries(origMenu),
                NewEntry = GetMenuEntry(new ModuleAction(Module) { Url = "" }),

                MenuGuid = menuGuid,
                ModEntry = new MenuEntry(),
                MenuVersion = modMenu.MenuVersion,
            };
            return View(model);
        }

        private List<MenuEntry> GetMenuEntries(List<ModuleAction> origMenu) {
            List<MenuEntry> entries = new List<MenuEntry>();
            foreach (ModuleAction moduleAction in origMenu) {
                MenuEntry entry = GetMenuEntry(moduleAction);
                entries.Add(entry);
                if (moduleAction.SubMenu != null && moduleAction.SubMenu.Count > 0) {
                    List<MenuEntry> subEntries = GetMenuEntries(moduleAction.SubMenu);
                    entry.SubEntries = (from s in subEntries select (TreeEntry)s).ToList();
                }
            }
            return entries;
        }
        private MenuEntry GetMenuEntry(ModuleAction moduleAction) {
            MenuEntry entry = new MenuEntry();
            ObjectSupport.CopyData(moduleAction, entry);
            return entry;
        }


        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> MenuEdit_Partial(MenuEditModel model, bool ValidateCurrent) {

            MenuModule modMenu = (MenuModule) await ModuleDefinition.LoadAsync(model.MenuGuid);
            if (modMenu == null)
                throw new InternalError("Can't find menu module {0}", model.MenuGuid);

            if (model.MenuVersion != modMenu.MenuVersion)
                throw new Error(this.__ResStr("menuChanged", "The menu has been changed by someone else - Your changes can't be saved - Please refresh the current page before proceeding"));

            if (!ValidateCurrent)
                ModelState.Clear();
            if (!ModelState.IsValid)
                return PartialView(model);

            return PartialView(model);
        }

        public class EntireMenuResult {
            public long NewVersion { get; set; }
        }

        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> EntireMenu(string entireMenu, Guid menuGuid, long menuVersion) {
            MenuModule modMenu = (MenuModule)await ModuleDefinition.LoadAsync(menuGuid);
            if (modMenu == null)
                throw new InternalError("Can't find menu module {0}", menuGuid);
            if (menuVersion != modMenu.MenuVersion)
                throw new Error(this.__ResStr("menuChanged", "The menu has been changed by someone else - Your changes can't be saved - Please refresh the current page before proceeding"));

            MenuList origMenu = await modMenu.GetMenuAsync();
            MenuList menu = DeserializeFromJSON(entireMenu);
            await modMenu.SaveMenuAsync(menu);

            return new YJsonResult() {
                Data = new EntireMenuResult {
                    NewVersion = modMenu.MenuVersion
                }
            };
        }
        private static MenuList DeserializeFromJSON(string menuJSON) {
            List<ModuleAction> actions = (List<ModuleAction>)Utility.JsonDeserialize(menuJSON, typeof(List<ModuleAction>));
            // fix some settings that aren't updated on the browser side
            MenuList menu = new MenuList(actions);
            return menu;
        }
        private static void FixMenuEntries(List<ModuleAction> actions) {
            foreach (ModuleAction action in actions) {
                if (action.SubModule == Guid.Empty)
                    action.SubModule = null;
                action.Location = ModuleAction.ActionLocationEnum.AnyMenu;
                action.NeedsModuleContext = false;
                if (action.Separator) {
                    // separator without real action
                    action.Url = null;
                    action.SubModule = null;
                    action.MenuText = new MultiString();
                    action.LinkText = new MultiString();
                    action.ImageUrlFinal = null;
                    action.Tooltip = new MultiString();
                    action.Legend = new MultiString();
                    action.Style = ModuleAction.ActionStyleEnum.Normal;
                    action.Mode = ModuleAction.ActionModeEnum.Any;
                    action.Category = ModuleAction.ActionCategoryEnum.Read;
                    action.LimitToRole = 0;
                    action.AuthorizationIgnore = false;
                    action.ConfirmationText = new MultiString();
                    action.PleaseWaitText = new MultiString();
                    action.SaveReturnUrl = false;
                    action.AddToOriginList = false;
                    action.DontFollow = false;
                } else if (string.IsNullOrWhiteSpace(action.Url) && action.SubModule == null) {
                    // parent item without real action
                    action.SubModule = null;
                    action.Separator = false;
                    action.Style = ModuleAction.ActionStyleEnum.Normal;
                    action.Mode = ModuleAction.ActionModeEnum.Any;
                    action.Category = ModuleAction.ActionCategoryEnum.Read;
                    action.ConfirmationText = new MultiString();
                    action.PleaseWaitText = new MultiString();
                    action.SaveReturnUrl = false;
                    action.AddToOriginList = false;
                    action.DontFollow = false;
                }
                if (action.SubMenu != null && action.SubMenu.Count > 0)
                    FixMenuEntries(action.SubMenu);
            }
        }
    }
}