/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Components;
using System.Collections.Generic;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateDropDownModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateDropDownModule> {

        public TemplateDropDownModuleController() { }

        [Trim]
        public class Model {

            public enum ControlStatusEnum { Normal, Disabled, }

            [Category("Core"), Caption("DropDownList"), Description("DropDownList (SelectionRequired)")]
            [UIHint("DropDownList"), StringLength(20)]
            [SelectionRequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string DropDownList { get; set; }
            public List<SelectionItem<string>> DropDownList_List { get; set; }

            [Category("Core"), Caption("DropDownListInt"), Description("DropDownListInt (SelectionRequired)")]
            [UIHint("DropDownListInt")]
            [SelectionRequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public int DropDownListInt { get; set; }
            public List<SelectionItem<int>> DropDownListInt_List { get; set; }

            [Category("Core"), Caption("Enum"), Description("Enum (Required)")]
            [UIHint("Enum")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public SampleEnum Enum { get; set; }

            // ENUM
            public enum SampleEnum {
                [EnumDescription("Enum 1", "Tooltip for Enum 1")]
                Value1 = 1,
                [EnumDescription("Enum 2", "Tooltip for Enum 2")]
                Value2 = 2,
                [EnumDescription("Enum 3", "Tooltip for Enum 3")]
                Value3 = 3,
            }

            [Caption("Control Status"), Description("Defines the processing status of the controls")]
            [UIHint("Enum")]
            public ControlStatusEnum ControlStatus { get; set; }

            public Model() {
                // DropDownList
                DropDownList_List = new List<SelectionItem<string>> {
                    new SelectionItem<string> { Text= "(select)", Value = "", Tooltip = "No selection" },
                    new SelectionItem<string> { Text= "Text1", Value="1", Tooltip = "Tooltip & @ # 1" },
                    new SelectionItem<string> { Text= "Text2", Value="2", Tooltip = "Tooltip & @ # 2" },
                    new SelectionItem<string> { Text= "Text3", Value="3", Tooltip = "Tooltip & @ # 3" },
                    new SelectionItem<string> { Text= "Text4", Value="4", Tooltip = "Tooltip & @ # 4" },
                    new SelectionItem<string> { Text= "Text5", Value="5", Tooltip = "Tooltip & @ # 5" },
                    new SelectionItem<string> { Text= "Text6", Value="6", Tooltip = "Tooltip & @ # 6" },
                    new SelectionItem<string> { Text= "Text7", Value="7", Tooltip = "Tooltip & @ # 7" },
                    new SelectionItem<string> { Text= "Text8", Value="8", Tooltip = "Tooltip & @ # 8" },
                    new SelectionItem<string> { Text= "Text9", Value="9", Tooltip = "Tooltip & @ # 9" },
                    new SelectionItem<string> { Text= "Text10", Value="10", Tooltip = "Tooltip & @ # 10" },
                    new SelectionItem<string> { Text= "Text11", Value="11", Tooltip = "Tooltip & @ # 11" },
                    new SelectionItem<string> { Text= "Text12", Value="12", Tooltip = "Tooltip & @ # 12" },
                    new SelectionItem<string> { Text= "Text13", Value="13", Tooltip = "Tooltip & @ # 13" },
                    new SelectionItem<string> { Text= "Text14", Value="14", Tooltip = "Tooltip & @ # 14" },
                    new SelectionItem<string> { Text= "Text15", Value="15", Tooltip = "Tooltip & @ # 15" },
                    new SelectionItem<string> { Text= "Text16", Value="16", Tooltip = "Tooltip & @ # 16" },
                    new SelectionItem<string> { Text= "Text17", Value="17", Tooltip = "Tooltip & @ # 17" },
                    new SelectionItem<string> { Text= "Text18", Value="18", Tooltip = "Tooltip & @ # 18" },
                };
                // DropDownListInt
                DropDownListInt_List = new List<SelectionItem<int>> {
                    new SelectionItem<int> { Text= "(select)", Value = 0, Tooltip = "No selection" },
                    new SelectionItem<int> { Text= "Item1", Value=1, Tooltip = "Tooltip & @ # 1" },
                    new SelectionItem<int> { Text= "Item2", Value=2, Tooltip = "Tooltip & @ # 2" },
                    new SelectionItem<int> { Text= "Item3", Value=3, Tooltip = "Tooltip & @ # 3" },
                    new SelectionItem<int> { Text= "Item4", Value=4, Tooltip = "Tooltip & @ # 4" },
                    new SelectionItem<int> { Text= "Item5", Value=5, Tooltip = "Tooltip & @ # 5" },
                    new SelectionItem<int> { Text= "Item6", Value=6, Tooltip = "Tooltip & @ # 6" },
                    new SelectionItem<int> { Text= "Item7", Value=7, Tooltip = "Tooltip & @ # 7" },
                    new SelectionItem<int> { Text= "Item8", Value=8, Tooltip = "Tooltip & @ # 8" },
                    new SelectionItem<int> { Text= "Item9", Value=9, Tooltip = "Tooltip & @ # 9" },
                    new SelectionItem<int> { Text= "Item10", Value=10, Tooltip = "Tooltip & @ # 10" },
                    new SelectionItem<int> { Text= "Item11", Value=11, Tooltip = "Tooltip & @ # 11" },
                    new SelectionItem<int> { Text= "Item12", Value=12, Tooltip = "Tooltip & @ # 12" },
                    new SelectionItem<int> { Text= "Item13", Value=13, Tooltip = "Tooltip & @ # 13" },
                    new SelectionItem<int> { Text= "Item14", Value=14, Tooltip = "Tooltip & @ # 14" },
                    new SelectionItem<int> { Text= "Item15", Value=15, Tooltip = "Tooltip & @ # 15" },
                    new SelectionItem<int> { Text= "Item16", Value=16, Tooltip = "Tooltip & @ # 16" },
                    new SelectionItem<int> { Text= "Item17", Value=17, Tooltip = "Tooltip & @ # 17" },
                    new SelectionItem<int> { Text= "Item18", Value=18, Tooltip = "Tooltip & @ # 18" },
                    new SelectionItem<int> { Text= "Item19", Value=19, Tooltip = "Tooltip & @ # 19" },
                };
            }
        }

        [AllowGet]
        public ActionResult TemplateDropDown() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateDropDown_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
