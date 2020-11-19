/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Panels.Models;

namespace YetaWF.Modules.Panels.Components {

    public abstract class SplitterInfoComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(SplitterInfoComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "SplitterInfo";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// This component is used by the YetaWF.Panels package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class SplitterInfoDisplayComponent : SplitterInfoComponentBase, IYetaWFComponent<SplitterInfo> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public class Setup {
            public string ContentId { get; set; }
            public int Height { get; set; }// 0 (auto-fill), or pixels
            public int MinWidth { get; set; }// pixels
            public int Width { get; set; }// percentage
        }

        public async Task<string> RenderAsync(SplitterInfo model) {

            HtmlBuilder hbLeft = new HtmlBuilder();

            string titleText = model.TitleText;
            string titleTooltip = model.TitleTooltip;

            if (await model.IsAuthorizedLeftAsync()) {
                ModuleDefinition mod = await model.GetModuleLeftAsync();
                if (mod != null) {
                    hbLeft.Append(await mod.RenderModuleViewAsync(HtmlHelper));
                    // get title & text from module
                    try { titleText = ((dynamic)mod).TitleText; } catch (Exception) { }
                    try { titleTooltip = ((dynamic)mod).TitleTooltip; } catch (Exception) { }
                } else {
                    hbLeft.Append($@"<div>{__ResStr("noModule", "(no module defined)")}</div>");
                }
            }

            HtmlBuilder hbRight = new HtmlBuilder();
            if (await model.IsAuthorizedRightAsync()) {
                ModuleDefinition mod = await model.GetModuleRightAsync();
                if (mod != null) {
                    hbRight.Append(await mod.RenderModuleViewAsync(HtmlHelper));
                } else {
                    hbRight.Append($@"<div>{__ResStr("noModule", "(no module defined)")}</div>");
                }
            }

            string heightStyle = null;
            if (Manager.EditMode)
                heightStyle = $" style='height:300px'";
            else if (model.Height > 0)
                heightStyle = $" style='height:{model.Height}px'";

            string leftStyle = $" style='flex-basis:{model.Width}%;min-width:{model.MinWidth}px'";

            string contentId = Manager.UniqueId();

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"
<div class='yt_panels_splitterinfo t_display t_expanded' id='{ControlId}'{heightStyle}>
    <div class='yt_panels_splitterinfo_left d-print-none yNoPrint'{leftStyle}>
        <div class='t_area'>");

            if (!string.IsNullOrWhiteSpace(titleText)) {
                hb.Append($@"
            <div class='yt_panels_splitterinfo_title' {Basics.CssTooltip}='{HAE(titleTooltip)}'>
                {HE(titleText)}
            </div>");
            }

            hb.Append($@"
            <div class='yt_panels_splitterinfo_cmds'>
                <div class='yt_panels_splitterinfo_colldesc'>{HE(model.CollapseText)}</div><div class='yt_panels_splitterinfo_coll' {Basics.CssTooltip}='{HAE(model.CollapseToolTip)}'></div>
            </div>
            {hbLeft.ToString()}
        </div>
    </div>
    <div class='yt_panels_splitterinfo_right'>
        <div class='t_area' id='{contentId}'>
            {hbRight.ToString()}
        </div>
        <div class='yt_panels_splitterinfo_resize d-print-none yNoPrint'></div>
    </div>
    <div class='yt_panels_splitterinfo_exp d-print-none yNoPrint' {Basics.CssTooltip}='{HAE(model.ExpandToolTip)}'></div>
</div>");

            Setup setup = new Setup {
                ContentId = contentId,
                Height = Manager.EditMode ? 300 : model.Height,
                MinWidth = model.MinWidth,
                Width = model.Width,
            };

            Manager.ScriptManager.AddLast($@"new YetaWF_Panels.SplitterInfoComponent('{ControlId}', {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }
    }
}
