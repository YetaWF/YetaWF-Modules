using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
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

            string menuContents = (await moduleMenu.RenderAsync(null, null, Globals.CssModuleLinks)).ToString();
            if (!string.IsNullOrWhiteSpace(menuContents)) {

                await Manager.AddOnManager.AddTemplateAsync(Package.Domain, Package.Product, "ActionIcons"); // action icons

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

            string menuContents = (await moduleMenu.RenderAsync(null, null, Globals.CssModuleMenu)).ToString();
            if (!string.IsNullOrWhiteSpace(menuContents)) {

                //await Manager.ScriptManager.AddKendoUICoreJsFile("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
                await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.menu.min.js");

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

    }
}
