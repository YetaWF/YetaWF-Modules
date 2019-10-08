/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.Models;
using YetaWF.Core.Pages;

namespace YetaWF.Modules.Panels.Models {

    public class LocalPage {
        public string Url { get; set; }
        public bool Popup { get; set; }
    }

    public class PanelInfoBase {

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

        public PanelInfoBase() {
            Panels = new List<Models.PagePanelInfo.PanelEntry>();
        }

        public List<PanelEntry> Panels { get; set; }

        public bool UsePopups { get; internal set; }
    }

    public class PagePanelInfo : PanelInfoBase {
        public Modules.PagePanelModule.PanelStyleEnum Style { get; set; }
    }
    public class PageBarInfo : PanelInfoBase {
        public Modules.PageBarModule.PanelStyleEnum Style { get; set; }
        public bool UseSkinFormatting { get; set; }
        public string ContentPane { get; set; }
        public PageDefinition ContentPage { get; set; }
        public Uri ContentUri { get; set; }
    }
}