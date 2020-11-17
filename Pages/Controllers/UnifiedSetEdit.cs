/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.Pages.DataProvider;
using YetaWF.Core.Skins;
using YetaWF.Modules.Pages.Components;
using System.Linq;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Pages.Controllers {

    public class UnifiedSetEditModuleController : ControllerImpl<YetaWF.Modules.Pages.Modules.UnifiedSetEditModule> {

        public UnifiedSetEditModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Name"), Description("The name of this Unified Page Set")]
            [UIHint("Text80"), StringLength(UnifiedSetData.MaxName), Required, Trim]
            public string Name { get; set; }

            [Caption("Description"), Description("The description for this Unified Page Set")]
            [UIHint("Text80"), StringLength(UnifiedSetData.MaxDescription), Required, Trim]
            public string Description { get; set; }

            [Caption("Created"), Description("The date/time this set was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }

            [Caption("Updated"), Description("The date/time this set was last updated")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Updated { get; set; }

            [Caption("Master Page"), Description("Defines the master page for the Unified Page Set that defines the skin, referenced modules, authorization and all other page attributes")]
            [UIHint("PageSelection"), SelectionRequired]
            public Guid MasterPageGuid { get; set; }

            [Caption("Disabled"), Description("Defines whether the Unified Page Set is disabled")]
            [UIHint("Boolean")]
            public bool Disabled { get; set; }

            [Caption("Mode"), Description("Defines how page content is combined")]
            [UIHint("Enum")]
            public PageDefinition.UnifiedModeEnum UnifiedMode { get; set; }

            [Caption("Page Skin"), Description("All pages using this skin are combined into this Unified Page Set and use the Master Page for all its attributes - Pages that are explicitly included in another Unified Page Set are not part of this set even if the skin matches")]
            [UIHint("PageSkin"), Required]
            [ProcessIf(nameof(UnifiedMode), PageDefinition.UnifiedModeEnum.SkinDynamicContent)]
            public SkinDefinition PageSkin { get; set; }

            [Caption("Popups"), Description("Defines whether popups are part of this Unified Page Set (used with SkinDynamicContent and DynamicContent only)")]
            [UIHint("Boolean")]
            [ProcessIf(nameof(UnifiedMode), PageDefinition.UnifiedModeEnum.SkinDynamicContent)]
            [ProcessIf(nameof(UnifiedMode), PageDefinition.UnifiedModeEnum.DynamicContent)]
            public bool Popups { get; set; }

            [Caption("Animation Duration"), Description("Defines the duration of the animation to scroll to the content (used only if Mode is set to Show All Content)")]
            [UIHint("IntValue6"), Range(0, 999999), Required]
            [ProcessIf(nameof(UnifiedMode), PageDefinition.UnifiedModeEnum.ShowDivs)]
            public int UnifiedAnimation { get; set; }

            [TextAbove("Add pages to the Unified Page Set by selecting them in the dropdown list below. " +
                "When selecting a page in the \"All Pages\" grid, the dropdown list is updated to match, so the page can be added. " +
                "Pages in the Unified Page Set can be reordered by dragging and dropping them within the list of pages.")]
            [Caption("Pages"), Description("Defines all pages that are part of this Unified Page Set - All modules (in designated panes) from all pages in the set are combined into one page - Whenever a user accesses any page by Url in the Unified Page Set, the same combined page is shown, activating or scrolling to the requested content - When moving between different pages within the Unified Page Set, no server access is required")]
            [UIHint("YetaWF_Pages_ListOfLocalPages")]
            [ProcessIf(nameof(UnifiedMode), PageDefinition.UnifiedModeEnum.DynamicContent)]
            [ProcessIf(nameof(UnifiedMode), PageDefinition.UnifiedModeEnum.HideDivs)]
            [ProcessIf(nameof(UnifiedMode), PageDefinition.UnifiedModeEnum.ShowDivs)]
            public List<string> PageList { get; set; }
            public string PageList_AjaxUrl { get { return Utility.UrlFor(typeof(UnifiedSetEditModuleController), nameof(AddPage)); } }

            [UIHint("Hidden")]
            public Guid UnifiedSetGuid { get; set; }

            public UnifiedSetData GetData(UnifiedSetData unifiedSet) {
                ObjectSupport.CopyData(this, unifiedSet);
                return unifiedSet;
            }
            public void SetData(UnifiedSetData unifiedSet) {
                ObjectSupport.CopyData(unifiedSet, this);
            }
            public EditModel() {
                PageList = new List<string>();
                PageSkin = new Core.Skins.SkinDefinition();
            }
        }

        [AllowGet]
        public async Task<ActionResult> UnifiedSetEdit(Guid unifiedSetGuid) {
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Editing Unified Page Sets is not possible when distributed caching is enabled");
            using (UnifiedSetDataProvider unifiedSetDP = new UnifiedSetDataProvider()) {
                EditModel model = new EditModel { };
                UnifiedSetData data = await unifiedSetDP.GetItemAsync(unifiedSetGuid);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Unified page set with id \"{0}\" not found"), unifiedSetGuid);
                model.SetData(data);
                return View(model);
            }
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> UnifiedSetEdit_Partial(EditModel model) {
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Editing Unified Page Sets is not possible when distributed caching is enabled");

            using (UnifiedSetDataProvider unifiedSetDP = new UnifiedSetDataProvider()) {
                UnifiedSetData unifiedSet = await unifiedSetDP.GetItemAsync(model.UnifiedSetGuid);// get the original item
                if (unifiedSet == null)
                    throw new Error(this.__ResStr("alreadyDeleted", "The unified page set with id {0} has been removed and can no longer be updated", model.UnifiedSetGuid));

                ObjectSupport.CopyData(unifiedSet, model, ReadOnly: true); // update read only properties in model in case there is an error
                if (!ModelState.IsValid)
                    return PartialView(model);

                unifiedSet = model.GetData(unifiedSet); // merge new data into original
                model.SetData(unifiedSet); // and all the data back into model for final display

                switch (await unifiedSetDP.UpdateItemAsync(unifiedSet)) {
                    default:
                    case UpdateStatusEnum.RecordDeleted:
                        throw new Error(this.__ResStr("alreadyDeleted", "The unified page set with id {0} has been removed and can no longer be updated", model.UnifiedSetGuid));
                    case UpdateStatusEnum.NewKeyExists:
                        ModelState.AddModelError(nameof(model.Name), this.__ResStr("alreadyExists", "An unified page set named \"{0}\" already exists", model.Name));
                        return PartialView(model);
                    case UpdateStatusEnum.OK:
                        break;
                }
                return FormProcessed(model, this.__ResStr("okSaved", "Unified page set saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AddPage(string data, string fieldPrefix, string newUrl) {
            // Validation
            UrlValidationAttribute attr = new UrlValidationAttribute(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local);
            if (!attr.IsValid(newUrl))
                throw new Error(attr.ErrorMessage);
            // check duplicates
            List<ListOfLocalPagesEditComponent.Entry> list = Utility.JsonDeserialize<List<ListOfLocalPagesEditComponent.Entry>>(data);
            if ((from l in list where l.Url.ToLower() == newUrl.ToLower() select l).FirstOrDefault() != null)
                throw new Error(this.__ResStr("dupUrl", "Url {0} has already been added", newUrl));
            // add new grid record
            ListOfLocalPagesEditComponent.Entry entry = new ListOfLocalPagesEditComponent.Entry(newUrl);
            return await GridRecordViewAsync(await ListOfLocalPagesEditComponent.GridRecordAsync(fieldPrefix, entry));
        }
    }
}
