/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Defines column horizontal alignment.
    /// </summary>
    public enum GridHAlignmentEnum {
        /// <summary>
        /// None specified.
        /// </summary>
        Unspecified = -1,
        /// <summary>
        /// Left aligned.
        /// </summary>
        Left = 0,
        /// <summary>
        /// Centered.
        /// </summary>
        Center = 1,
        /// <summary>
        /// Right aligned.
        /// </summary>
        Right = 2
    }

    internal class GridColumnInfo {
        public int ChWidth { get; set; }
        public int PixWidth { get; set; }
        public bool Sortable { get; set; }
        public bool Locked { get; set; }
        public bool Hidden { get; set; }
        public bool OnlySubmitWhenChecked { get; set; }
        public GridHAlignmentEnum Alignment { get; set; }
        public int Icons { get; set; }
        public List<FilterOptionEnum> FilterOptions { get; set; }
        public enum FilterOptionEnum {
            Equal = 1,
            NotEqual,
            LessThan,
            LessEqual,
            GreaterThan,
            GreaterEqual,
            StartsWith,
            NotStartsWith,
            Contains,
            NotContains,
            Endswith,
            NotEndswith,
            All = 0xffff,
        }

        public GridColumnInfo() {
            PixWidth = ChWidth = 0;
            Sortable = false;
            Locked = false;
            Hidden = false;
            OnlySubmitWhenChecked = false;
            Alignment = GridHAlignmentEnum.Unspecified;
            Icons = 0;
            FilterOptions = new List<FilterOptionEnum>();
        }
    }
}
