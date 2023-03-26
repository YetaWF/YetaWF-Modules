/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

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
using YetaWF.Modules.Feedback.DataProvider;

namespace YetaWF.Modules.Feedback.Modules;

public class FeedbackDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, FeedbackDisplayModule>, IInstallableModel { }

[ModuleGuid("{a29a50e9-9457-46f4-9bce-77a967b1671e}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class FeedbackDisplayModule : ModuleDefinition {

    public FeedbackDisplayModule() : base() {
        Title = this.__ResStr("modTitle", "Feedback Message");
        Name = this.__ResStr("modName", "Feedback Message");
        Description = this.__ResStr("modSummary", "Displays a feedback message entered by a site visitor. This is used by the Browse Feedback Module.");
        DefaultViewName = StandardViews.Display;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new FeedbackDisplayModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Display(string? url, int key) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { Key = key },
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "Display"),
            MenuText = this.__ResStr("displayText", "Display"),
            Tooltip = this.__ResStr("displayTooltip", "Display the feedback message"),
            Legend = this.__ResStr("displayLegend", "Displays a feedback message"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,

        };
    }

    public class DisplayModel {

        [Caption("Created"), Description("The date the message was created")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Created { get; set; }

        [Caption("Name"), Description("The user's name")]
        [UIHint("String"), ReadOnly]
        public string Name { get; set; } = null!;

        [Caption("Email Address"), Description("The user's email address")]
        [UIHint("String"), ReadOnly]
        public string Email { get; set; } = null!;

        [Caption("Subject"), Description("The subject of the message")]
        [UIHint("String"), ReadOnly]
        public string Subject { get; set; } = null!;

        [Caption("IP Address"), Description("The IP address from which the feedback message was sent")]
        [UIHint("IPAddress"), ReadOnly]
        public string IPAddress { get; set; } = null!;

        [Caption("Message"), Description("The feedback message")]
        [UIHint("TextArea"), ReadOnly]
        public string Message { get; set; } = null!;

        public void SetData(FeedbackData data) {
            ObjectSupport.CopyData(data, this);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync(int key) {
        using (FeedbackDataProvider dataProvider = new FeedbackDataProvider()) {
            FeedbackData? data = await dataProvider.GetItemAsync(key);
            if (data == null)
                throw new Error(this.__ResStr("notFound", "Feedback \"{0}\" not found."), key);
            DisplayModel model = new DisplayModel();
            model.SetData(data);
            return await RenderAsync(model);
        }
    }
}
