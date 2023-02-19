/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints.Support;
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

public class FeedbackConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, FeedbackConfigModule>, IInstallableModel { }

[ModuleGuid("{933431f8-8ac9-45b8-99e5-05abe2fd8f56}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class FeedbackConfigModule : ModuleDefinition2 {

    public FeedbackConfigModule() {
        Title = this.__ResStr("modTitle", "Feedback Settings");
        Name = this.__ResStr("modName", "Feedback Settings");
        Description = this.__ResStr("modSummary", "The configuration module for the feedback modules and defines global settings. It is accessible using Admin > Settings > Feedback Settings (standard YetaWF site).");
        ShowHelp = true;
        DefaultViewName = StandardViews.Config;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new FeedbackConfigModuleDataProvider(); }
    public override DataProviderImpl GetConfigDataProvider() { return new FeedbackConfigDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Config",
            LinkText = this.__ResStr("editLink", "Feedback Settings"),
            MenuText = this.__ResStr("editText", "Feedback Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit the feedback settings"),
            Legend = this.__ResStr("editLegend", "Edits the feedback settings"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class Model {

        [Caption("Use Captcha"), Description("The user has to pass a \"human\" test")]
        [UIHint("Boolean")]
        public bool Captcha { get; set; }

        [Caption("Name Required"), Description("Defines whether the user must enter his/her name to be able to send feedback")]
        [UIHint("Boolean")]
        public bool RequireName { get; set; }

        [Caption("Email Address Required"), Description("Defines whether the user must enter his/her email address to be able to send feedback")]
        [UIHint("Boolean")]
        public bool RequireEmail { get; set; }

        [Caption("Email Address"), Description("The email address where all feedback messages are sent")]
        [UIHint("Email"), StringLength(Globals.MaxEmail), EmailValidation, Trim]
        public string? Email { get; set; }

        [Caption("Copy Feedback Emails"), Description("Defines whether the site administrator receives a copy of all feedback messages")]
        [UIHint("Boolean")]
        public bool BccEmails { get; set; }

        public FeedbackConfigData GetData(FeedbackConfigData data) {
            ObjectSupport.CopyData(this, data);
            return data;
        }
        public void SetData(FeedbackConfigData data) {
            ObjectSupport.CopyData(data, this);
        }
        public Model() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (FeedbackConfigDataProvider dataProvider = new FeedbackConfigDataProvider()) {
            Model model = new Model { };
            FeedbackConfigData data = await dataProvider.GetItemAsync();
            if (data == null)
                throw new Error(this.__ResStr("notFound", "The feedback settings were not found."));
            model.SetData(data);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        using (FeedbackConfigDataProvider dataProvider = new FeedbackConfigDataProvider()) {
            FeedbackConfigData data = await dataProvider.GetItemAsync();// get the original item
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            data = model.GetData(data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display
            await dataProvider.UpdateConfigAsync(data);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Feedback settings saved"), NextPage: Manager.ReturnToUrl);
        }
    }
}
