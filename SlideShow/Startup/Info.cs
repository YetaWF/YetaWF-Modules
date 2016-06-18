/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/SlideShow#License */

using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.SlideShow.Controllers;
using YetaWF.Modules.SlideShow.Models;

namespace YetaWF.Modules.SlideShow.Addons {

    public class Info : IAddOnSupport {

        public void AddSupport(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddConfigOption(areaName, "Action_MoveLeft", SlideShowInfo.SlideShowAction.MoveLeft);
            scripts.AddConfigOption(areaName, "Action_MoveRight", SlideShowInfo.SlideShowAction.MoveRight);
            scripts.AddConfigOption(areaName, "Action_Add", SlideShowInfo.SlideShowAction.Add);
            scripts.AddConfigOption(areaName, "Action_Insert", SlideShowInfo.SlideShowAction.Insert);
            scripts.AddConfigOption(areaName, "Action_Remove", SlideShowInfo.SlideShowAction.Remove);

            scripts.AddLocalization(areaName, "RemoveConfirm", this.__ResStr("removeConfirm", "Are you sure you want to remove this image?"));
            scripts.AddLocalization(areaName, "RemoveTitle", this.__ResStr("removeTitle", "Remove Image"));
        }
    }
}