/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
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

    public class UnifiedSetAddModuleController : ControllerImpl<YetaWF.Modules.Pages.Modules.UnifiedSetAddModule> {

        public UnifiedSetAddModuleController() { }

        [Trim]
        public class AddModel {

            [Caption("Name"), Description("The name of this Unified Page Set")]
            [UIHint("Text80"), StringLength(UnifiedSetData.MaxName), Required, Trim]
            public string Name { get; set; }

            [Caption("Description"), Description("The description for this Unified Page Set")]
            [UIHint("Text80"), StringLength(UnifiedSetData.MaxDescription), Required, Trim]
            public string Description { get; set; }

            [Caption("Master Page"), Description("Defines the master page for the Unified Page Set that defines the skin, referenced modules, authorization and all other page attributes - The Master Page is not automatically included in the Unified Page Set")]
            [UIHint("PageSelection"), SelectionRequired]
            public Guid MasterPageGuid { get; set; }

            [Caption("Mode"), Description("Defines how page content is combined")]
            [UIHint("Enum")]
            public PageDefinition.UnifiedModeEnum UnifiedMode { get; set; }

            [Caption("Page Skin"), Description("All pages using this skin are combined into this Unified Page Set and use the Master Page for all its attributes - Pages that are explicitly included in another Unified Page Set are not part of this set even if the skin matches")]
            [UIHint("PageSkin"), Required]
            [ProcessIf("UnifiedMode", PageDefinition.UnifiedModeEnum.SkinDynamicContent)]
            public SkinDefinition PageSkin { get; set; }

            [Caption("Popups"), Description("Defines whether popups are part of this Unified Page Set (used with SkinDynamicContent and DynamicContent only)")]
            [UIHint("Boolean")]
            [ProcessIf("UnifiedMode", PageDefinition.UnifiedModeEnum.SkinDynamicContent, PageDefinition.UnifiedModeEnum.DynamicContent)]
            public bool Popups { get; set; }

            [Caption("Animation Duration"), Description("Defines the duration of the animation to scroll to the content (used only if Mode is set to Show All Content)")]
            [UIHint("IntValue6"), Range(0, 999999), ProcessIf("UnifiedMode", PageDefinition.UnifiedModeEnum.ShowDivs), Required]
            public int UnifiedAnimation { get; set; }

            [TextAbove("Add pages to the Unified Page Set by selecting them in the dropdown list below. " +
                "When selecting a page in the \"All Pages\" grid, the dropdown list is updated to match, so the page can be added. " +
                "Pages in the Unified Page Set can be reordered by dragging and dropping them within the list of pages.")]
            [Caption("Pages"), Description("Defines all pages that are part of this Unified Page Set - All modules (in designated panes) from all pages in the set are combined into one page - Whenever a user accesses any page by Url in the Unified Page Set, the same combined page is shown, activating or scrolling to the requested content - When moving between different pages within the Unified Page Set, no server access is required")]
            [UIHint("YetaWF_Pages_ListOfLocalPages")]
            [ProcessIf("UnifiedMode", PageDefinition.UnifiedModeEnum.DynamicContent, PageDefinition.UnifiedModeEnum.HideDivs, PageDefinition.UnifiedModeEnum.ShowDivs, PageDefinition.UnifiedModeEnum.None)]
            public List<string> PageList { get; set; }
            public string PageList_AjaxUrl { get { return YetaWFManager.UrlFor(typeof(UnifiedSetAddModuleController), nameof(AddPage)); } }

            public AddModel() { }

            public UnifiedSetData GetData() {
                UnifiedSetData unifiedSet = new UnifiedSetData();
                ObjectSupport.CopyData(this, unifiedSet);
                return unifiedSet;
            }
        }

        [AllowGet]
        public ActionResult UnifiedSetAdd() {
            AddModel model = new AddModel {};
            ObjectSupport.CopyData(new UnifiedSetData(), model);
            return View(model);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> UnifiedSetAdd_Partial(AddModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);

            using (UnifiedSetDataProvider unifiedSetDP = new UnifiedSetDataProvider()) {
                if (!await unifiedSetDP.AddItemAsync(model.GetData())) {
                    ModelState.AddModelError("Name", this.__ResStr("alreadyExists", "A Unified Page Set named \"{0}\" already exists", model.Name));
                    return PartialView(model);
                }
                return FormProcessed(model, this.__ResStr("okSaved", "New Unified Page Set saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
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
            List<ListOfLocalPagesEditComponent.Entry> list = YetaWFManager.JsonDeserialize<List<ListOfLocalPagesEditComponent.Entry>>(data);
            if ((from l in list where l.Url.ToLower() == newUrl.ToLower() select l).FirstOrDefault() != null)
                throw new Error(this.__ResStr("dupUrl", "Url {0} has already been added", newUrl));
            // add new grid record
            ListOfLocalPagesEditComponent.Entry entry = new ListOfLocalPagesEditComponent.Entry(newUrl);
            return await GridRecordViewAsync(await ListOfLocalPagesEditComponent.GridRecordAsync(fieldPrefix, entry));
        }
    }
}
