/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Modules;

public class RolesDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, RolesDisplayModule>, IInstallableModel { }

[ModuleGuid("{6584a819-f957-454d-8d58-aa57f2104e46}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Configuration")]
public class RolesDisplayModule : ModuleDefinition2 {

    public RolesDisplayModule() : base() {
        Title = this.__ResStr("modTitle", "Role");
        Name = this.__ResStr("modName", "Display Role");
        Description = this.__ResStr("modSummary", "Displays an existing role. This is used by the Roles Module to display a role.");
        DefaultViewName = StandardViews.Display;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new RolesDisplayModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Display(string url, string name) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { Name = name },
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "Display"),
            MenuText = this.__ResStr("displayText", "Display"),
            Tooltip = this.__ResStr("displayTooltip", "Display an existing role"),
            Legend = this.__ResStr("displayLegend", "Displays an existing role"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class DisplayModel {

        [Caption("Name"), Description("The role name")]
        [UIHint("String"), ReadOnly]
        public string Name { get; set; }

        [Caption("Description"), Description("The intended use of the role")]
        [UIHint("String"), ReadOnly]
        public string Description { get; set; }

        [Caption("Role Id"), Description("The internal id of the role")]
        [UIHint("IntValue"), ReadOnly]
        public int RoleId { get; set; }

        [Caption("Post Login URL"), Description("The URL where a user with this role is redirected after logging on")]
        [UIHint("Url"), ReadOnly]
        public string PostLoginUrl { get; set; }

        public void SetData(DataProvider.RoleDefinition data) {
            ObjectSupport.CopyData(data, this);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync(string name) {
        using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
            DataProvider.RoleDefinition data = await dataProvider.GetItemAsync(name);
            if (data == null)
                throw new Error(this.__ResStr("notFound", "Role \"{0}\" not found."), name);
            DisplayModel model = new DisplayModel();
            model.SetData(data);
            Title = this.__ResStr("modDisplayTitle", "{0} Role", name);
            return await RenderAsync(model);
        }
    }
}
