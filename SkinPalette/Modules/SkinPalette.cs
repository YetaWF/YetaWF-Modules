/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SkinPalette#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.SkinPalette.Modules {

    public class SkinPaletteModuleDataProvider : ModuleDefinitionDataProvider<Guid, SkinPaletteModule>, IInstallableModel { }

    [ModuleGuid("{915a366d-facb-4d02-b8f8-bb1acef73c4c}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    //[ModuleCategory("...")]
    public class SkinPaletteModule : ModuleDefinition {

        public SkinPaletteModule() {
            Title = this.__ResStr("modTitle", "Skin Palette");
            Name = this.__ResStr("modName", "Skin Palette (Skin)");
            Description = this.__ResStr("modSummary", "Edits the current skin");
            UsePartialFormCss = false;
            ShowTitleActions = false;
            ShowTitle = false;
            WantFocus = false;
            WantSearch = false;
            Print = false;
            Invokable = true;
            CssClass = "yCondense";
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SkinPaletteModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
