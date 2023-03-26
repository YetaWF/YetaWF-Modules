/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
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
using YetaWF.Modules.Feedback.Support;

namespace YetaWF.Modules.Feedback.Modules;

public class FeedbackAddModuleDataProvider : ModuleDefinitionDataProvider<Guid, FeedbackAddModule>, IInstallableModel { }

[ModuleGuid("{30eeace2-f61d-45b7-a430-12c873f78bae}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class FeedbackAddModule : ModuleDefinition {

    public FeedbackAddModule() {
        Title = this.__ResStr("modTitle", "Send Feedback");
        Name = this.__ResStr("modName", "Send Feedback");
        Description = this.__ResStr("modSummary", "Allows entry of a message to the site owner or administrator by a site visitor.");
        Print = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new FeedbackAddModuleDataProvider(); }

    [Category("General"), Caption("Default Subject"), Description("The optional default subject of the message when new feedback is entered")]
    [UIHint("Text80"), StringLength(FeedbackData.MaxSubject), Trim]
    public string? DefaultSubject { get; set; }

    [Category("General"), Caption("Default Message"), Description("The optional default message when new feedback is entered")]
    [UIHint("TextAreaSourceOnly"), StringLength(FeedbackData.MaxMessage), Trim]
    public string? DefaultMessage { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Add(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Add",
            LinkText = this.__ResStr("addLink", "Enter Feedback"),
            MenuText = this.__ResStr("addText", "Enter Feedback"),
            Tooltip = this.__ResStr("addTooltip", "Enter feedback for us"),
            Legend = this.__ResStr("addLegend", "Allows you to enter feedback for us"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,

        };
    }

    [Trim]
    public class AddModel {

        public int Key { get; set; }

        [Caption("Subject"), Description("Please enter the subject for your message")]
        [UIHint("Text80"), StringLength(FeedbackData.MaxSubject), Required, Trim]
        public string? Subject { get; set; } = null!;

        [Caption("Your Name"), Description("Please enter your name - Your name will not be publicly visible")]
        [UIHint("Text40"), SuppressIf(nameof(RequireName), false), StringLength(FeedbackData.MaxName), Required, Trim]
        public string? Name { get; set; } = null!;

        [Caption("Your Email Address"), Description("Please enter your email address - Your email address will not be publicly visible")]
        [UIHint("Email"), SuppressIf(nameof(RequireEmail), false), StringLength(Globals.MaxEmail), EmailValidation, Required, Trim]
        public string? Email { get; set; } = null!;

        [Caption("Message"), Description("Please enter the message")]
        [UIHint("TextAreaSourceOnly"), StringLength(FeedbackData.MaxMessage), Required]
        public string? Message { get; set; } = null!;

        [Caption("Captcha"), Description("Please verify that you're a human and not a spam bot")]
        [UIHint("RecaptchaV2"), RecaptchaV2("Please verify that you're a human and not a spam bot"), SuppressIf("ShowCaptcha", false)]
        public RecaptchaV2Data Captcha { get; set; }

        [UIHint("Hidden")]
        public bool RequireName { get; set; }
        [UIHint("Hidden")]
        public bool RequireEmail { get; set; }
        [UIHint("Hidden")]
        public bool ShowCaptcha { get; set; }

        public AddModel() {
            Captcha = new RecaptchaV2Data() { };
        }

        public FeedbackData GetData() {
            FeedbackData data = new FeedbackData();
            ObjectSupport.CopyData(this, data);
            return data;
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        AddModel model = new AddModel {
            Captcha = new RecaptchaV2Data(),
            Subject = DefaultSubject ?? string.Empty,
            Message = DefaultMessage ?? string.Empty,
        };
        FeedbackConfigData config = await FeedbackConfigDataProvider.GetConfigAsync();
        model.RequireName = config.RequireName;
        model.RequireEmail = config.RequireEmail;
        model.ShowCaptcha = config.Captcha;
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(AddModel model) {

        FeedbackConfigData config = await FeedbackConfigDataProvider.GetConfigAsync();
        model.RequireName = config.RequireName;
        model.RequireEmail = config.RequireEmail;
        model.ShowCaptcha = config.Captcha;

        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        using (FeedbackDataProvider dataProvider = new FeedbackDataProvider()) {
            if (!await dataProvider.AddItemAsync(model.GetData()))
                throw new InternalError("Feedback couldn't be sent");

            Emails emails = new Emails();
            await emails.SendFeedbackAsync(config.Email, model.Email, model.Subject!, model.Message!, model.Name!, config.BccEmails ? Manager.CurrentSite.AdminEmail : null);

            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Your message has been sent!"));
        }
    }
}
