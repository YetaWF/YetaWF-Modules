/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System.Collections.Generic;
using YetaWF.Core.Models;

namespace YetaWF.Modules.Panels.Models {

    public class LocalPage {
        public string Url { get; set; }
        public bool Popup { get; set; }
    }

    public class PagePanelInfo {

        public class PanelEntry {

            public string Url { get; set; }
            public MultiString Caption { get; set; }
            public MultiString ToolTip { get; set; }
            public string ImageUrl { get; internal set; }
            public bool Popup { get; set; }

            public PanelEntry() {
                Caption = new MultiString();
                ToolTip = new MultiString();
            }
        }

        public PagePanelInfo() {
            Panels = new List<Models.PagePanelInfo.PanelEntry>();
        }

        public List<PanelEntry> Panels { get; set; }

        public bool UsePopups { get; internal set; }
        public Modules.PagePanelModule.PanelStyleEnum Style { get; set; }
    }
}
