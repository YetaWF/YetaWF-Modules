/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using YetaWF.Core.Support;

namespace YetaWF.Modules.Panels.DataProvider {

    public static class PageBarDataProvider {

        private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        public static bool GetExpanded() {
            return Manager.SessionSettings.SiteSettings.GetValue<bool>("YetaWF_Panels_Expanded", true);
        }

        public static void SaveExpanded(bool expanded) {
            if (expanded)
                Manager.SessionSettings.SiteSettings.Remove("YetaWF_Panels_Expanded");
            else
                Manager.SessionSettings.SiteSettings.SetValue("YetaWF_Panels_Expanded", expanded);
            Manager.SessionSettings.SiteSettings.Save();
        }
    }
}