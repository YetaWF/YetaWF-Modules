/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Menus.Modules {

    public class MainMenuModuleDataProvider : ModuleDefinitionDataProvider<Guid, MainMenuModule>, IInstallableModel { }

    [ModuleGuid("{59909BB1-75F4-419f-B961-8569BB282131}"), PublishedModuleGuid]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)] // this is unique because we need one site-wide menu for admin commands, etc. (additional regular menus can be defined using the MenuModule)
    public class MainMenuModule : MenuModule {

        public MainMenuModule() {
            Name = this.__ResStr("modName", "Main Menu");
            Title = this.__ResStr("modTitle", "Main Menu");
            Description = this.__ResStr("modSummary", "Implements a menu. This is can be added to a site's skin or can be added to pages as needed.");
            CssClass = "yPageMenu";
        }

        public override IModuleDefinitionIO GetDataProvider() { return new MainMenuModuleDataProvider(); }
    }
}