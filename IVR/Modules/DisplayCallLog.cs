/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Softelvdm.Modules.IVR.DataProvider;
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

namespace Softelvdm.Modules.IVR.Modules;

public class DisplayCallLogModuleDataProvider : ModuleDefinitionDataProvider<Guid, DisplayCallLogModule>, IInstallableModel { }

[ModuleGuid("{92a027ed-704a-4dd3-bbc7-ff4185539e82}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class DisplayCallLogModule : ModuleDefinition2 {

    public DisplayCallLogModule() {
        Title = this.__ResStr("modTitle", "Call Log Entry");
        Name = this.__ResStr("modName", "Call Log Entry");
        Description = this.__ResStr("modSummary", "Displays an existing call log entry.");
        DefaultViewName = StandardViews.Display;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new DisplayCallLogModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Display(string? url, int id) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { Id = id },
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "Display"),
            MenuText = this.__ResStr("displayText", "Display"),
            Tooltip = this.__ResStr("displayTooltip", "Display the call log entry"),
            Legend = this.__ResStr("displayLegend", "Displays an existing call log entry"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    public class DisplayModel {

        [Caption("Created"), Description("The date/time the call was received")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Created { get; set; }

        [Caption("From"), Description("The caller's phone number")]
        [UIHint("PhoneNumber"), ReadOnly]
        [ExcludeDemoMode]
        public string Caller { get; set; } = null!;
        [Caption("From City"), Description("The caller's city (if available)")]
        [UIHint("String"), ReadOnly]
        public string? CallerCity { get; set; }
        [Caption("From State"), Description("The caller's state (if available)")]
        [UIHint("String"), ReadOnly]
        public string? CallerState { get; set; }
        [Caption("From Zip Code"), Description("The caller's ZIP code (if available)")]
        [UIHint("String"), ReadOnly]
        public string? CallerZip { get; set; }
        [Caption("From Country"), Description("The caller's country (if available)")]
        [UIHint("String"), ReadOnly]
        public string? CallerCountry { get; set; }

        [Caption("Phone Number"), Description("The phone number called")]
        [UIHint("PhoneNumber"), ReadOnly]
        public string? To { get; set; }

        [Caption("Id"), Description("The internal id")]
        [UIHint("IntValue"), ReadOnly]
        public int Id { get; set; }

        public void SetData(CallLogEntry data) {
            ObjectSupport.CopyData(data, this);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        if (!int.TryParse(Manager.RequestQueryString["Id"], out int id)) throw new InternalError($"Argument {nameof(id)} missing");
        using (CallLogDataProvider dataProvider = new CallLogDataProvider()) {
            CallLogEntry? data = await dataProvider.GetItemByIdentityAsync(id);
            if (data == null)
                throw new Error(this.__ResStr("notFound", "Call Log Entry with id {0} not found"), id);
            DisplayModel model = new DisplayModel();
            model.SetData(data);
            return await RenderAsync(model);
        }
    }
}
