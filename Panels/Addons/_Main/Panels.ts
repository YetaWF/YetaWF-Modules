/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

namespace YetaWF {
    export interface ILocs {
        YetaWF_Panels: YetaWF_Panels.IPackageLocs;
    }
    export interface IConfigs {
        YetaWF_Panels: YetaWF_Panels.IPackageConfigs;
    }
}

namespace YetaWF_Panels {

    export interface IPackageLocs {
        RemoveConfirm: string;
        RemoveTitle: string;
    }
    export interface IPackageConfigs {
        Action_Apply: string;
        Action_MoveLeft: string;
        Action_MoveRight: string;
        Action_Remove: string;
        Action_Insert: string;
        Action_Add: string;
    }
}
