/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/Softelvdm_TwilioProcessor/Topic/License */

using PhoneNumbers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using YetaWF.Core.Localize;

namespace Softelvdm.Modules.TwilioProcessor.Models.Attributes {

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class AreaCodeUSAttribute : ValidationAttribute {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(AreaCodeUSAttribute), name, defaultValue, parms); }

        public AreaCodeUSAttribute() { }

        protected override ValidationResult IsValid(object value, ValidationContext context) {
            if (value != null) {
                string areaCode = (string)value;
                if (!AreaCodeUSAttribute.Valid(areaCode))
                    return new ValidationResult(__ResStr("inv", "{0} is an invalid area code", areaCode));
            }
            return ValidationResult.Success;
        }

        public static bool Valid(string areaCode) {
            Regex areaRegex = new Regex(@"^[0-9]{3}$");
            return areaRegex.IsMatch(areaCode);
        }
    }
}
