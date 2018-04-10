/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Models;
using YetaWF.Core.Serializers;

namespace YetaWF.Modules.Panels.Models {

    public class PagePanelInfo {

        public class PanelEntry {

            public string Url { get; set; }
            public MultiString Caption { get; set; }
            public MultiString ToolTip { get; set; }
            public string ImageUrl { get; internal set; }

            public PanelEntry() {
                Caption = new MultiString();
                ToolTip = new MultiString();
            }
        }

        public PagePanelInfo() {
            Panels = new SerializableList<Models.PagePanelInfo.PanelEntry>();
        }

        [Data_Binary]
        public SerializableList<PanelEntry> Panels { get; set; }

        public bool UsePopups { get; internal set; }
        public Modules.PagePanelModule.PanelStyleEnum Style { get; set; }
    }
}
