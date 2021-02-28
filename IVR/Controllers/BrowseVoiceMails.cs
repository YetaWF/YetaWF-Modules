/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Softelvdm.Modules.IVR.DataProvider;
using Softelvdm.Modules.IVR.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace Softelvdm.Modules.IVR.Controllers {

    public class BrowseVoiceMailsModuleController : ControllerImpl<Softelvdm.Modules.IVR.Modules.BrowseVoiceMailsModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands { get; set; }

            public async Task<List<ModuleAction>> __GetCommandsAsync() {
                List<ModuleAction> actions = new List<ModuleAction>();
                actions.New(await DisplayModule.GetAction_DisplayAsync(Module.DisplayUrl, Id), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(Module.GetAction_Remove(Id), ModuleAction.ActionLocationEnum.GridLinks);
                return actions;
            }

            [Caption("Id"), Description("The internal id")]
            [UIHint("IntValue"), ReadOnly]
            public int Id { get; set; }

            [Caption("Created"), Description("The date/time the voice mail message was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }

            [Caption("Heard"), Description("Defines whether the voice mail was already listened to")]
            [UIHint("Boolean"), ReadOnly]
            public bool Heard { get; set; }

            [Caption("Call Sid"), Description("The id used by Twilio to identify the call")]
            [UIHint("String"), ReadOnly]
            public string CallSid { get; set; }

            [Caption("Phone Number"), Description("The phone number for which the voice mail message is saved")]
            [UIHint("Softelvdm_IVR_PhoneNumber"), ReadOnly]
            public string To { get; set; }
            [Caption("Extension"), Description("The extension for which the voice mail message is saved")]
            [UIHint("String"), ReadOnly]
            public string Extension { get; set; }

            public string RecordingUrl { get; set; }

            [Caption("From"), Description("The caller's phone number")]
            [UIHint("Softelvdm_IVR_PhoneNumber"), ReadOnly]
            [ExcludeDemoMode]
            public string Caller { get; set; }
            [Caption("From City"), Description("The caller's city (if available)")]
            [UIHint("String"), ReadOnly]
            public string CallerCity { get; set; }
            [Caption("From State"), Description("The caller's state (if available)")]
            [UIHint("String"), ReadOnly]
            public string CallerState { get; set; }
            [Caption("From Zip Code"), Description("The caller's ZIP code (if available)")]
            [UIHint("String"), ReadOnly]
            public string CallerZip { get; set; }
            [Caption("From Country"), Description("The caller's country (if available)")]
            [UIHint("String"), ReadOnly]
            public string CallerCountry { get; set; }

            [Caption("Duration"), Description("The duration of the voice mail message (in seconds)")]
            [UIHint("IntValue"), ReadOnly]
            public int Duration { get; set; }

            private BrowseVoiceMailsModule Module { get; set; }
            private DisplayVoiceMailModule DisplayModule { get; set; }

            public BrowseItem(BrowseVoiceMailsModule module, DisplayVoiceMailModule displayModule, VoiceMailData data) {
                Module = module;
                DisplayModule = displayModule;
                ObjectSupport.CopyData(data, this);
            }
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
                ModuleGuid = Module.ModuleGuid,
                //SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(BrowseVoiceMails_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    List<string> extensions = new List<string>();
                    if (!Manager.HasSuperUserRole) {
                        using (ExtensionEntryDataProvider extDP = new ExtensionEntryDataProvider()) {
                            extensions = await extDP.GetExtensionsForUserAsync(Manager.UserId);
                        }
                        if (extensions.Count == 0)
                            throw new Error(this.__ResStr("noInbox", "No extension defined for the current user"));
                    }
                    DisplayVoiceMailModule dispMod = (DisplayVoiceMailModule)await ModuleDefinition.LoadAsync(ModuleDefinition.GetPermanentGuid(typeof(DisplayVoiceMailModule)));
                    using (VoiceMailDataProvider dataProvider = new VoiceMailDataProvider()) {
                        List<DataProviderFilterInfo> extFilters = null;
                        foreach (string extension in extensions) {
                            extFilters = DataProviderFilterInfo.Join(extFilters, new DataProviderFilterInfo { Field = nameof(ExtensionEntry.Extension), Operator = "==", Value = extension }, SimpleLogic: "||");
                        }
                        if (extFilters != null)
                            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Filters = extFilters, Logic = "||" });
                        DataProviderGetRecords<VoiceMailData> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem(Module, dispMod, s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        public class BrowseModel {
            [Caption(""), Description("")] // empty entries required so property is shown in property list (but with a suppressed label)
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }
        }

        [AllowGet]
        public ActionResult BrowseVoiceMails() {
            Manager.NeedUser();
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel()
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> BrowseVoiceMails_GridData(GridPartialViewData gridPVData) {
            Manager.NeedUser();
            return await GridPartialViewAsync(GetGridModel(), gridPVData);
        }

        [AllowPost]
        [Permission("RemoveItems")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(int id) {
            using (VoiceMailDataProvider dataProvider = new VoiceMailDataProvider()) {
                if (!await dataProvider.RemoveItemByIdentityAsync(id))
                    throw new Error(this.__ResStr("cantRemove", "Couldn't remove voice mail message with id {0}", id));
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }
    }
}
