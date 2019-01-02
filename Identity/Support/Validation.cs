/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;

namespace YetaWF.Modules.Identity.Support {

    public class Resources { } // class holding all localization resources

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RoleNameValidationAttribute : RegexValidationBaseAttribute {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(Resources), name, defaultValue, parms); }

        public RoleNameValidationAttribute() : base(@"^\s*[A-Za-z0-9_][A-Za-z0-9_\.]*\s*$",
                __ResStr("valRoleName", "The role name must consist of one word containing letters and numbers, underscores and periods, without spaces"),
                __ResStr("valRoleName2", "The role name is invalid ('{0}' property) - It must consist of one word containing letters and numbers, underscores and periods, without spaces"),
                __ResStr("valRoleName3", "The role name '{0}' is invalid - It must consist of one word containing letters and numbers, underscores and periods, without spaces")
            ) { }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ResourceNameValidationAttribute : RegexValidationBaseAttribute {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(Resources), name, defaultValue, parms); }

        public ResourceNameValidationAttribute() : base(@"^\s*[A-Za-z][A-Za-z0-9]*_[A-Za-z][A-Za-z0-9]*\-.*\s*$",
                __ResStr("valResourceName", "The resource name must follow this naming convention: AreaName-ResourceName - An example of a valid resource name is \"YetaWF_Identity-Authorization Admin (Display)\""),
                __ResStr("valResourceName2", "The resource name is invalid ('{0}' property) - It must follow this naming convention: AreaName-ResourceName - An example of a valid resource name is \"YetaWF_Identity-Authorization Admin (Display)\""),
                __ResStr("valResourceName3", "The resource name '{0}' is invalid - It must follow this naming convention: AreaName-ResourceName - An example of a valid resource name is \"YetaWF_Identity-Authorization Admin (Display)\"")
            ) { }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UserNameValidationAttribute : RegexValidationBaseAttribute {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(Resources), name, defaultValue, parms); }

        public UserNameValidationAttribute() : base(@"^\s*\S.*\s*$",
                __ResStr("valuserName", "The user name can't start with characters like a tab or a space"),
                __ResStr("valuserName2", "The user name is invalid ('{0}' property) - The user name can't start with characters like a tab or a space"),
                __ResStr("valuserName3", "The user name '{0}' is invalid - The user name can't start with characters like a tab or a space")
            ) { }
    }
}