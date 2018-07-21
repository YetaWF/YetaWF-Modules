/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF {
    export interface IVolatile {
        YetaWF_ComponentsHTML: YetaWF_ComponentsHTML.IPackageVolatiles;
    }
}
namespace YetaWF_ComponentsHTML {
    export interface IPackageVolatiles {

    }
}

interface Event {
    __YetaWF: boolean; // we add this to a jquery Event to signal that we already translated it to native
}

namespace YetaWF_ComponentsHTML {

    export class ComponentsHTML {

    }
}

var ComponentsHTML = new YetaWF_ComponentsHTML.ComponentsHTML();