/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML {

    public partial class CoreRendering {

        /// <summary>
        /// Renders a form button.
        /// </summary>
        /// <param name="formButton">The form button to render.</param>
        /// <returns>Returns the rendered form button as HTML.</returns>
        public Task<string> RenderFormButtonAsync(FormButton formButton) {
            return Task.FromResult(RenderFormButton(formButton));
        }
        private string RenderFormButton(FormButton formButton) {

            Dictionary<string, object> attrs = new Dictionary<string, object>();
            string css = null;

            string text = formButton.Text;
            switch (formButton.ButtonType) {
                case ButtonTypeEnum.Submit:
                case ButtonTypeEnum.ConditionalSubmit:
                    if (formButton.ButtonType == ButtonTypeEnum.ConditionalSubmit && !Manager.IsInPopup && !Manager.HaveReturnToUrl) {
                        // if we don't have anyplace to return to and we're not in a popup we don't need a submit button
                        return null;
                    }
                    if (string.IsNullOrWhiteSpace(text)) text = this.__ResStr("btnSave", "Save");
                    attrs.Add("type", "submit");
                    break;
                case ButtonTypeEnum.Apply:
                    if (string.IsNullOrWhiteSpace(text)) text = this.__ResStr("btnApply", "Apply");
                    attrs.Add("type", "button");
                    attrs.Add(Forms.CssDataApplyButton, string.Empty);
                    break;
                default:
                case ButtonTypeEnum.Button:
                    attrs.Add("type", "button");
                    break;
                case ButtonTypeEnum.Cancel:
                    //if (!Manager.IsInPopup && !Manager.HaveReturnToUrl) {
                    //    // if we don't have anyplace to return to and we're not in a popup so we don't need a cancel button
                    //    return null;
                    //}
                    if (string.IsNullOrWhiteSpace(text)) text = this.__ResStr("btnCancel", "Cancel");
                    attrs.Add("type", "button");
                    css = CssManager.CombineCss(css, Manager.AddOnManager.CheckInvokedCssModule(Forms.CssFormCancel));
                    break;
            }
            if (!string.IsNullOrWhiteSpace(formButton.Name))
                attrs.Add("name", formButton.Name);
            if (formButton.Hidden)
                attrs.Add("style", "display:none");
            if (!string.IsNullOrWhiteSpace(formButton.Title))
                attrs.Add("title", formButton.Title);

            if (!string.IsNullOrWhiteSpace(formButton.CssClass))
                css = CssManager.CombineCss(css, formButton.CssClass);

            string id = null;
            if (!string.IsNullOrWhiteSpace(formButton.Id))
                id = $" id='{formButton.Id}'";

            return $@"<input{id}{HtmlBuilder.GetClassAttribute(attrs, css)} value='{Utility.HAE(text)}'{HtmlBuilder.Attributes(attrs)}>";
        }
    }
}
