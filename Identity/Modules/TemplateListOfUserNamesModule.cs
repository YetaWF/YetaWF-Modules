/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Modules;

public class TemplateListOfUserNamesModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateListOfUserNamesModule>, IInstallableModel { }

[ModuleGuid("{190287E8-EC79-404C-9FCA-6D43607825BC}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Tools")]
public class TemplateListOfUserNamesModule : ModuleDefinition2 {

    public TemplateListOfUserNamesModule() {
        Title = this.__ResStr("modTitle", "ListOfUserNames Test Component");
        Name = this.__ResStr("modName", "Component Test - ListOfUserNames");
        Description = this.__ResStr("modSummary", "Test module for the ListOfUserNames component (edit and display). A test page for this module can be found at Tests > Templates > ListOfUserNames (standard YetaWF site).");
        DefaultViewName = StandardViews.EditApply;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new TemplateListOfUserNamesModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "ListOfUserNames"),
            MenuText = this.__ResStr("displayText", "ListOfUserNames"),
            Tooltip = this.__ResStr("displayTooltip", "Display the ListOfUserNames test template"),
            Legend = this.__ResStr("displayLegend", "Displays the ListOfUserNames test template"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class Model {

        [Caption("User Names (Required)"), Description("List of User Names (Required)")]
        [UIHint("YetaWF_Identity_ListOfUserNames"), ListNoDuplicates, Required, Trim]
        public SerializableList<User> Prop1Req { get; set; }

        [Caption("User Names"), Description("List of User Names")]
        [UIHint("YetaWF_Identity_ListOfUserNames"), ListNoDuplicates, Trim]
        public SerializableList<User> Prop1 { get; set; }

        [Caption("User Names (Read/Only)"), Description("List of User Names (read/only)")]
        [UIHint("YetaWF_Identity_ListOfUserNames"), ReadOnly]
        public SerializableList<User> Prop1RO { get; set; }

        public Model() {
            Prop1Req = new SerializableList<User>();
            Prop1 = new SerializableList<User>();
            Prop1RO = new SerializableList<User>();
        }
        public async Task Init() {
            using (UserDefinitionDataProvider userDP = new DataProvider.UserDefinitionDataProvider()) {
                // get 5 sample users
                DataProviderGetRecords<UserDefinition> recs = await userDP.GetItemsAsync(0, 5, null, null);
                SerializableList<User> users = new SerializableList<User>((from u in recs.Data select new User { UserId = u.UserId }).ToList());
                Prop1Req = users;
                Prop1 = users;
                Prop1RO = users;
            }
        }
        public async Task Update() {
            using (UserDefinitionDataProvider userDP = new DataProvider.UserDefinitionDataProvider()) {
                // get 5 sample users
                DataProviderGetRecords<UserDefinition> recs = await userDP.GetItemsAsync(0, 5, null, null);
                SerializableList<User> users = new SerializableList<User>((from u in recs.Data select new User { UserId = u.UserId }).ToList());
                Prop1RO = users;
            }
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        Model model = new Model { };
        await model.Init();
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        await model.Update();
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);
        return await FormProcessedAsync(model, this.__ResStr("ok", "OK"));
    }
}
