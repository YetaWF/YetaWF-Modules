/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;

namespace YetaWF.Modules.Blog.Support {

    public class Resources { } // class holding all localization resources

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ShortNameValidationAttribute : RegexValidationBaseAttribute {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(Resources), name, defaultValue, parms); }

        public ShortNameValidationAttribute() : base(@"^\s*[A-Za-z0-9_\-]*\s*$",
                __ResStr("valShortName", "A shortname must consist of letters, numbers, underscores or dashes, without spaces"),
                __ResStr("valShortName2", "The shortname is invalid ('{0}' property) - it must consist of letters, numbers, underscores or dashes, without spaces"),
                __ResStr("valShortName3", "The shortname '{0}' is invalid - it must consist of letters, numbers, underscores or dashes, without spaces")
            ) { }
    }
}
