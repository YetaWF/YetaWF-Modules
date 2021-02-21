/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF.Modules.ComponentsHTML.Components {

    internal class NumberSetup {
        public double Min { get; set; }
        public double Max { get; set; }
        public double Step { get; set; }
        public string? Lead { get; set; }
        public string? Trail { get; set; }
        public int Digits { get; set; }
        public string? Currency { get; set; }
        public string? Locale { get; set; }
    }
}
