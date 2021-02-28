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
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace Softelvdm.Modules.IVR.Controllers {

    public class BrowseExtensionsModuleController : ControllerImpl<Softelvdm.Modules.IVR.Modules.BrowseExtensionsModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands {
                get {
                    List<ModuleAction> actions = new List<ModuleAction>();

                    EditExtensionModule editMod = new EditExtensionModule();
                    actions.New(editMod.GetAction_Edit(Module.EditUrl, Extension), ModuleAction.ActionLocationEnum.GridLinks);
                    actions.New(Module.GetAction_Remove(Id), ModuleAction.ActionLocationEnum.GridLinks);
                    return actions;
                }
            }

            [Caption("Extension"), Description("Defines the extension (digits)")]
            [UIHint("String"), ReadOnly]
            [StringLength(ExtensionEntry.MaxExtension)]
            public string Extension { get; set; }

            [Caption("Description"), Description("Describes the extension")]
            [UIHint("String"), ReadOnly]
            [StringLength(ExtensionEntry.MaxDescription)]
            public string Description { get; set; }

            [Caption("Phone Numbers / SMS"), Description("Shows the phone numbers to call when this extension is entered and the text messaging selection")]
            [UIHint("Softelvdm_IVR_ListOfPhoneNumbers"), AdditionalMetadata("UseSkinFormatting", false), AdditionalMetadata("Header", false), AdditionalMetadata("Pager", false), ReadOnly]
            public SerializableList<ExtensionPhoneNumber> PhoneNumbers { get; set; }

            [Caption("Users"), Description("Shows the users that can access voice mails for this extension")]
            [UIHint("YetaWF_Identity_ListOfUserNames"), AdditionalMetadata("UseSkinFormatting", false), AdditionalMetadata("Header", false), AdditionalMetadata("Pager", false), ReadOnly]
            public SerializableList<User> Users { get; set; }

            [Caption("Created"), Description("The date/time the extension was added")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }
            [Caption("Updated"), Description("The date/time the extension was last updated")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime? Updated { get; set; }

            public int Id { get; set; }
            private BrowseExtensionsModule Module { get; set; }

            public BrowseItem(BrowseExtensionsModule module, ExtensionEntry data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
            }
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(BrowseExtensions_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    using (ExtensionEntryDataProvider dataProvider = new ExtensionEntryDataProvider()) {
                        DataProviderGetRecords<ExtensionEntry> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem(Module, s)).ToList<object>(),
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
        public ActionResult BrowseExtensions() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel()
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> BrowseExtensions_GridData(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync(GetGridModel(), gridPVData);
        }

        [AllowPost]
        [Permission("RemoveItems")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(int id) {
            using (ExtensionEntryDataProvider dataProvider = new ExtensionEntryDataProvider()) {
                if (!await dataProvider.RemoveItemByIdentityAsync(id))
                    throw new Error(this.__ResStr("cantRemove", "Couldn't remove extension with id {0}", id));
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }
    }
}
