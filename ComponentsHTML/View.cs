/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using YetaWF.Core.Components;
using YetaWF.Core.Support;
#if MVC6
#else
#endif

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class YetaWFView : YetaWFViewBase {

        protected class JSDocumentReady : IDisposable {

            public JSDocumentReady(HtmlBuilder hb) {
                this.HB = hb;
                DisposableTracker.AddObject(this);
            }
            public void Dispose() { Dispose(true); }
            protected virtual void Dispose(bool disposing) {
                if (disposing) DisposableTracker.RemoveObject(this);
                while (CloseParen > 0) {
                    HB.Append("}");
                    CloseParen = CloseParen - 1;
                }
                HB.Append("}});");
            }
            //~JSDocumentReady() { Dispose(false); }
            public HtmlBuilder HB { get; set; }
            public int CloseParen { get; internal set; }
        }
        protected JSDocumentReady DocumentReady(HtmlBuilder hb, string id) {
            hb.Append($@"YetaWF_Basics.whenReadyOnce.push({{callback: function ($tag) {{ if ($tag.has('#{id}').length > 0) {{");
            return new JSDocumentReady(hb) { CloseParen = 1 };
        }
        protected JSDocumentReady DocumentReady(HtmlBuilder hb) {
            hb.Append("YetaWF_Basics.whenReadyOnce.push({callback: function ($tag) {\n");
            return new JSDocumentReady(hb);
        }
    }
}
