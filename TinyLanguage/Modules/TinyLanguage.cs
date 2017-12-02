/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TinyLanguage#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.TinyLanguage.Modules {

    public class TinyLanguageModuleDataProvider : ModuleDefinitionDataProvider<Guid, TinyLanguageModule>, IInstallableModel { }

    [ModuleGuid("{bb3653e4-6d75-45e2-b998-714b225b5ffa}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TinyLanguageModule : ModuleDefinition {

        public TinyLanguageModule() : base() {
            Title = this.__ResStr("modTitle", "Tiny Language Selection");
            Name = this.__ResStr("modName", "Tiny Language Selection");
            Description = this.__ResStr("modSummary", "Allow user selection of the site's default language");
            ShowTitle = false;
            WantFocus = false;
            WantSearch = false;
            Print = false;
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TinyLanguageModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
    }
}