/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.ComponentModel.DataAnnotations;
using YetaWF.Core.Localize;

namespace YetaWF.Modules.Identity.Support {

    public class Resources { } // class holding all localization resources

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RoleNameValidationAttribute : YetaWF.Core.Models.Attributes.RegularExpressionAttribute {

        [CombinedResources]
        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(Resources), name, defaultValue, parms); }

        public RoleNameValidationAttribute()
            : base("[A-Za-z0-9_][A-Za-z0-9_\\.]*") {
            ErrorMessage = __ResStr("valRoleName", "The role name must consist of one word containing letters and numbers, underscores and periods, without spaces.");
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            string roleName = ((string) value).Trim();
            return base.IsValid(roleName, validationContext);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ResourceNameValidationAttribute : YetaWF.Core.Models.Attributes.RegularExpressionAttribute {

        [CombinedResources]
        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(Resources), name, defaultValue, parms); }

        public ResourceNameValidationAttribute()
            : base(@"[A-Za-z][A-Za-z0-9]*_[A-Za-z][A-Za-z0-9]*\-.*") {
                ErrorMessage = __ResStr("valResourceName", "The resource name must follow this naming convention: AreaName-ResourceName. An example of a valid resource name is \"YetaWF_Identity-Authorization Admin (Display)\".");
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            string resourceName = ((string) value).Trim();
            return base.IsValid(resourceName, validationContext);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UserNameValidationAttribute : YetaWF.Core.Models.Attributes.RegularExpressionAttribute {

        [CombinedResources]
        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(Resources), name, defaultValue, parms); }

        public UserNameValidationAttribute()
            : base(@"\S.*") { // a non-whitespace character followed by anything
            ErrorMessage = __ResStr("valuserName", "The user name can't start with characters like a tab or a space.");
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            if (value == null) return ValidationResult.Success;
            string userName = ((string) value).Trim();
            return base.IsValid(userName, validationContext);
        }
    }
}