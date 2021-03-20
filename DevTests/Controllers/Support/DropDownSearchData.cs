/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.DevTests.Controllers {

    public class DropDownSearchDataController : YetaWFController {

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult DropDownSearchData_GetData(string? search) {
            search = search?.ToLower();
            List<SelectionItem<int>> list = GetSampleData();
            if (search != null)
                list = (from l in list where l.Text.ToString().ToLower().Contains(search) select l).Take(50).ToList();
            else
                list = list.Take(50).ToList();
            return new DropDownSearchResult<int>(list);
        }

        public static List<SelectionItem<int>> GetSampleData() {
            List<SelectionItem<int>> list = new List<SelectionItem<int>>();
            for (int i = 1; i < 100; ++i) {
                list.Add(new SelectionItem<int> { Value = i, Text = $"Item {i}", Tooltip = $"Tooltip for item {i}" });
            }
            return list;
        }
    }
}
