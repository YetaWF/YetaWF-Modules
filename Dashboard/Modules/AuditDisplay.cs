/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Extensions;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Dashboard.DataProvider;

namespace YetaWF.Modules.Dashboard.Modules;

public class AuditDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, AuditDisplayModule>, IInstallableModel { }

[ModuleGuid("{d739ad4d-2208-485c-9b98-81bf2c960c0d}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class AuditDisplayModule : ModuleDefinition {

    public AuditDisplayModule() {
        Title = this.__ResStr("modTitle", "Audit Record");
        Name = this.__ResStr("modName", "Audit Record");
        Description = this.__ResStr("modSummary", "Displays an existing audit record. Used by the Audit Information Module.");
        DefaultViewName = StandardViews.Display;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new AuditDisplayModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Display(string? url, int id) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { Id = id },
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "Display"),
            MenuText = this.__ResStr("displayText", "Display"),
            Tooltip = this.__ResStr("displayTooltip", "Display the audit record"),
            Legend = this.__ResStr("displayLegend", "Displays an existing audit record"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    public class DisplayModel {

        [Caption("Id"), Description("The internal id")]
        [UIHint("IntValue"), ReadOnly]
        public int Id { get; set; }
        [Caption("Created"), Description("The date/time this record was created")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Created { get; set; }

        [Caption("Identifier/String"), Description("The identifying string of the record - Identifier String and Type both identify the source of this record")]
        [UIHint("String"), ReadOnly]
        public string IdentifyString { get; set; } = null!;
        [Caption("Identifier/Type"), Description("The type of the record - Identifier String and Type both identify the source of this record")]
        [UIHint("Guid"), ReadOnly]
        public Guid IdentifyGuid { get; set; }

        [Caption("Action"), Description("The action that created this record")]
        [UIHint("String"), ReadOnly]
        public string Action { get; set; } = null!;
        [Caption("Description"), Description("The description for this record")]
        [UIHint("String"), ReadOnly]
        public string? Description { get; set; }
        [Caption("Changes"), Description("The properties that were changed")]
        [UIHint("String"), ReadOnly]
        public string? Changes { get; set; }

        [Caption("Site"), Description("The site that was changed")]
        [UIHint("SiteId"), ReadOnly]
        public int SiteIdentity { get; set; }
        [Caption("User"), Description("The user that made the change")]
        [UIHint("YetaWF_Identity_UserId"), ReadOnly]
        public int UserId { get; set; }

        [Caption("Restart Pending"), Description("Defines whether this action requires a restart to take effect")]
        [UIHint("Boolean"), ReadOnly]
        public bool RequiresRestart { get; set; }
        [Caption("Expensive Action"), Description("Defines whether this action is an expensive action in a multi-instance site")]
        [UIHint("Boolean"), ReadOnly]
        public bool ExpensiveMultiInstance { get; set; }

        public void SetData(AuditInfo data) {
            ObjectSupport.CopyData(data, this);
            Description = Description?.Truncate(100);
            Changes = Changes?.Replace(",", ", ");
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        if (!int.TryParse(Manager.RequestQueryString["Id"], out int id)) id = 0;
        using (AuditInfoDataProvider dataProvider = new AuditInfoDataProvider()) {
            AuditInfo? data = await dataProvider.GetItemAsync(id);
            if (data == null)
                throw new Error(this.__ResStr("notFound", "Audit Info \"{0}\" not found"), id);
            DisplayModel model = new DisplayModel();
            model.SetData(data);
            return await RenderAsync(model);
        }
    }
}
