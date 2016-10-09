/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.ComponentModel.DataAnnotations;
using YetaWF.Core.Localize;

namespace YetaWF.Modules.Blog.Support {

    public class Resources { } // class holding all localization resources

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ShortNameValidationAttribute : YetaWF.Core.Models.Attributes.RegularExpressionAttribute {

        [CombinedResources]
        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(Resources), name, defaultValue, parms); }

        public ShortNameValidationAttribute()
            : base(@"[A-Za-z0-9_\-]*") {
            ErrorMessage = __ResStr("valShortName", "A shortname must consist of letters, numbers, underscores or dashes, without spaces.");
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            if (value == null) return ValidationResult.Success;
            string shortName = ((string) value).Trim();
            return base.IsValid(shortName, validationContext);
        }
    }
}