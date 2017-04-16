/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.BootstrapCarousel.Modules {

    public class CarouselEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, CarouselEditModule>, IInstallableModel { }

    [ModuleGuid("{e664a6fb-bb2c-44ef-891f-f9f4a0e1125d}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class CarouselEditModule : ModuleDefinition {

        public CarouselEditModule() {
            Title = this.__ResStr("modTitle", "Bootstrap Carousel Settings");
            Name = this.__ResStr("modName", "Edit Bootstrap Carousel");
            Description = this.__ResStr("modSummary", "Edits an existing Bootstrap Carousel");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new CarouselEditModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url, Guid carouselDisplayMod) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { Carousel = carouselDisplayMod },
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Carousel Settings"),
                MenuText = this.__ResStr("editText", "Carousel Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit Bootstrap Carousel settings"),
                Legend = this.__ResStr("editLegend", "Edits an existing Bootstrap Carousel"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | (Manager.EditMode ? ModuleAction.ActionLocationEnum.ModuleMenu : 0),
                SaveReturnUrl = true,
            };
        }
    }
}