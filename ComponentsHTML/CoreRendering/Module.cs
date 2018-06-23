/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using static YetaWF.Core.Modules.ModuleAction;
#if MVC6
#else
#endif

namespace YetaWF.Modules.ComponentsHTML {

    public partial class CoreRendering {

        protected static YetaWFManager Manager { get { return YetaWFManager.Manager; } }
        protected static Package Package { get { return Controllers.AreaRegistration.CurrentPackage; } }

        /// <summary>
        /// Render module links.
        /// </summary>
        public async Task<YHtmlString> RenderModuleLinksAsync(ModuleDefinition mod, ModuleAction.RenderModeEnum renderMode, string cssClass) {

            HtmlBuilder hb = new HtmlBuilder();

            MenuList moduleMenu = await mod.GetModuleMenuListAsync(renderMode, ModuleAction.ActionLocationEnum.ModuleLinks);

            string menuContents = (await RenderMenuAsync(moduleMenu, null, Globals.CssModuleLinks)).ToString();
            if (!string.IsNullOrWhiteSpace(menuContents)) {

                await Manager.AddOnManager.AddTemplateFromUIHintAsync("ActionIcons"); // action icons

                // <div>
                YTagBuilder div2Tag = new YTagBuilder("div");
                div2Tag.AddCssClass(Manager.AddOnManager.CheckInvokedCssModule(cssClass));
                hb.Append(div2Tag.ToString(YTagRenderMode.StartTag));

                // <ul><li> menu
                hb.Append(menuContents);

                // </div>
                hb.Append(div2Tag.ToString(YTagRenderMode.EndTag));
            }
            return hb.ToYHtmlString();
        }

        /// <summary>
        /// Render a module menu.
        /// </summary>
        public async Task<YHtmlString> RenderModuleMenuAsync(ModuleDefinition mod) {

            HtmlBuilder hb = new HtmlBuilder();

            MenuList moduleMenu = await mod.GetModuleMenuListAsync(ModuleAction.RenderModeEnum.NormalMenu, ModuleAction.ActionLocationEnum.ModuleMenu);

            string menuContents = (await RenderMenuAsync(moduleMenu, null, Globals.CssModuleMenu)).ToString();
            if (!string.IsNullOrWhiteSpace(menuContents)) {

                //await Manager.ScriptManager.AddKendoUICoreJsFile("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
                await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.menu.min.js");

                //$$$ That's not good here
                await Manager.AddOnManager.AddAddOnNamedAsync(Package.Domain, Package.Product, "ModuleMenu"); // module menu support
                await Manager.AddOnManager.AddAddOnNamedAsync(Package.Domain, Package.Product, "Modules");// various module support
                await Manager.AddOnManager.AddAddOnGlobalAsync("jquery.com", "jquery-color");// for color change when entering module edit menu

                // <div class= >
                YTagBuilder divTag = new YTagBuilder("div");
                divTag.AddCssClass(Manager.AddOnManager.CheckInvokedCssModule(Globals.CssModuleMenuEditIcon));
                divTag.Attributes.Add("style", "display:none");
                hb.Append(divTag.ToString(YTagRenderMode.StartTag));

                SkinImages skinImages = new SkinImages();
                string imageUrl = await skinImages.FindIcon_PackageAsync("#ModuleMenu", Package.GetCurrentPackage(this));
                YTagBuilder tagImg = ImageHTML.BuildKnownImageYTag(imageUrl, alt: this.__ResStr("mmAlt", "Menu"));
                hb.Append(tagImg.ToString(YTagRenderMode.StartTag));

                // <div>
                YTagBuilder div2Tag = new YTagBuilder("div");
                div2Tag.AddCssClass(Manager.AddOnManager.CheckInvokedCssModule(Globals.CssModuleMenuContainer));
                hb.Append(div2Tag.ToString(YTagRenderMode.StartTag));

                // <ul><li> menu
                hb.Append(menuContents);

                // </div>
                hb.Append(div2Tag.ToString(YTagRenderMode.EndTag));

                // </div>
                hb.Append(divTag.ToString(YTagRenderMode.EndTag));
            }
            return hb.ToYHtmlString();
        }

        public async Task<YHtmlString> RenderModuleActionAsync(ModuleAction action, RenderModeEnum mode, string id) {
            return await RenderActionAsync(action, mode, id);
        }

        /// <summary>
        /// Render a module action.
        /// </summary>
        public static async Task<YHtmlString> RenderActionAsync(ModuleAction action, RenderModeEnum mode, string id,
                RenderEngineEnum RenderEngine = RenderEngineEnum.JqueryMenu, int BootstrapSmartMenuLevel = 0, bool HasSubmenu = false) {

            // check if we're in the right mode
            if (!await action.RendersSomethingAsync()) return new YHtmlString("");

            await Manager.AddOnManager.AddTemplateFromUIHintAsync("ActionIcons");// this is needed because we're not used by templates

            if (!string.IsNullOrWhiteSpace(action.ConfirmationText) && (action.Style != ActionStyleEnum.Post && action.Style != ActionStyleEnum.Nothing))
                throw new InternalError("When using ConfirmationText, the Style property must be set to Post");
            if (!string.IsNullOrWhiteSpace(action.PleaseWaitText) && (action.Style != ActionStyleEnum.Normal && action.Style != ActionStyleEnum.Post))
                throw new InternalError("When using PleaseWaitText, the Style property must be set to Normal or Post");
            if (action.CookieAsDoneSignal && action.Style != ActionStyleEnum.Normal)
                throw new InternalError("When using CookieAsDoneSignal, the Style property must be set to Normal");

            ActionStyleEnum style = action.Style;
            if (style == ActionStyleEnum.OuterWindow)
                if (!Manager.IsInPopup)
                    style = ActionStyleEnum.Normal;

            if (style == ActionStyleEnum.Popup || style == ActionStyleEnum.PopupEdit)
                if (Manager.IsInPopup)
                    style = ActionStyleEnum.NewWindow;

            if (style == ActionStyleEnum.Popup || style == ActionStyleEnum.PopupEdit || style == ActionStyleEnum.ForcePopup)
                await Manager.AddOnManager.AddAddOnNamedAsync("YetaWF", "Core", "Popups");// this is needed for popup support //$$$$$this probably needs to move/rename

            bool newWindow = false, outerWindow = false;
            bool popup = false, popupEdit = false;
            bool nothing = false, post = false;

            switch (style) {
                default:
                case ActionStyleEnum.Normal:
                    break;
                case ActionStyleEnum.NewWindow:
                    newWindow = true;
                    break;
                case ActionStyleEnum.Popup:
                case ActionStyleEnum.ForcePopup:
                    popup = Manager.CurrentSite.AllowPopups;
                    break;
                case ActionStyleEnum.PopupEdit:
                    popup = Manager.CurrentSite.AllowPopups;
                    popupEdit = Manager.CurrentSite.AllowPopups;
                    break;
                case ActionStyleEnum.OuterWindow:
                    outerWindow = true;
                    break;
                case ActionStyleEnum.Nothing:
                    nothing = true;
                    break;
                case ActionStyleEnum.Post:
                    post = true;
                    break;
            }

            YTagBuilder tag = new YTagBuilder("a");
            if (!string.IsNullOrWhiteSpace(action.Tooltip))
                tag.MergeAttribute(Basics.CssTooltip, action.Tooltip);
            if (!string.IsNullOrWhiteSpace(action.Name))
                tag.MergeAttribute("data-name", action.Name);
            if (!action.Displayed)
                tag.MergeAttribute("style", "display:none");
            if (HasSubmenu) {
                if (RenderEngine == RenderEngineEnum.BootstrapSmartMenu) {
                    tag.AddCssClass("dropdown-toggle");
                    tag.Attributes.Add("data-toggle", "dropdown-toggle");
                }
                tag.Attributes.Add("aria-haspopup", "true");
                tag.Attributes.Add("aria-expanded", "false");
            }
            if (RenderEngine == ModuleAction.RenderEngineEnum.BootstrapSmartMenu)
                tag.AddCssClass(BootstrapSmartMenuLevel <= 1 ? "nav-link" : "dropdown-item");

            if (!string.IsNullOrWhiteSpace(id))
                tag.Attributes.Add("id", id);

            if (!string.IsNullOrWhiteSpace(action.CssClass))
                tag.AddCssClass(Manager.AddOnManager.CheckInvokedCssModule(action.CssClass));
            string extraClass;
            switch (mode) {
                default:
                case RenderModeEnum.Button: extraClass = "y_act_button"; break;
                case RenderModeEnum.ButtonIcon: extraClass = "y_act_buttonicon"; break;
                case RenderModeEnum.IconsOnly: extraClass = "y_act_icon"; break;
                case RenderModeEnum.LinksOnly: extraClass = "y_act_link"; break;
                case RenderModeEnum.NormalLinks: extraClass = "y_act_normlink"; break;
                case RenderModeEnum.NormalMenu: extraClass = "y_act_normmenu"; break;
            }
            tag.AddCssClass(Manager.AddOnManager.CheckInvokedCssModule(extraClass));

            string url = action.GetCompleteUrl(OnPage: true);
            if (!string.IsNullOrWhiteSpace(url)) {
                tag.MergeAttribute("href", YetaWFManager.UrlEncodePath(url));
                if (Manager.CurrentPage != null) {
                    string currUrl = Manager.CurrentPage.EvaluatedCanonicalUrl;
                    if (!string.IsNullOrWhiteSpace(currUrl) && currUrl != "/") {// this doesn't work on home page because everything matches
                        if (action.Url == currUrl)
                            tag.AddCssClass("t_currenturl");
                        if (currUrl.StartsWith(action.Url))
                            tag.AddCssClass("t_currenturlpart");
                    }
                }
            } else
                tag.MergeAttribute("href", "javascript:void(0);");

            if (!string.IsNullOrWhiteSpace(action.ConfirmationText)) {
                if (action.Category == ActionCategoryEnum.Delete) {
                    // confirm deletions?
                    if (UserSettings.GetProperty<bool>("ConfirmDelete"))
                        tag.MergeAttribute(Basics.CssConfirm, action.ConfirmationText);
                } else {
                    // confirm actions?
                    if (UserSettings.GetProperty<bool>("ConfirmActions"))
                        tag.MergeAttribute(Basics.CssConfirm, action.ConfirmationText);
                }
            }
            if (!string.IsNullOrWhiteSpace(action.PleaseWaitText))
                tag.MergeAttribute(Basics.CssPleaseWait, action.PleaseWaitText);
            if (action.CookieAsDoneSignal)
                tag.Attributes.Add(Basics.CookieDoneCssAttr, "");
            if (action.SaveReturnUrl) {
                tag.Attributes.Add(Basics.CssSaveReturnUrl, "");
                if (!action.AddToOriginList)
                    tag.Attributes.Add(Basics.CssDontAddToOriginList, "");
            }
            if (!string.IsNullOrWhiteSpace(action.ExtraData))
                tag.Attributes.Add(Basics.CssExtraData, action.ExtraData);
            if (action.NeedsModuleContext)
                tag.Attributes.Add(Basics.CssAddModuleContext, "");

            if (post)
                tag.Attributes.Add(Basics.PostAttr, "");
            if (action.DontFollow || action.CookieAsDoneSignal || post || nothing) {
                if (!newWindow)
                    tag.Attributes.Add("rel", "nofollow"); // this is so bots don't follow this assuming it's a simple page (Post actions can't be retrieved with GET/HEAD anyway)
            }
            if (outerWindow)
                tag.Attributes.Add(Basics.CssOuterWindow, "");
            if (!nothing)
                tag.AddCssClass(Manager.AddOnManager.CheckInvokedCssModule(Basics.CssActionLink));
            if (newWindow) {
                tag.MergeAttribute("target", "_blank");
                tag.MergeAttribute("rel", "noopener noreferrer");
            }
            if (popup) {
                tag.AddCssClass(Manager.AddOnManager.CheckInvokedCssModule(Basics.CssPopupLink));
                if (popupEdit)
                    tag.Attributes.Add(Basics.CssAttrDataSpecialEdit, "");
            }
            if (mode == RenderModeEnum.Button || mode == RenderModeEnum.ButtonIcon)
                tag.Attributes.Add(Basics.CssAttrActionButton, "");

            bool hasText = false, hasImg = false;
            string innerHtml = "";
            if (mode != RenderModeEnum.LinksOnly && !string.IsNullOrWhiteSpace(action.ImageUrlFinal)) {
                YTagBuilder tagImg = ImageHTML.BuildKnownImageYTag(await action.GetImageUrlFinalAsync(), alt: mode == RenderModeEnum.NormalMenu ? action.MenuText : action.LinkText);
                tagImg.AddCssClass(Basics.CssNoTooltip);
                innerHtml += tagImg.ToString(YTagRenderMode.StartTag);
                hasImg = true;
            }
            if (mode != RenderModeEnum.IconsOnly && mode != RenderModeEnum.ButtonIcon) {
                string text = mode == RenderModeEnum.NormalMenu ? action.MenuText : action.LinkText;
                if (!string.IsNullOrWhiteSpace(text)) {
                    innerHtml += YetaWFManager.HtmlEncode(text);
                    hasText = true;
                }
            }
            if (hasText) {
                if (hasImg) {
                    tag.AddCssClass("y_act_textimg");
                } else {
                    tag.AddCssClass("y_act_text");
                }
            } else {
                if (hasImg) {
                    tag.AddCssClass("y_act_img");
                }
            }
            if (HasSubmenu && RenderEngine == RenderEngineEnum.BootstrapSmartMenu)
                innerHtml += " <span class='caret'></span>";

            tag.AddCssClass(Globals.CssModuleNoPrint);
            tag.InnerHtml = innerHtml;

            return tag.ToYHtmlString(YTagRenderMode.Normal);
        }
    }
}
