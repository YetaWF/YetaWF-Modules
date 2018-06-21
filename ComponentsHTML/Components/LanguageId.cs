/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class LanguageIdComponent : YetaWFComponent, IYetaWFComponent<string> {

        public const string TemplateName = "LanguageId";

        public override ComponentType GetComponentType() { return ComponentType.Edit; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public async Task<YHtmlString> RenderAsync(string model) {

            bool useDefault = !PropData.GetAdditionalAttributeValue<bool>("NoDefault");
            bool allLanguages = PropData.GetAdditionalAttributeValue<bool>("AllLanguages");

            List<SelectionItem<string>> list;
            if (allLanguages) {
                CultureInfo[] ci = CultureInfo.GetCultures(CultureTypes.AllCultures);
                list = (from c in ci orderby c.DisplayName select new SelectionItem<string>() {
                    Text = string.Format("{0} - {1}", c.DisplayName, c.Name),
                    Value = c.Name,
                }).ToList();
            } else {
                list = (from l in MultiString.Languages select new SelectionItem<string>() {
                    Text = l.ShortName,
                    Tooltip = l.Description,
                    Value = l.Id,
                }).ToList();
            }
            if (useDefault) {
                list.Insert(0, new SelectionItem<string> {
                    Text = this.__ResStr("default", "(Site Default)"),
                    Tooltip = this.__ResStr("defaultTT", "Use the site defined default language"),
                    Value = "",
                });
            } else {
                if (string.IsNullOrWhiteSpace(model))
                    model = MultiString.ActiveLanguage;
            }
            // display the languages in a drop down
            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_languageid");
        }
    }
}
