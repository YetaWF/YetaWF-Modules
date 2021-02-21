/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Search display component implementation.
    /// </summary>
    public class SearchEditComponent : YetaWFComponent, IYetaWFComponent<string> {

        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
        public override string GetTemplateName() { return "Search"; }

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <inheritdoc/>
        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddTemplateAsync(AreaRegistration.CurrentPackage.AreaName, "Number", ComponentType.Edit);
            await base.IncludeAsync();
        }

        internal class SearchEditSetup {
            public int AutoClickDelay { get; set; }
        }

        /// <inheritdoc/>
        public Task<string> RenderAsync(string model) {

            bool rdonly = PropData.GetAdditionalAttributeValue<bool>("ReadOnly", false);
            bool disabled = PropData.GetAdditionalAttributeValue<bool>("Disabled", false);
            string dis = string.Empty;
            if (disabled)
                dis = " disabled='disabled'";
            string ro = string.Empty;
            if (rdonly)
                ro = " readonly='readonly'";

            if (!HtmlAttributes.TryGetValue("AutoDelay", out object? objAutoDelay))
                objAutoDelay = 0;
            int autoDelay = Convert.ToInt32(objAutoDelay);

            TryGetSiblingProperty<string>($"{PropertyName}_PlaceHolder", out string? placeHolder);
            placeHolder = string.IsNullOrWhiteSpace(placeHolder) ? string.Empty : $" placeholder={HAE(placeHolder)}";

            string? autoComplete = PropData.GetAdditionalAttributeValue<string>("AutoComplete");
            if (autoComplete == null) {
                if (Manager.CurrentModule != null && Manager.CurrentModule.FormAutoComplete)
                    autoComplete = "on";
                else
                    autoComplete = "off";
            }

            // handle StringLengthAttribute as maxlength
            StringLengthAttribute? lenAttr = PropData.TryGetAttribute<StringLengthAttribute>();
            if (lenAttr != null) {
#if DEBUG
                if (HtmlAttributes.ContainsKey("maxlength"))
                    throw new InternalError("Both StringLengthAttribute and maxlength specified - {0}", FieldName);
#endif
                int maxLength = lenAttr.MaximumLength;
                if (maxLength > 0 && maxLength <= 8000)
                    HtmlAttributes.Add("maxlength", maxLength.ToString());
            }
#if DEBUG
            if (lenAttr == null && !HtmlAttributes.ContainsKey("maxlength"))
                throw new InternalError("No max string length given using StringLengthAttribute or maxlength - {0}", FieldName);
#endif

            string tags =
            $@"<div class='yt_search_container{(disabled ? " t_disabled" : "")}{(rdonly ? " t_readonly" : "")}'>
                <input type='text'{FieldSetup(Validation ? FieldType.Validated : FieldType.Normal)} id='{ControlId}' class='{GetClasses("yt_search t_edit")}' maxlength='20' value='{HAE(model)}' autocomplete='{autoComplete}'{dis}{ro}{placeHolder}>
                <div class='t_search'>
                    {SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-search")}
                </div>
            </div>";

            SearchEditSetup setup = new SearchEditSetup {
                AutoClickDelay = autoDelay
            };

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.SearchEditComponent('{ControlId}', {Utility.JsonSerialize(setup)});");

            return Task.FromResult(tags);
        }
    }
}
