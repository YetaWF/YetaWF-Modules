/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

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

            // icons used: fas-expand-arrows-alt, fas-compress-arrows-alt
            hb.Append($@"
            <div class='yt_panels_splitterinfo_cmds'>
                <div class='yt_panels_splitterinfo_colldesc'>
                    {HE(model.CollapseText)}
                </div>
                <div class='yt_panels_splitterinfo_coll' {Basics.CssTooltip}='{HAE(model.CollapseToolTip)}'>
                    <svg aria-hidden='true' focusable='false' role='img' viewBox='0 0 512 512'>
                        <path fill='currentColor' d='M200 288H88c-21.4 0-32.1 25.8-17 41l32.9 31-99.2 99.3c-6.2 6.2-6.2 16.4 0 22.6l25.4 25.4c6.2 6.2 16.4 6.2 22.6 0L152 408l31.1 33c15.1 15.1 40.9 4.4 40.9-17V312c0-13.3-10.7-24-24-24zm112-64h112c21.4 0 32.1-25.9 17-41l-33-31 99.3-99.3c6.2-6.2 6.2-16.4 0-22.6L481.9 4.7c-6.2-6.2-16.4-6.2-22.6 0L360 104l-31.1-33C313.8 55.9 288 66.6 288 88v112c0 13.3 10.7 24 24 24zm96 136l33-31.1c15.1-15.1 4.4-40.9-17-40.9H312c-13.3 0-24 10.7-24 24v112c0 21.4 25.9 32.1 41 17l31-32.9 99.3 99.3c6.2 6.2 16.4 6.2 22.6 0l25.4-25.4c6.2-6.2 6.2-16.4 0-22.6L408 360zM183 71.1L152 104 52.7 4.7c-6.2-6.2-16.4-6.2-22.6 0L4.7 30.1c-6.2 6.2-6.2 16.4 0 22.6L104 152l-33 31.1C55.9 198.2 66.6 224 88 224h112c13.3 0 24-10.7 24-24V88c0-21.3-25.9-32-41-16.9z'></path>
                    </svg>
                </div>
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
    <div class='yt_panels_splitterinfo_exp d-print-none yNoPrint' {Basics.CssTooltip}='{HAE(model.ExpandToolTip)}'>
        <svg aria-hidden='true' focusable='false' role='img' viewBox='0 0 448 512'>
            <path fill='currentColor' d='M448 344v112a23.94 23.94 0 0 1-24 24H312c-21.39 0-32.09-25.9-17-41l36.2-36.2L224 295.6 116.77 402.9 153 439c15.09 15.1 4.39 41-17 41H24a23.94 23.94 0 0 1-24-24V344c0-21.4 25.89-32.1 41-17l36.19 36.2L184.46 256 77.18 148.7 41 185c-15.1 15.1-41 4.4-41-17V56a23.94 23.94 0 0 1 24-24h112c21.39 0 32.09 25.9 17 41l-36.2 36.2L224 216.4l107.23-107.3L295 73c-15.09-15.1-4.39-41 17-41h112a23.94 23.94 0 0 1 24 24v112c0 21.4-25.89 32.1-41 17l-36.19-36.2L263.54 256l107.28 107.3L407 327.1c15.1-15.2 41-4.5 41 16.9z'></path>
        </svg>
    </div>
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
