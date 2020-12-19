/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Pages.Controllers;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Pages.Modules {

    public class UnifiedSetsBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, UnifiedSetsBrowseModule>, IInstallableModel { }

    [ModuleGuid("{da479b99-d296-4fbb-8f2b-d7abec2e69d8}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class UnifiedSetsBrowseModule : ModuleDefinition {

        public UnifiedSetsBrowseModule() {
            Title = this.__ResStr("modTitle", "Unified Page Sets");
            Name = this.__ResStr("modName", "Unified Page Sets");
            Description = this.__ResStr("modSummary", "Displays and manages Unified Page Sets. It can be accessed using Admin > Panel > Unified Page Sets (standard YetaWF site).");
            DefaultViewName = StandardViews.PropertyListEdit;
            ShowHelp = true;
            WantSearch = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new UnifiedSetsBrowseModuleDataProvider(); }

        [Category("General"), Caption("Add Url"), Description("The Url to add a new Unified Page Set - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string AddUrl { get; set; }
        [Category("General"), Caption("Edit Url"), Description("The Url to edit a Unified Page Set - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string EditUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("RemoveItems",
                        this.__ResStr("roleRemItemsC", "Remove Unified Page Sets"), this.__ResStr("roleRemItems", "The role has permission to remove individual Unified Page Sets"),
                        this.__ResStr("userRemItemsC", "Remove Unified Page Sets"), this.__ResStr("userRemItems", "The user has permission to remove individual Unified Page Sets")),
                };
            }
        }

        public override async Task<MenuList> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = await base.GetModuleMenuListAsync(renderMode, location);
            UnifiedSetAddModule mod = new UnifiedSetAddModule();
            menuList.New(mod.GetAction_Add(AddUrl), location);
            return menuList;
        }

        public ModuleAction GetAction_Items(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Unified Page Sets"),
                MenuText = this.__ResStr("browseText", "Unified Page Sets"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage Unified Page Sets"),
                Legend = this.__ResStr("browseLegend", "Displays and manages Unified Page Sets"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public ModuleAction GetAction_Remove(Guid unifiedSetGuid, string name) {
            if (YetaWF.Core.Support.Startup.MultiInstance) return null;
            if (!IsAuthorized("RemoveItems")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(UnifiedSetsBrowseModuleController), "Remove"),
                NeedsModuleContext = true,
                QueryArgs = new { UnifiedSetGuid = unifiedSetGuid },
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeLink", "Remove"),
                MenuText = this.__ResStr("removeMenu", "Remove"),
                Tooltip = this.__ResStr("removeTT", "Remove the Unified Page Set"),
                Legend = this.__ResStr("removeLegend", "Removes the Unified Page Set"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove Unified Page Set \"{0}\"?", name),
            };
        }
    }
}
