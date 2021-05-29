/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Addons;
using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.ComponentsHTML.Controllers {

    /// <summary>
    /// Skin template support.
    /// </summary>
    public class SkinController : YetaWFController {

        internal class Lists {
            public string PagesHTML { get; set; } = null!;
            public string PopupsHTML { get; set; } = null!;
        }

        /// <summary>
        /// Returns JSON data to replace the dropdown contents with the provided page/popup names.
        /// </summary>
        /// <param name="skinCollection">The name of the skin collection.</param>
        /// <returns>JSON data to replace the dropdown contents with the provided skin collection.
        ///
        /// Works in conjunction with client-side code and the Skin template.</returns>
        [AllowPost]
        [ResourceAuthorize(CoreInfo.Resource_SkinLists)]
        public ActionResult GetSkins(string skinCollection) {
            Lists lists = new Lists {
                PagesHTML = SkinNamePageEditComponent.RenderReplacementSkinsForCollection(skinCollection),
                PopupsHTML = SkinNamePopupEditComponent.RenderReplacementSkinsForCollection(skinCollection),
            };
            return new YJsonResult { Data = lists };
        }
    }
}
