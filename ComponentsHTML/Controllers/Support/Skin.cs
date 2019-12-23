/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using YetaWF.Core.Addons;
using YetaWF.Core.Identity;
using YetaWF.Core.Support;
using YetaWF.Core.Controllers;
using YetaWF.Modules.ComponentsHTML.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ComponentsHTML.Controllers {

    /// <summary>
    /// PageSkin and PopupSkin template support.
    /// </summary>
    public class SkinController : YetaWFController {

        /// <summary>
        /// Returns JSON data to replace the dropdown contents with the provided skin collection.
        /// </summary>
        /// <param name="skinCollection">The name of the skin collection.</param>
        /// <returns>JSON data to replace the dropdown contents with the provided skin collection.
        ///
        /// Works in conjunction with client-side code and the PageSkin template.</returns>
        [AllowPost]
        [ResourceAuthorize(CoreInfo.Resource_SkinLists)]
        public ActionResult GetPageSkins(string skinCollection) {
            ScriptBuilder sb = new ScriptBuilder();
            sb.Append(SkinNamePageEditComponent.RenderReplacementSkinsForCollection(skinCollection));
            return new YJsonResult { Data = sb.ToString() };
        }
        /// <summary>
        /// Returns JSON data to replace the dropdown contents with the provided skin collection.
        /// </summary>
        /// <param name="skinCollection">The name of the skin collection.</param>
        /// <returns>&lt;option&gt; HTML to replace a select statement with popup skins.
        ///
        /// Works in conjunction with client-side code and the PopupSkin template.</returns>
        [AllowPost]
        [ResourceAuthorize(CoreInfo.Resource_SkinLists)]
        public ActionResult GetPopupPageSkins(string skinCollection) {
            ScriptBuilder sb = new ScriptBuilder();
            sb.Append(SkinNamePopupEditComponent.RenderReplacementSkinsForCollection(skinCollection));
            return new YJsonResult { Data = sb.ToString() };
        }
    }
}
