/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Licensing */

using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.SyntaxHighlighter.Controllers;

namespace YetaWF.Modules.SyntaxHighlighter.Addons {

    public class SkinSyntaxHighlighter : IAddOnSupport {

        public void AddSupport(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string area = AreaRegistration.CurrentPackage.AreaName;

            // Syntax highlighter
            scripts.AddLocalization(area, "msg_expandSource", this.__ResStr("expandSource", "+ expand source"));
            scripts.AddLocalization(area, "msg_help", this.__ResStr("help", "?"));
            scripts.AddLocalization(area, "msg_alert", this.__ResStr("alert", "SyntaxHighlighter\n\n"));
            scripts.AddLocalization(area, "msg_noBrush", this.__ResStr("noBrush", "Can't find brush for "));
            scripts.AddLocalization(area, "msg_brushNotHtmlScript", this.__ResStr("brushNotHtmlScript", "Brush wasn't made for html-script option "));
            scripts.AddLocalization(area, "msg_viewSource", this.__ResStr("viewSource", "View Source"));
            scripts.AddLocalization(area, "msg_copyToClipboard", this.__ResStr("copyToClipboard", "Copy to Clipboard"));
            scripts.AddLocalization(area, "msg_copyToClipboardConfirmation", this.__ResStr("copyToClipboardConfirmation", "The code has been copied to your clipboard."));
            scripts.AddLocalization(area, "msg_print", this.__ResStr("print", "Print"));
        }
    }
}
