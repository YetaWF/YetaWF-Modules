/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Lightbox#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Lightbox.Modules {

    public class SkinLightboxModuleDataProvider : ModuleDefinitionDataProvider<Guid, SkinLightboxModule>, IInstallableModel { }

    [ModuleGuid("{39244dbc-0536-4c85-88d1-b84b504510ac}"), PublishedModuleGuid]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SkinLightboxModule : ModuleDefinition {

        public SkinLightboxModule() {
            Title = this.__ResStr("modTitle", "Skin Lightbox");
            Name = this.__ResStr("modName", "Lightbox (Skin)");
            Description = this.__ResStr("modSummary", "Implements Lightbox on any page where this module is included. It can be included explicitly by a skin or by a module's, page's or site's Skin Modules (see References tab in Page, Module or Site Settings). See Skin Modules for more information.");
            WantFocus = false;
            ShowTitle = false;
            WantSearch = false;
            Invokable = true;
            InvokeInPopup = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SkinLightboxModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    }
}