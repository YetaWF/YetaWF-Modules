/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Menus#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Menus.Modules {

    public class MainMenuModuleDataProvider : ModuleDefinitionDataProvider<Guid, MainMenuModule>, IInstallableModel { }

    [ModuleGuid("{59909BB1-75F4-419f-B961-8569BB282131}")] // Published Guid
    [UniqueModule(UniqueModuleStyle.UniqueOnly)] // this is unique because we need one site-wide menu for admin commands, etc. (additional regular menus can be defined using the MenuModule)
    public class MainMenuModule : MenuModule {

        public MainMenuModule() {
            Name = this.__ResStr("modName", "Main Menu");
            Title = this.__ResStr("modTitle", "Main Menu");
            Description = this.__ResStr("modSummary", "Main page menu");
            CssClass = "urlpreview-follow yPageMenu";// make sure all links are followed by default in the main menu for Url Previews
        }

        public override IModuleDefinitionIO GetDataProvider() { return new MainMenuModuleDataProvider(); }
    }
}