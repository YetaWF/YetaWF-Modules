/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Languages#License */

using System;
using System.Collections.Generic;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.DataProvider;
using YetaWF.Modules.Languages.Controllers;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Languages.Modules {

    public class LanguagesBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, LanguagesBrowseModule>, IInstallableModel { }

    [ModuleGuid("{0ce1d3eb-6f43-44ad-acf0-4590652f9012}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class LanguagesBrowseModule : ModuleDefinition {

        public LanguagesBrowseModule() : base() {
            Title = this.__ResStr("modTitle", "Languages");
            Name = this.__ResStr("modName", "Languages");
            Description = this.__ResStr("modSummary", "Displays and manages languages");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new LanguagesBrowseModuleDataProvider(); }

        [Category("General"), Caption("Add URL"), Description("The URL to add a new language - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string AddUrl { get; set; }
        [Category("General"), Caption("Display URL"), Description("The URL to display a language - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string DisplayUrl { get; set; }
        [Category("General"), Caption("Edit URL"), Description("The URL to edit a language - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string EditUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return SuperuserLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() { 
                    new RoleDefinition("RemoveLanguages", 
                        this.__ResStr("roleRemItemsC", "Remove Languages"), this.__ResStr("roleRemItems", "The role has permission to remove individual languages"), 
                        this.__ResStr("userRemItemsC", "Remove Languages"), this.__ResStr("userRemItems", "The user has permission to remove individual languages")),
                };
            }
        }

        public override MenuList GetModuleMenuList(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = base.GetModuleMenuList(renderMode, location);
            LanguageAddModule mod = new LanguageAddModule();
            menuList.New(mod.GetAction_Add(AddUrl), location);
            return menuList;
        }

        public ModuleAction GetAction_Languages(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Languages"),
                MenuText = this.__ResStr("browseText", "Languages"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage languages"),
                Legend = this.__ResStr("browseLegend", "Displays and manages languages"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public ModuleAction GetAction_RemoveLanguage(string Id) {
            if (!IsAuthorized("RemoveLanguages")) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(LanguagesBrowseModuleController), "RemoveLanguage"),
                QueryArgs = new { Id = Id },
                NeedsModuleContext = true,
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeLink", "Remove language {0}", Id),
                MenuText = this.__ResStr("removeMenu", "Remove language {0}", Id),
                Legend = this.__ResStr("removeLegend", "Removes the language"),
                Tooltip = this.__ResStr("removeTT", "Removes the language"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove language \"{0}\"?", Id),
            };
        }
    }
}