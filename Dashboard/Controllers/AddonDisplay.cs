/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Dashboard.Controllers {

    public class AddonDisplayModuleController : ControllerImpl<YetaWF.Modules.Dashboard.Modules.AddonDisplayModule> {

        public AddonDisplayModuleController() { }

        public class DisplayModel {

            [Caption("Type"), Description("The AddOn type")]
            [UIHint("Enum"), ReadOnly]
            public Package.AddOnType Type { get; set; }
            [Caption("Domain"), Description("The domain owning this AddOn")]
            [UIHint("String"), ReadOnly]
            public string Domain { get; set; } = null!;
            [Caption("Product"), Description("The AddOn's product name")]
            [UIHint("String"), ReadOnly]
            public string Product { get; set; } = null!;
            [Caption("Version"), Description("The AddOn's version")]
            [UIHint("String"), ReadOnly]
            public string Version { get; set; } = null!;
            [Caption("Name"), Description("The AddOn's internal name")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; } = null!;
            [Caption("Url"), Description("The AddOn's Url where its files are located")]
            [UIHint("String"), ReadOnly]
            public string Url { get; set; } = null!;

            [Caption("Javascript Files"), Description("List of Javascript files for this AddOn (filelistJS.txt file contents)")]
            [UIHint("ListOfStrings"), AdditionalMetadata("Delimiter", "<br/>"), ReadOnly]
            public List<string> JsFiles { get; set; } = null!;
            [Caption("Javascript Path"), Description("The AddOn's location for Javascript files (overrides Url) - only used if a Folder directive was found in filelistJS.txt")]
            [UIHint("String"), ReadOnly]
            public string? JsPathUrl { get; set; }
            [Caption("Css Files"), Description("List of Css files for this AddOn (filelistCSS.txt file contents)")]
            [UIHint("ListOfStrings"), AdditionalMetadata("Delimiter", "<br/>"), ReadOnly]
            public List<string>? CssFiles { get; set; }
            [Caption("Css Path"), Description("The AddOn's location for Css files (overrides Url) - only used if a Folder directive was found in filelistCSS.txt")]
            [UIHint("String"), ReadOnly]
            public string CssPathUrl { get; set; } = null!;

            [Caption("Support Types"), Description("List of types that define the IAddOnSupport interface for this AddOn, used to add Localization and Config information for client-side Javascript use")]
            [UIHint("ListOfStrings"), AdditionalMetadata("Delimiter", "<br/>"), ReadOnly]
            public List<string> SupportTypesStrings { get; set; } = null!;

            [Caption("Skin Definition Path"), Description("The location where the Skin.txt file is located defining the Skin attributes - only used for Skin AddOns")]
            [UIHint("String"), ReadOnly]
            public string? SkinFilePath { get; set; }

            public void SetData(Package.AddOnProduct data) {
                ObjectSupport.CopyData(data, this);
                JsPathUrl = Utility.PhysicalToUrl(data.JsPath);
                CssPathUrl = Utility.PhysicalToUrl(data.CssPath);
                SupportTypesStrings = new List<string>();
                foreach (Type t in data.SupportTypes) {
                    SupportTypesStrings.Add(t.FullName!);
                }
                if (data.SkinInfo != null)
                    SkinFilePath = data.SkinInfo.Folder;
            }
        }

        [AllowGet]
        public ActionResult AddonDisplay(string key) {
            List<Package.AddOnProduct> list = Package.GetAvailableAddOns();
            Package.AddOnProduct? data = (from l in list where l.AddonKey == key select l).FirstOrDefault();
            if (data == null)
                throw new Error(this.__ResStr("notFound", "AddOn Info for key \"{0}\" not found"), key);
            DisplayModel model = new DisplayModel();
            model.SetData(data);
            return View(model);
        }
    }
}
