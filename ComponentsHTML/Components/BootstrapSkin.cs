﻿/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class BootstrapSkinComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(BootstrapSkinComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "BootstrapSkin";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    public class BootstrapSkinDisplayComponent : BootstrapSkinComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(string model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (string.IsNullOrWhiteSpace(model))
                return Task.FromResult(new YHtmlString());

            hb.Append($@"<div class='yt_bootstrapskin t_display'>
                {HE(model)}
            </div>");

            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public class BootstrapSkinEditComponent : BootstrapSkinComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(string model) {

            // get all available skins
            SkinAccess skinAccess = new SkinAccess();
            List<SelectionItem<string>> list = (from theme in await skinAccess.GetBootstrapThemeListAsync() select new SelectionItem<string>() {
                Text = theme.Name,
                Tooltip = theme.Description,
                Value = theme.Name,
            }).ToList();

            bool useDefault = !PropData.GetAdditionalAttributeValue<bool>("NoDefault");
            if (useDefault)
                list.Insert(0, new SelectionItem<string> {
                    Text = __ResStr("default", "(Site Default)"),
                    Tooltip = __ResStr("defaultTT", "Use the site defined default theme"),
                    Value = "",
                });
            else if (model == null)
                model = await SkinAccess.GetBootstrapDefaultSkinAsync();

            // display the skins in a drop down
            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_bootstrapskin");
        }
    }
}
