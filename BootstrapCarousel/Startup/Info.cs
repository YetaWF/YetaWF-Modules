﻿/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.BootstrapCarousel.Controllers;
using YetaWF.Modules.BootstrapCarousel.Models;

namespace YetaWF.Modules.BootstrapCarousel.Addons.Templates {

    /// <summary>
    /// Template specific config strings.
    /// </summary>
    public class SlideShow : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddConfigOption(areaName, "Action_Apply", CarouselInfo.CarouselAction.Apply);
            scripts.AddConfigOption(areaName, "Action_MoveLeft", CarouselInfo.CarouselAction.MoveLeft);
            scripts.AddConfigOption(areaName, "Action_MoveRight", CarouselInfo.CarouselAction.MoveRight);
            scripts.AddConfigOption(areaName, "Action_Add", CarouselInfo.CarouselAction.Add);
            scripts.AddConfigOption(areaName, "Action_Insert", CarouselInfo.CarouselAction.Insert);
            scripts.AddConfigOption(areaName, "Action_Remove", CarouselInfo.CarouselAction.Remove);

            scripts.AddLocalization(areaName, "RemoveConfirm", this.__ResStr("removeConfirm", "Are you sure you want to remove this image?"));
            scripts.AddLocalization(areaName, "RemoveTitle", this.__ResStr("removeTitle", "Remove Image"));

            return Task.CompletedTask;
        }
    }
}