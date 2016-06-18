/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/TinyLogin#License */

using System;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;

namespace YetaWF.Modules.TinyLogin.Support {

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LogoffRegularExpressionAttribute : RegularExpressionAttribute {
        [CombinedResources]
        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(LogoffRegularExpressionAttribute), name, defaultValue, parms); }

        public LogoffRegularExpressionAttribute() : base(@"\/.*") { }

        protected override System.ComponentModel.DataAnnotations.ValidationResult IsValid(object value, System.ComponentModel.DataAnnotations.ValidationContext validationContext) {
            System.ComponentModel.DataAnnotations.ValidationResult result = base.IsValid(value, validationContext);
            if (result == System.ComponentModel.DataAnnotations.ValidationResult.Success)
                return System.ComponentModel.DataAnnotations.ValidationResult.Success;
            string errorMessage = __ResStr("err", "The field '{0}' must start with a / character (a direct Url on this site)", AttributeHelper.GetPropertyCaption(validationContext));
            return new System.ComponentModel.DataAnnotations.ValidationResult(errorMessage);
        }
    }
}
