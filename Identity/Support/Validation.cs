/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;

namespace YetaWF.Modules.Identity.Support {

    public class Resources { } // class holding all localization resources

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RoleNameValidationAttribute : RegexValidationBaseAttribute {

        [CombinedResources]
        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(Resources), name, defaultValue, parms); }

        public RoleNameValidationAttribute() : base("[A-Za-z0-9_][A-Za-z0-9_\\.]*",
                __ResStr("valRoleName", "The role name must consist of one word containing letters and numbers, underscores and periods, without spaces"),
                __ResStr("valShortName2", "The role name is invalid ('{0}' property) - It must consist of one word containing letters and numbers, underscores and periods, without spaces"),
                __ResStr("valShortName3", "The role name '{0}' is invalid - It must consist of one word containing letters and numbers, underscores and periods, without spaces")
            ) { }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ResourceNameValidationAttribute : RegexValidationBaseAttribute {

        [CombinedResources]
        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(Resources), name, defaultValue, parms); }

        public ResourceNameValidationAttribute() : base(@"[A-Za-z][A-Za-z0-9]*_[A-Za-z][A-Za-z0-9]*\-.*",
                __ResStr("valRoleName", "The resource name must follow this naming convention: AreaName-ResourceName - An example of a valid resource name is \"YetaWF_Identity-Authorization Admin (Display)\""),
                __ResStr("valShortName2", "The resource name is invalid ('{0}' property) - It must follow this naming convention: AreaName-ResourceName - An example of a valid resource name is \"YetaWF_Identity-Authorization Admin (Display)\""),
                __ResStr("valShortName3", "The resource name '{0}' is invalid - It must follow this naming convention: AreaName-ResourceName - An example of a valid resource name is \"YetaWF_Identity-Authorization Admin (Display)\"")
            ) { }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UserNameValidationAttribute : RegexValidationBaseAttribute {

        [CombinedResources]
        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(Resources), name, defaultValue, parms); }

        public UserNameValidationAttribute() : base(@"\S.*",
                __ResStr("valuserName", "The user name can't start with characters like a tab or a space"),
                __ResStr("valShortName2", "The user name is invalid ('{0}' property) - The user name can't start with characters like a tab or a space"),
                __ResStr("valShortName3", "The user name '{0}' is invalid - The user name can't start with characters like a tab or a space")
            ) { }
    }
}