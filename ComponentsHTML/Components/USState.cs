﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Templates;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class USStateComponent : YetaWFComponent {

        public const string TemplateName = "USState";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    public class USStateDisplayComponent : USStateComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(string model) {
            List<SelectionItem<string>> states = await USState.ReadStatesListAsync();
            if (model == null) model = "";
            string state = (from s in states where string.Compare(s.Value, model.ToUpper(), true) == 0 select s.Text).FirstOrDefault();
            return new YHtmlString(state);
        }
    }
    public class USStateEditComponent : USStateComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(string model) {

            List<SelectionItem<string>> states = await USState.ReadStatesListAsync();

            bool useDefault = !PropData.GetAdditionalAttributeValue<bool>("NoDefault");
            if (useDefault) {
                states = (from s in states select s).ToList();//copy
                states.Insert(0, new SelectionItem<string> {
                    Text = this.__ResStr("default", "(select)"),
                    Tooltip = this.__ResStr("defaultTT", "Please make a selection"),
                    Value = "",
                });
            }
            return await DropDownListComponent.RenderDropDownListAsync(model, states, this, "yt_usstate");
        }
    }
}
