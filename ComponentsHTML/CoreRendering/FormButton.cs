/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Support;
using YetaWF.Core.Localize;
using YetaWF.Core.Addons;

namespace YetaWF.Modules.ComponentsHTML {

    public partial class CoreRendering {

        /// <summary>
        /// Renders a form button.
        /// </summary>
        /// <param name="formButton">The form button to render.</param>
        /// <returns>Returns the rendered form button as HTML.</returns>
        public Task<YHtmlString> RenderFormButtonAsync(FormButton formButton) {
            return Task.FromResult(RenderFormButton(formButton));
        }
        private YHtmlString RenderFormButton(FormButton formButton) {

            YTagBuilder tag = new YTagBuilder("input");

            string text = formButton.Text;
            switch (formButton.ButtonType) {
                case ButtonTypeEnum.Submit:
                case ButtonTypeEnum.ConditionalSubmit:
                    if (formButton.ButtonType == ButtonTypeEnum.ConditionalSubmit && !Manager.IsInPopup && !Manager.HaveReturnToUrl) {
                        // if we don't have anyplace to return to and we're not in a popup we don't need a submit button
                        return new YHtmlString("");
                    }
                    if (string.IsNullOrWhiteSpace(text)) text = this.__ResStr("btnSave", "Save");
                    tag.Attributes.Add("type", "submit");
                    break;
                case ButtonTypeEnum.Apply:
                    if (string.IsNullOrWhiteSpace(text)) text = this.__ResStr("btnApply", "Apply");
                    tag.Attributes.Add("type", "button");
                    tag.Attributes.Add(Forms.CssDataApplyButton, "");
                    break;
                default:
                case ButtonTypeEnum.Button:
                    tag.Attributes.Add("type", "button");
                    break;
                case ButtonTypeEnum.Cancel:
                    if (!Manager.IsInPopup && !Manager.HaveReturnToUrl) {
                        // if we don't have anyplace to return to and we're not in a popup so we don't need a cancel button
                        return new YHtmlString("");
                    }
                    if (string.IsNullOrWhiteSpace(text)) text = this.__ResStr("btnCancel", "Cancel");
                    tag.Attributes.Add("type", "button");
                    tag.AddCssClass(Manager.AddOnManager.CheckInvokedCssModule(Forms.CssFormCancel));
                    break;
            }
            if (!string.IsNullOrWhiteSpace(formButton.Id))
                tag.Attributes.Add("id", formButton.Id);
            if (!string.IsNullOrWhiteSpace(formButton.Name))
                tag.Attributes.Add("name", formButton.Name);
            if (formButton.Hidden)
                tag.Attributes.Add("style", "display:none");
            if (!string.IsNullOrWhiteSpace(formButton.Title))
                tag.Attributes.Add("title", formButton.Title);
            tag.Attributes.Add("value", text);
            if (!string.IsNullOrWhiteSpace(formButton.CssClass))
                tag.AddCssClass(formButton.CssClass);

            return tag.ToYHtmlString(YTagRenderMode.StartTag);
        }
    }
}
