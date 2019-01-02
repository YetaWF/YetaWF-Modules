﻿/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class ModuleSelectionModuleExistingEditComponent : YetaWFComponent, IYetaWFComponent<Guid?> {

        public const string TemplateName = "ModuleSelectionModuleExisting";

        public override ComponentType GetComponentType() { return ComponentType.Edit; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public async Task<YHtmlString> RenderAsync(Guid? model) {

            model = model ?? Guid.Empty;
            string areaName = await ModuleSelectionModuleNewEditComponent.GetAreaNameFromGuidAsync(false, model);

            List<SelectionItem<string>> list = (
                from module in await DesignedModules.LoadDesignedModulesAsync()
                where module.AreaName == areaName
                orderby module.Name select
                    new SelectionItem<string> {
                        Text = module.Name,
                        Value = module.ModuleGuid.ToString(),
                        Tooltip = module.Description,
                    }).ToList<SelectionItem<string>>();
            list.Insert(0, new SelectionItem<string> { Text = this.__ResStr("none", "(none)"), Value = null });
            return await DropDownListComponent.RenderDropDownListAsync(this, model.ToString(), list, "yt_moduleselectionmoduleexisting");
        }
    }
}
