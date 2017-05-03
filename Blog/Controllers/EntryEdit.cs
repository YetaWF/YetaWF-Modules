/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Blog.Controllers {

    public class EntryEditModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.EntryEditModule> {

        public EntryEditModuleController() { }

        [Trim]
        public class EditModel {

            [UIHint("Hidden")]
            public int Identity { get; set; }
            [UIHint("Hidden")]
            public int OriginalCategoryIdentity { get; set; }
            [UIHint("Hidden")]
            public Guid UniqueId { get; set; }

            [Caption("Id"), Description("The id of this blog entry - used to uniquely identify this blog entry internally")]
            [UIHint("IntValue"), ReadOnly]
            public int DisplayIdentity { get; set; }

            [Caption("Category"), Description("The category for this blog entry")]
            [UIHint("YetaWF_Blog_Category"), Required]
            public int CategoryIdentity { get; set; }

            [Caption("Title"), Description("The title for this blog entry")]
            [UIHint("MultiString"), StringLength(BlogEntry.MaxTitle), Required, Trim]
            public MultiString Title { get; set; }

            [Caption("Keywords"), Description("The keywords for this blog entry")]
            [UIHint("MultiString"), StringLength(BlogEntry.MaxKwds), Trim]
            public MultiString Keywords { get; set; }

            [Caption("Author"), Description("The name of the blog author")]
            [UIHint("Text40"), StringLength(BlogEntry.MaxAuthor), Required, Trim]
            public string Author { get; set; }

            [Caption("Allow Comments"), Description("Defines whether comments can be entered for this blog entry")]
            [UIHint("Boolean")]
            public bool OpenForComments { get; set; }

            [Caption("Published"), Description("Defines whether this entry has been published and is viewable by everyone")]
            [UIHint("Boolean")]
            public bool Published { get; set; }
            [Caption("Date Published"), Description("The date this entry has been published")]
            [UIHint("Date"), RequiredIf("Published", true)]
            public DateTime DatePublished { get; set; }

            [Caption("Date Created"), Description("The date this entry was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime DateCreated { get; set; }

            [Caption("Date Updated"), Description("The date this entry was updated")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime DateUpdated { get; set; }

            [Caption("Summary"), Description("The summary for this blog entry - If no summary is entered, the entire blog text is shown instead of the summary")]
            [UIHint("TextArea"), AdditionalMetadata("EmHeight", 10), StringLength(BlogEntry.MaxSummary)]
            [AdditionalMetadata("TextAreaSave", false), AdditionalMetadata("ImageBrowse", true), AdditionalMetadata("FlashBrowse", true), AdditionalMetadata("PageBrowse", false)]
            public string Summary { get; set; }

            [Caption("Blog Text"), Description("The complete text for this blog entry")]
            [UIHint("TextArea"), AdditionalMetadata("EmHeight", 20), StringLength(BlogEntry.MaxText)]
            [AdditionalMetadata("TextAreaSave", false), AdditionalMetadata("ImageBrowse", true), AdditionalMetadata("FlashBrowse", true), AdditionalMetadata("PageBrowse", false)]
            [RequiredIf("Published", true)]
            public string Text { get; set; }

            public Guid Text_Folder { get { return BlogEntry.FolderGuid; } }
            public Guid Text_SubFolder { get { return UniqueId; } }
            public Guid Summary_Folder { get { return Text_Folder; } }
            public Guid Summary_SubFolder { get { return UniqueId; } }

            public BlogEntry GetData(BlogEntry data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }

            public void SetData(BlogEntry data) {
                ObjectSupport.CopyData(data, this);
                DisplayIdentity = Identity;
                OriginalCategoryIdentity = data.CategoryIdentity;
            }
            public EditModel() { }
        }

        [HttpGet]
        public ActionResult EntryEdit(int blogEntry) {
            using (BlogEntryDataProvider dataProvider = new BlogEntryDataProvider()) {
                EditModel model = new EditModel { };
                BlogEntry data = dataProvider.GetItem(blogEntry);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Blog entry with id {0} not found"), blogEntry);
                model.SetData(data);
                Module.Title = data.Title;
                return View(model);
            }
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult EntryEdit_Partial(EditModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            using (BlogEntryDataProvider dataProvider = new BlogEntryDataProvider()) {
                BlogEntry data = dataProvider.GetItem(model.Identity);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Blog entry with id {0} not found"), model.Identity);
                // save updated item
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display

                UpdateStatusEnum status = dataProvider.UpdateItem(data);
                if (status != UpdateStatusEnum.OK) {
                    ModelState.AddModelError("Name", this.__ResStr("errSaving", "An unexpected error occurred saving the blog entry - {0}", status));
                    return PartialView(model);
                }
                return FormProcessed(model, this.__ResStr("okSaved", "Blog entry saved"));
            }
        }
    }
}