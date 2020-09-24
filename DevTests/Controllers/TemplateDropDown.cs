/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core;
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
            [UIHint("DropDown2List"), StringLength(20)]
            [SelectionRequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string DropDownList { get; set; }
            public List<SelectionItem<string>> DropDownList_List { get; set; }

            [Category("Core"), Caption("DropDownListInt"), Description("DropDownListInt (SelectionRequired)")]
            [UIHint("DropDown2ListInt")]
            [SelectionRequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public int DropDownListInt { get; set; }
            public List<SelectionItem<int>> DropDownListInt_List { get; set; }

            //$$$ [Category("Core"), Caption("Enum"), Description("Enum (Required)")]
            //[UIHint("Enum")]
            //[RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            //[ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            //public SampleEnum Enum { get; set; }

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
                    new SelectionItem<string> { Text= "Text1", Value="1", Tooltip = "Tooltip1" },
                    new SelectionItem<string> { Text= "Text2", Value="2", Tooltip = "Tooltip2" },
                    new SelectionItem<string> { Text= "Text3", Value="3", Tooltip = "Tooltip3" },
                    new SelectionItem<string> { Text= "Text1", Value="1", Tooltip = "Tooltip1" },
                    new SelectionItem<string> { Text= "Text2", Value="2", Tooltip = "Tooltip2" },
                    new SelectionItem<string> { Text= "Text3", Value="3", Tooltip = "Tooltip3" },
                    new SelectionItem<string> { Text= "Text1", Value="1", Tooltip = "Tooltip1" },
                    new SelectionItem<string> { Text= "Text2", Value="2", Tooltip = "Tooltip2" },
                    new SelectionItem<string> { Text= "Text3", Value="3", Tooltip = "Tooltip3" },
                    new SelectionItem<string> { Text= "Text1", Value="1", Tooltip = "Tooltip1" },
                    new SelectionItem<string> { Text= "Text2", Value="2", Tooltip = "Tooltip2" },
                    new SelectionItem<string> { Text= "Text3", Value="3", Tooltip = "Tooltip3" },
                    new SelectionItem<string> { Text= "Text1", Value="1", Tooltip = "Tooltip1" },
                    new SelectionItem<string> { Text= "Text2", Value="2", Tooltip = "Tooltip2" },
                    new SelectionItem<string> { Text= "Text3", Value="3", Tooltip = "Tooltip3" },
                    new SelectionItem<string> { Text= "Text1", Value="1", Tooltip = "Tooltip1" },
                    new SelectionItem<string> { Text= "Text2", Value="2", Tooltip = "Tooltip2" },
                    new SelectionItem<string> { Text= "Text3", Value="3", Tooltip = "Tooltip3" },
                };
                // DropDownListInt
                DropDownListInt_List = new List<SelectionItem<int>> {
                    new SelectionItem<int> { Text= "(select)", Value = 0, Tooltip = "No selection" },
                    new SelectionItem<int> { Text= "Item1", Value=1, Tooltip = "Tooltip1" },
                    new SelectionItem<int> { Text= "Item2", Value=2, Tooltip = "Tooltip2" },
                    new SelectionItem<int> { Text= "Item3", Value=3, Tooltip = "Tooltip3" },
                    new SelectionItem<int> { Text= "Item1", Value=1, Tooltip = "Tooltip1" },
                    new SelectionItem<int> { Text= "Item2", Value=2, Tooltip = "Tooltip2" },
                    new SelectionItem<int> { Text= "Item3", Value=3, Tooltip = "Tooltip3" },
                    new SelectionItem<int> { Text= "Item1", Value=1, Tooltip = "Tooltip1" },
                    new SelectionItem<int> { Text= "Item2", Value=2, Tooltip = "Tooltip2" },
                    new SelectionItem<int> { Text= "Item3", Value=3, Tooltip = "Tooltip3" },
                    new SelectionItem<int> { Text= "Item1", Value=1, Tooltip = "Tooltip1" },
                    new SelectionItem<int> { Text= "Item2", Value=2, Tooltip = "Tooltip2" },
                    new SelectionItem<int> { Text= "Item3", Value=3, Tooltip = "Tooltip3" },
                    new SelectionItem<int> { Text= "Item1", Value=1, Tooltip = "Tooltip1" },
                    new SelectionItem<int> { Text= "Item2", Value=2, Tooltip = "Tooltip2" },
                    new SelectionItem<int> { Text= "Item3", Value=3, Tooltip = "Tooltip3" },
                    new SelectionItem<int> { Text= "Item1", Value=1, Tooltip = "Tooltip1" },
                    new SelectionItem<int> { Text= "Item2", Value=2, Tooltip = "Tooltip2" },
                    new SelectionItem<int> { Text= "Item3", Value=3, Tooltip = "Tooltip3" },
                    new SelectionItem<int> { Text= "Item1", Value=1, Tooltip = "Tooltip1" },
                    new SelectionItem<int> { Text= "Item2", Value=2, Tooltip = "Tooltip2" },
                    new SelectionItem<int> { Text= "Item3", Value=3, Tooltip = "Tooltip3" },
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
