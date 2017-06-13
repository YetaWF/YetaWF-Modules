/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Licensing */

using System;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;

namespace YetaWF.Modules.TinyLogin.Support {

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LogoffUrlValidationAttribute : RegexValidationBaseAttribute {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(LogoffUrlValidationAttribute), name, defaultValue, parms); }

        public LogoffUrlValidationAttribute() : base(@"^\s*\/.*\s*$",
            __ResStr("valLogoff", "The logoff Url is invalid - It must start with a / character (a direct Url on this site)"),
            __ResStr("valLogoff2", "The logoff Url is invalid ('{0}' property) - It must start with a / character (a direct Url on this site)"),
            __ResStr("valLogoff3", "The logoff Url '{0}' is invalid - It must start with a / character (a direct Url on this site)")
            ) { }
    }
}
