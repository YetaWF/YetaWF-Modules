﻿/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class CAProvinceComponent : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(CAProvinceComponent), name, defaultValue, parms); }

        public const string TemplateName = "CAProvince";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    public class CAProvinceDisplayComponent : CAProvinceComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(string model) {
            List<SelectionItem<string>> states = await CAProvince.ReadProvincesListAsync();
            if (model == null) model = "";
            string state = (from s in states where string.Compare(s.Value, model.ToUpper(), true) == 0 select s.Text).FirstOrDefault();
            return new YHtmlString(HE(state));
        }
    }
    public class CAProvinceEditComponent : CAProvinceComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(string model) {

            List<SelectionItem<string>> states = await CAProvince.ReadProvincesListAsync();

            bool useDefault = !PropData.GetAdditionalAttributeValue<bool>("NoDefault");
            if (useDefault) {
                states = (from s in states select s).ToList();//copy
                states.Insert(0, new SelectionItem<string> {
                    Text = __ResStr("default", "(select)"),
                    Tooltip = __ResStr("defaultTT", "Please make a selection"),
                    Value = "",
                });
            }
            return await DropDownListComponent.RenderDropDownListAsync(this, model, states, "yt_caprovince");
        }
    }
}
