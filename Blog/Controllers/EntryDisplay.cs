/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Modules;
using YetaWF.Core;
#if MVC6
using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Blog.Controllers {

    public class EntryDisplayModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.EntryDisplayModule> {

        public EntryDisplayModuleController() { }

        public class DisplayModel {

            public int Identity { get; set; }
            public int CategoryIdentity { get; set; }

            [Caption("Author"), Description("The name of the blog author")]
            [UIHint("String"), ReadOnly, SuppressIfNotEqual("AuthorUrl", null)]
            public string Author { get; set; }

            [Caption("Author"), Description("The optional Url linking to the author's information")]
            [UIHint("Url"), ReadOnly, SuppressEmpty]
            public string AuthorUrl { get; set; }
            public string AuthorUrl_Text { get { return Author; } }

            public bool Published { get; set; }

            [Caption("Date Published"), Description("The date this entry has been published")]
            [UIHint("Date"), SuppressIfEqual("Published", false), ReadOnly]
            public DateTime DatePublished { get; set; }

            //[Caption("Date Created"), Description("The date this entry was created")]
            //[UIHint("DateTime"), ReadOnly]
            //public DateTime DateCreated { get; set; }

            //[Caption("Date Updated"), Description("The date this entry was updated")]
            //[UIHint("DateTime"), ReadOnly]
            //public DateTime DateUpdated { get; set; }

            [Caption("Blog Text"), Description("The complete text for this blog entry")]
            [UIHint("TextArea"), AdditionalMetadata("Encode", false), ReadOnly]
            public string Text { get; set; }

            public void SetData(BlogEntry data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        [AllowGet]
        public ActionResult EntryDisplay(int? blogEntry) {
            int entryNum = blogEntry ?? 0;
            using (BlogEntryDataProvider dataProvider = new BlogEntryDataProvider()) {
                BlogEntry data = null;
                if (entryNum != 0)
                    data = dataProvider.GetItem(entryNum);
                if (data == null) {
                    MarkNotFound();
                    return View("NotFound");
                }

                Manager.CurrentPage.EvaluatedCanonicalUrl = BlogConfigData.GetEntryCanonicalName(entryNum);
                if (!string.IsNullOrWhiteSpace(data.Keywords)) {
                    Manager.CurrentPage.Keywords = data.Keywords;
                    Manager.MetatagsManager.AddMetatag("news_keywords", data.Keywords.ToString());
                }
                Manager.CurrentPage.Description = data.Title;
                Manager.PageTitle = data.Title;

                DisplayModel model = new DisplayModel();
                model.SetData(data);
                Module.Title = data.Title;
                return View(model);
            }
        }
    }
}
