/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Panels.Models;
#if MVC6
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
#else
#endif

namespace YetaWF.Modules.Panels.Modules {

    public class DisplayStepsModuleDataProvider : ModuleDefinitionDataProvider<Guid, DisplayStepsModule>, IInstallableModel { }

    [ModuleGuid("{fff6a061-5b49-4501-ad70-3138ec1bf1b3}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class DisplayStepsModule : ModuleDefinition {

        public DisplayStepsModule() {
            Title = this.__ResStr("modTitle", "Steps");
            Name = this.__ResStr("modName", "Display Steps");
            Description = this.__ResStr("modSummary", "Displays steps to complete an action, usually a sequence of forms or pages. A sample page is available at Tests > Modules > Steps (standard YetaWF site).");
            StepInfo = new Models.StepInfo();
            UsePartialFormCss = false;
            ShowTitle = false;
            WantFocus = false;
            WantSearch = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new DisplayStepsModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

#if MVC6
        [ValidateNever, BindNever]
#endif
        [Copy] // not shown in property page
        public StepInfo StepInfo { get; set; }

        public override Task ModuleSavingAsync() {
            StepInfo.Saving(nameof(StepInfo), ModuleGuid); // update internal information
            return Task.CompletedTask;
        }
    }
}
