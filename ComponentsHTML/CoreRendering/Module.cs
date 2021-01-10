/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Addons;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.ComponentsHTML {

    public partial class CoreRendering {

        private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }
        private static Package Package { get { return AreaRegistration.CurrentPackage; } }

        /// <summary>
        /// Renders module links.
        /// </summary>
        /// <param name="mod">The module for which the module links are rendered.</param>
        /// <param name="renderMode">The module links' rendering mode.</param>
        /// <param name="cssClass">The optional CSS classes to use for the module links.</param>
        /// <returns>Returns the module links as HTML.</returns>
        public async Task<string> RenderModuleLinksAsync(ModuleDefinition mod, ModuleAction.RenderModeEnum renderMode, string cssClass) {

            HtmlBuilder hb = new HtmlBuilder();

            MenuList moduleMenu = await mod.GetModuleMenuListAsync(renderMode, ModuleAction.ActionLocationEnum.ModuleLinks);

            string menuContents = (await MenuDisplayComponent.RenderMenuAsync(moduleMenu, null, Globals.CssModuleLinks));
            if (!string.IsNullOrWhiteSpace(menuContents)) {

                await Manager.AddOnManager.AddTemplateFromUIHintAsync("ActionIcons", YetaWFComponentBase.ComponentType.Display); // action icons

                hb.Append($@"
<div class='{Manager.AddOnManager.CheckInvokedCssModule(cssClass)}'>
    {menuContents}
</div>");
            }
            return hb.ToString();
        }

        /// <summary>
        /// Renders a complete module menu.
        /// </summary>
        /// <param name="mod">The module for which the module menu is rendered.</param>
        /// <returns>Returns the complete module menu as HTML.</returns>
        public async Task<string> RenderModuleMenuAsync(ModuleDefinition mod) {

            HtmlBuilder hb = new HtmlBuilder();

            MenuList moduleMenu = await mod.GetModuleMenuListAsync(ModuleAction.RenderModeEnum.NormalMenu, ModuleAction.ActionLocationEnum.ModuleMenu);

            string id = Manager.UniqueId();
            string menuContents = (await MenuDisplayComponent.RenderMenuAsync(moduleMenu, id, Globals.CssModuleMenu));
            if (!string.IsNullOrWhiteSpace(menuContents)) {

                await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "Modules");// various module support
                await Manager.AddOnManager.AddTemplateAsync(YetaWF.Modules.ComponentsHTML.AreaRegistration.CurrentPackage.AreaName, "MenuUL", YetaWFComponentBase.ComponentType.Display);

                hb.Append($@"
<div class={Manager.AddOnManager.CheckInvokedCssModule(Globals.CssModuleMenuEditIcon)} style='display:none'>
    {ImageHTML.BuildKnownIcon("#ModuleMenuEdit", sprites: Info.PredefSpriteIcons, title: null /*no tooltip here as it's useless */)}
    <div class='{Manager.AddOnManager.CheckInvokedCssModule(Globals.CssModuleMenuContainer)}'>
        {menuContents}
    </div>
</div>");
            }
            return hb.ToString();
        }

        /// <summary>
        /// Renders a module action.
        /// </summary>
        /// <param name="action">The module action to render.</param>
        /// <param name="mode">The module action's rendering mode.</param>
        /// <param name="id">The ID to generate.</param>
        /// <returns>Returns the module action as HTML.</returns>
        public async Task<string> RenderModuleActionAsync(ModuleAction action, ModuleAction.RenderModeEnum mode, string id) {
            return await RenderActionAsync(action, mode, id);
        }

        internal static async Task<string> RenderActionAsync(ModuleAction action, ModuleAction.RenderModeEnum mode, string id) {

            // check if we're in the right mode
            if (!await action.RendersSomethingAsync()) return null;

            await Manager.AddOnManager.AddTemplateFromUIHintAsync("ActionIcons", YetaWFComponentBase.ComponentType.Display);// this is needed because we're not used by templates

            if (!string.IsNullOrWhiteSpace(action.ConfirmationText) && (action.Style != ModuleAction.ActionStyleEnum.Post && action.Style != ModuleAction.ActionStyleEnum.Nothing))
                throw new InternalError("When using ConfirmationText, the Style property must be set to Post");
            if (!string.IsNullOrWhiteSpace(action.PleaseWaitText) && (action.Style != ModuleAction.ActionStyleEnum.Normal && action.Style != ModuleAction.ActionStyleEnum.Post))
                throw new InternalError("When using PleaseWaitText, the Style property must be set to Normal or Post");
            if (action.CookieAsDoneSignal && action.Style != ModuleAction.ActionStyleEnum.Normal)
                throw new InternalError("When using CookieAsDoneSignal, the Style property must be set to Normal");

            ModuleAction.ActionStyleEnum style = action.Style;
            if (style == ModuleAction.ActionStyleEnum.OuterWindow)
                if (!Manager.IsInPopup)
                    style = ModuleAction.ActionStyleEnum.Normal;

            if (style == ModuleAction.ActionStyleEnum.Popup || style == ModuleAction.ActionStyleEnum.PopupEdit)
                if (Manager.IsInPopup)
                    style = ModuleAction.ActionStyleEnum.NewWindow;

            if (style == ModuleAction.ActionStyleEnum.Popup || style == ModuleAction.ActionStyleEnum.PopupEdit || style == ModuleAction.ActionStyleEnum.ForcePopup)
                await YetaWFCoreRendering.Render.AddPopupsAddOnsAsync();

            bool newWindow = false, outerWindow = false;
            bool popup = false, popupEdit = false;
            bool nothing = false, post = false;

            switch (style) {
                default:
                case ModuleAction.ActionStyleEnum.Normal:
                    break;
                case ModuleAction.ActionStyleEnum.NewWindow:
                    newWindow = true;
                    break;
                case ModuleAction.ActionStyleEnum.Popup:
                case ModuleAction.ActionStyleEnum.ForcePopup:
                    popup = Manager.CurrentSite.AllowPopups;
                    break;
                case ModuleAction.ActionStyleEnum.PopupEdit:
                    popup = Manager.CurrentSite.AllowPopups;
                    popupEdit = Manager.CurrentSite.AllowPopups;
                    break;
                case ModuleAction.ActionStyleEnum.OuterWindow:
                    outerWindow = true;
                    break;
                case ModuleAction.ActionStyleEnum.Nothing:
                    nothing = true;
                    break;
                case ModuleAction.ActionStyleEnum.Post:
                    post = true;
                    break;
            }

            Dictionary<string, object> attrs = new Dictionary<string, object>();
            string css = null;
            if (!string.IsNullOrWhiteSpace(action.Tooltip))
                attrs.Add(Basics.CssTooltip, action.Tooltip);
            if (!string.IsNullOrWhiteSpace(action.Name))
                attrs.Add("data-name", action.Name);
            if (!action.Displayed)
                attrs.Add("style", "display:none");

            if (!string.IsNullOrWhiteSpace(action.CssClass))
                css = CssManager.CombineCss(css, Manager.AddOnManager.CheckInvokedCssModule(action.CssClass));

            string extraClass;
            switch (mode) {
                default:
                case ModuleAction.RenderModeEnum.Button: extraClass = "y_button y_act_button"; break;
                case ModuleAction.RenderModeEnum.ButtonIcon: extraClass = "y_button y_act_buttonicon"; break;
                case ModuleAction.RenderModeEnum.ButtonOnly: extraClass = "y_button y_act_buttononly"; break;
                case ModuleAction.RenderModeEnum.IconsOnly: extraClass = "y_act_icon"; break;
                case ModuleAction.RenderModeEnum.LinksOnly: extraClass = "y_act_link"; break;
                case ModuleAction.RenderModeEnum.NormalLinks: extraClass = "y_act_normlink"; break;
                case ModuleAction.RenderModeEnum.NormalMenu: extraClass = "y_act_normmenu"; break;
            }
            css = CssManager.CombineCss(css, Manager.AddOnManager.CheckInvokedCssModule(extraClass));

            string url = action.GetCompleteUrl(OnPage: true);
            if (!string.IsNullOrWhiteSpace(url)) {
                attrs.Add("href", url);
                if (Manager.CurrentPage != null) {
                    string currUrl = Manager.CurrentPage.EvaluatedCanonicalUrl;
                    if (!string.IsNullOrWhiteSpace(currUrl) && currUrl != "/") {// this doesn't work on home page because everything matches
                        if (action.Url == currUrl)
                            css = CssManager.CombineCss(css, "t_currenturl");
                        if (currUrl.StartsWith(action.Url))
                            css = CssManager.CombineCss(css, "t_currenturlpart");
                    }
                }
            } else
                attrs.Add("href", "javascript:void(0);");

            if (!string.IsNullOrWhiteSpace(action.ConfirmationText)) {
                if (action.Category == ModuleAction.ActionCategoryEnum.Delete) {
                    // confirm deletions?
                    if (UserSettings.GetProperty<bool>("ConfirmDelete"))
                        attrs.Add(Basics.CssConfirm, action.ConfirmationText);
                } else {
                    // confirm actions?
                    if (UserSettings.GetProperty<bool>("ConfirmActions"))
                        attrs.Add(Basics.CssConfirm, action.ConfirmationText);
                }
            }
            if (!string.IsNullOrWhiteSpace(action.PleaseWaitText))
                attrs.Add(Basics.CssPleaseWait, action.PleaseWaitText);
            if (action.CookieAsDoneSignal)
                attrs.Add(Basics.CookieDoneCssAttr, string.Empty);

            if (action.SaveReturnUrl) {
                attrs.Add(Basics.CssSaveReturnUrl, string.Empty);
                if (!action.AddToOriginList)
                    attrs.Add(Basics.CssDontAddToOriginList, string.Empty);
            }
            if (!string.IsNullOrWhiteSpace(action.ExtraData))
                attrs.Add(Basics.CssExtraData, action.ExtraData);

            if (action.NeedsModuleContext)
                attrs.Add(Basics.CssAddModuleContext, string.Empty);

            if (post)
                attrs.Add(Basics.PostAttr, string.Empty);

            if (action.DontFollow || action.CookieAsDoneSignal || post || nothing)
                attrs.Add("rel", "nofollow"); // this is so bots don't follow this assuming it's a simple page (Post actions can't be retrieved with GET/HEAD anyway)

            if (outerWindow)
                attrs.Add(Basics.CssOuterWindow, string.Empty);

            if (!nothing)
                css = CssManager.CombineCss(css, Manager.AddOnManager.CheckInvokedCssModule(Basics.CssActionLink));

            if (newWindow) {
                attrs.Add("target", "_blank");
                if (!attrs.ContainsKey("rel"))
                    attrs.Add("rel", "noopener noreferrer");
            }

            if (popup) {
                css = CssManager.CombineCss(css, Manager.AddOnManager.CheckInvokedCssModule(Basics.CssPopupLink));
                if (popupEdit)
                    attrs.Add(Basics.CssAttrDataSpecialEdit, string.Empty);
            }
            if (mode == ModuleAction.RenderModeEnum.Button || mode == ModuleAction.RenderModeEnum.ButtonIcon || mode == ModuleAction.RenderModeEnum.ButtonOnly)
                attrs.Add(Basics.CssAttrActionButton, string.Empty);

            bool hasText = false, hasImg = false;
            string innerHtml = string.Empty;
            if (mode != ModuleAction.RenderModeEnum.LinksOnly && mode != ModuleAction.RenderModeEnum.ButtonOnly && !string.IsNullOrWhiteSpace(action.ImageUrlFinal)) {
                string text = mode == ModuleAction.RenderModeEnum.NormalMenu ? action.MenuText : action.LinkText;
                innerHtml = ImageHTML.BuildKnownIcon(action.ImageUrlFinal, alt: text, cssClass: Basics.CssNoTooltip);
                hasImg = true;
            }

            if (mode != ModuleAction.RenderModeEnum.IconsOnly && mode != ModuleAction.RenderModeEnum.ButtonIcon) {
                string text = mode == ModuleAction.RenderModeEnum.NormalMenu ? action.MenuText : action.LinkText;
                if (!string.IsNullOrWhiteSpace(text)) {
                    innerHtml += Utility.HE(text);
                    hasText = true;
                }
            }

            if (hasText) {
                if (hasImg) {
                    css = CssManager.CombineCss(css, "y_act_textimg");
                } else {
                    css = CssManager.CombineCss(css, "y_act_text");
                }
            } else {
                if (hasImg) {
                    css = CssManager.CombineCss(css, "y_act_img");
                }
            }

            return $@"<a{(id != null ? $" id='{id}'" : null)} class='{Globals.CssModuleNoPrint}{HtmlBuilder.GetClasses(attrs, css)}'{HtmlBuilder.Attributes(attrs)}>{innerHtml}</a>";
        }
    }
}
