/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Pages.Modules {

    public class SkinScrollToTopModuleDataProvider : ModuleDefinitionDataProvider<Guid, SkinScrollToTopModule>, IInstallableModel { }

    [ModuleGuid("{2a4e6f13-24a0-45c1-8a42-f1072e6ac7de}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SkinScrollToTopModule : ModuleDefinition {

        public SkinScrollToTopModule() {
            Title = this.__ResStr("modTitle", "Skin ScrollToTop");
            Name = this.__ResStr("modName", "ScrollToTop (Skin)");
            Description = this.__ResStr("modSummary", "Skin module implementing a scroll to top button to return to the top of the current page");
            WantFocus = false;
            ShowTitle = false;
            WantSearch = false;
            Invokable = true;
            InvokeInPopup = false;
            InvokeInAjax = true;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SkinScrollToTopModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
    }
}
