/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TwilioProcessor#License */

using PhoneNumbers;
using System;
using System.ComponentModel.DataAnnotations;
using YetaWF.Core.Localize;
using YetaWF.Core.Support;

namespace Softelvdm.Modules.TwilioProcessor.Models.Attributes {

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PhoneNumberUSAttribute : ValidationAttribute {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(PhoneNumberUSAttribute), name, defaultValue, parms); }

        public PhoneNumberUSAttribute() { }

        protected override ValidationResult IsValid(object value, ValidationContext context) {
            if (value != null) {
                string number = (string)value;
                if (!PhoneNumberUSAttribute.Valid(number))
                    return new ValidationResult(__ResStr("inv", "{0} is an invalid phone number", number));
            }
            return ValidationResult.Success;
        }

        public static bool Valid(string phoneNumber) {
            PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
            try {
                PhoneNumber p = phoneNumberUtil.Parse(phoneNumber, "US");//$$$$$
                if (phoneNumberUtil.IsValidNumberForRegion(p, "US") || phoneNumberUtil.IsValidNumberForRegion(p, "CA"))
                    throw new Error(__ResStr("invUSCan", "{0} is not a US or Canadian phone number. International phone numbers are not supported.", phoneNumber));
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public static string GetE164(string phoneNumber) {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return null;
            PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
            try {
                PhoneNumber p = phoneNumberUtil.Parse(phoneNumber, "US");
                if (!phoneNumberUtil.IsValidNumber(p))
                    return null;
                return phoneNumberUtil.Format(p, PhoneNumberFormat.E164);
            } catch (Exception) {
                return null;
            }
        }
    }
}
