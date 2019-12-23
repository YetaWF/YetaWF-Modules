/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;

namespace Softelvdm.Modules.IVR.Models.Attributes {

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExtensionValidationAttribute : RegexValidationBaseAttribute {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ExtensionValidationAttribute), name, defaultValue, parms); }

        public ExtensionValidationAttribute() : base(@"^\s*[0-9]{1,6}\s*$",
                __ResStr("valExtension", "The extension is invalid"),
                __ResStr("valExtension2", "The extension (field '{0}') is invalid"),
                __ResStr("valExtension3", "The extension '{0}' is invalid")
            ) { }
    }
}
