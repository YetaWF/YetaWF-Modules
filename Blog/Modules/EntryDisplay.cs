﻿/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Views.Shared;
using YetaWF.DataProvider;
using YetaWF.Modules.Blog.DataProvider;
#if MVC6
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Blog.Modules {

    public class EntryDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, EntryDisplayModule>, IInstallableModel { }

    [ModuleGuid("{dc5acba8-fdf9-4146-abf3-8321b4fe8c7a}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class EntryDisplayModule : ModuleDefinition {

        public EntryDisplayModule() {
            Title = this.__ResStr("modTitle", "Blog Entry");
            Name = this.__ResStr("modName", "Display Blog Entry");
            Description = this.__ResStr("modSummary", "Displays an existing blog entry");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new EntryDisplayModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Category("General"), Caption("Edit URL"), Description("The URL to edit the displayed blog entry - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string EditUrl { get; set; }

        public override MenuList GetModuleMenuList(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = base.GetModuleMenuList(renderMode, location);
            EntryEditModule mod = new EntryEditModule();
            int blogEntry;
            if (Manager.TryGetUrlArg<int>("BlogEntry", out blogEntry))
                menuList.New(mod.GetAction_Edit(EditUrl, blogEntry), location);
            return menuList;
        }

        public ModuleAction GetAction_Display(int blogEntry, bool ReadMore = false) {
            string url = BlogConfigData.GetEntryCanonicalName(blogEntry);
            return new ModuleAction(this) {
                Url = url,
                Image = "#Display",
                LinkText = ReadMore ? this.__ResStr("moreLink", "Read More") : this.__ResStr("displayLink", "Display"),
                MenuText = ReadMore ? this.__ResStr("moreText", "Display") : this.__ResStr("displayText", "Display"),
                Tooltip = ReadMore ? this.__ResStr("moreTooltip", "Read the entire blog entry") : this.__ResStr("displayTooltip", "Display the blog entry"),
                Legend = ReadMore ? this.__ResStr("moreLegend", "Displays the entire blog entry") : this.__ResStr("displayLegend", "Displays an existing blog entry"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}