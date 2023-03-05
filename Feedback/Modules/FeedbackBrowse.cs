/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
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
using YetaWF.Modules.Feedback.Endpoints;

namespace YetaWF.Modules.Feedback.Modules;

public class FeedbackBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, FeedbackBrowseModule>, IInstallableModel { }

[ModuleGuid("{1dc47bb4-bcf9-4615-992a-29e2a4450f32}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class FeedbackBrowseModule : ModuleDefinition2 {

    public FeedbackBrowseModule() : base() {
        Title = this.__ResStr("modTitle", "Feedback");
        Name = this.__ResStr("modName", "Browse Feedback");
        Description = this.__ResStr("modSummary", "Used by the site owner to display and manage user feedback. It is accessible using Admin > Feedback (standard YetaWF site).");
        DefaultViewName = StandardViews.Browse;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new FeedbackBrowseModuleDataProvider(); }

    [Category("General"), Caption("Display URL"), Description("The URL to display a feedback message - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? DisplayUrl { get; set; }
    [Category("General"), Caption("Edit URL"), Description("The URL to edit a feedback message - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? EditUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("RemoveFeedback",
                    this.__ResStr("roleRemItemsC", "Remove Feedback"), this.__ResStr("roleRemItems", "The role has permission to remove feedback messages"),
                    this.__ResStr("userRemItemsC", "Remove Feedback"), this.__ResStr("userRemItems", "The user has permission to remove feedback messages")),
            };
        }
    }

    public ModuleAction? GetAction_Feedback(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Browse",
            LinkText = this.__ResStr("browseLink", "Feedback"),
            MenuText = this.__ResStr("browseText", "Feedback"),
            Tooltip = this.__ResStr("browseTooltip", "Display and manage user feedback"),
            Legend = this.__ResStr("browseLegend", "Displays and manages user feedback"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }
    public ModuleAction? GetAction_RemoveFeedback(int id) {
        if (!IsAuthorized("RemoveFeedback")) return null;
        return new ModuleAction() {
            Url = $"{Utility.UrlFor<FeedbackBrowseModuleEndpoints>(FeedbackBrowseModuleEndpoints.Remove)}/{id}",
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("removeLink", "Remove Feedback"),
            MenuText = this.__ResStr("removeMenu", "Remove Feedback"),
            Legend = this.__ResStr("removeLegend", "Remove the feedback message"),
            Tooltip = this.__ResStr("removeTT", "Removes the feedback message"),
            Category = ModuleAction.ActionCategoryEnum.Delete,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove this feedback message?"),
        };
    }

    public class BrowseItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands {
            get {
                List<ModuleAction> actions = new List<ModuleAction>();

                FeedbackDisplayModule dispMod = new FeedbackDisplayModule();
                actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Key), ModuleAction.ActionLocationEnum.GridLinks);

                actions.New(Module.GetAction_RemoveFeedback(Key), ModuleAction.ActionLocationEnum.GridLinks);
                return actions;
            }
        }
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
        [UIHint("String"), ReadOnly]
        public string Message { get; set; } = null!;

        private int Key { get; set; }
        private FeedbackBrowseModule Module { get; set; }

        public BrowseItem(FeedbackBrowseModule module, FeedbackData data) {
            Module = module;
            Key = data.Key;
            ObjectSupport.CopyData(data, this);
            if (data.Message.Length > 100)
                Message = data.Message.Substring(0, 100) + this.__ResStr("more", "...more");
        }
    }

    public class BrowseModel {
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; } = null!;
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<FeedbackBrowseModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                using (FeedbackDataProvider dataProvider = new FeedbackDataProvider()) {
                    DataProviderGetRecords<FeedbackData> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from s in browseItems.Data select new BrowseItem(this, s)).ToList<object>(),
                        Total = browseItems.Total
                    };
                }
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        BrowseModel model = new BrowseModel {
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}
