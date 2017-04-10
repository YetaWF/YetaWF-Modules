/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Licensing */

using System;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
#else
using System.Collections.Generic;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.UserProfile.Attributes {

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ZipCodeValidationAttribute : System.ComponentModel.DataAnnotations.RegularExpressionAttribute, YIClientValidatable {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(Resources), name, defaultValue, parms); }

        public ZipCodeValidationAttribute() : base(@"^\s*[0-9]{5}(\-[0-9]{4}){0,1}\s*$") { }
#if MVC6
        public void AddValidation(ClientModelValidationContext context) {
            ErrorMessage = __ResStr("valArea2", "The specified ZIP code is invalid ({0}) - Use ZIP code format 00000 or 00000-0000", AttributeHelper.GetPropertyCaption(context.ModelMetadata));
            AttributeHelper.MergeAttribute(context.Attributes, "data-val-regex", ErrorMessage);
            AttributeHelper.MergeAttribute(context.Attributes, "data-val-regex-pattern", this.Pattern);
            AttributeHelper.MergeAttribute(context.Attributes, "data-val", "true");
        }
#else
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context) {
            ErrorMessage = __ResStr("valArea2", "The specified ZIP code is invalid ({0}) - Use ZIP code format 00000 or 00000-0000", AttributeHelper.GetPropertyCaption(metadata));
            return new[] { new ModelClientValidationRegexRule(ErrorMessage, this.Pattern) };
        }
#endif
    }
}
