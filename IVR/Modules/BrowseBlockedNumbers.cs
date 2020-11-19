/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Softelvdm.Modules.IVR.Controllers;
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
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace Softelvdm.Modules.IVR.Modules {

    public class BrowseBlockedNumbersModuleDataProvider : ModuleDefinitionDataProvider<Guid, BrowseBlockedNumbersModule>, IInstallableModel { }

    [ModuleGuid("{9a34582b-631a-4d6f-9557-e2b08228c254}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class BrowseBlockedNumbersModule : ModuleDefinition {

        public BrowseBlockedNumbersModule() {
            Title = this.__ResStr("modTitle", "Blocked Numbers");
            Name = this.__ResStr("modName", "Blocked Numbers");
            Description = this.__ResStr("modSummary", "Displays and manages blocked numbers.");
            DefaultViewName = StandardViews.PropertyListEdit;
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new BrowseBlockedNumbersModuleDataProvider(); }

        [Category("General"), Caption("Add Url"), Description("The Url to add a new blocked number - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string AddUrl { get; set; }
        [Category("General"), Caption("Edit Url"), Description("The Url to edit a blocked number - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string EditUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("RemoveItems",
                        this.__ResStr("roleRemItemsC", "Remove Blocked Numbers"), this.__ResStr("roleRemItems", "The role has permission to remove individual blocked numbers"),
                        this.__ResStr("userRemItemsC", "Remove Blocked Numbers"), this.__ResStr("userRemItems", "The user has permission to remove individual blocked numbers")),
                };
            }
        }

        public override async Task<MenuList> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = await base.GetModuleMenuListAsync(renderMode, location);
            AddBlockedNumberModule mod = new AddBlockedNumberModule();
            menuList.New(mod.GetAction_Add(AddUrl), location);
            return menuList;
        }

        public ModuleAction GetAction_Items(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Blocked Numbers"),
                MenuText = this.__ResStr("browseText", "Blocked Numbers"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage blocked numbers"),
                Legend = this.__ResStr("browseLegend", "Displays and manages blocked numbers"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public ModuleAction GetAction_Remove(int id) {
            if (!IsAuthorized("RemoveItems")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(BrowseBlockedNumbersModuleController), nameof(BrowseBlockedNumbersModuleController.Remove)),
                NeedsModuleContext = true,
                QueryArgs = new { Id = id },
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeLink", "Remove"),
                MenuText = this.__ResStr("removeMenu", "Remove"),
                Tooltip = this.__ResStr("removeTT", "Remove the blocked number"),
                Legend = this.__ResStr("removeLegend", "Removes the blocked number"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove this blocked number?"),
            };
        }
    }
}
