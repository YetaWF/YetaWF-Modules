/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TwilioProcessor#License */

using PhoneNumbers;
using System;
using System.ComponentModel.DataAnnotations;
using YetaWF.Core.Localize;
using YetaWF.Core.Support;

namespace Softelvdm.Modules.TwilioProcessor.Models.Attributes {

    //$$$$$ should remove this, dup of PhoneNumberUSAttribute
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PhoneNumberAttribute : ValidationAttribute {

        //private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(PhoneNumberAttribute), name, defaultValue, parms); }

        //public PhoneNumberAttribute() { }

        //protected override ValidationResult IsValid(object value, ValidationContext context) {
        //    if (value != null) {
        //        string number = (string)value;
        //        if (!PhoneNumberAttribute.Valid(number))
        //            return new ValidationResult(__ResStr("inv", "{0} is an invalid phone number", number));
        //    }
        //    return ValidationResult.Success;
        //}

        //public static bool Valid(string phoneNumber) {
        //    PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
        //    try {
        //        PhoneNumber p = phoneNumberUtil.Parse(phoneNumber, null);
        //        return phoneNumberUtil.IsValidNumber(p);
        //    } catch (Exception) {
        //        return false;
        //    }
        //}

        public static string GetE164(string phoneNumber) {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return null;
            PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
            try {
                PhoneNumber p = phoneNumberUtil.Parse(phoneNumber, "US");
                if (!phoneNumberUtil.IsValidNumber(p))
                    return phoneNumber;
                string e164 = phoneNumberUtil.Format(p, PhoneNumberFormat.E164);
                if (string.IsNullOrWhiteSpace(e164))
                    throw new Error("badNumber", "Phone number {0} is not a valid number", phoneNumber);
                return e164;
            } catch (Exception) {
                return null;
            }
        }
    }
}
