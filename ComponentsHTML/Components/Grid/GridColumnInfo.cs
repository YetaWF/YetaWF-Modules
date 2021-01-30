/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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

    /// <summary>
    /// Defines how the column is shown.
    /// </summary>
    public enum ColumnVisibilityStatus {
        /// <summary>
        /// By default the column is shown. The user can change the column using the column selection dropdown (if available).
        /// </summary>
        Shown = 0,
        /// <summary>
        /// By default the column is not shown. The user can change the column using the column selection dropdown (if available).
        /// </summary>
        NotShown = 1,
        /// <summary>
        /// By column is always shown. The user cannot change the column using the column selection dropdown.
        /// </summary>
        AlwaysShown = 2,
    }

    internal class GridColumnInfo {
        public int ChWidth { get; set; }
        public int PixWidth { get; set; }
        public bool Sort { get; set; }
        public bool Locked { get; set; }
        public bool Truncate { get; set; }
        public ColumnVisibilityStatus Visibility { get; set; }
        public bool Hidden { get; set; }
        public bool OnlySubmitWhenChecked { get; set; }
        public GridHAlignmentEnum Align { get; set; }
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
        public string OriginalWidthCh { get; set; }

        public GridColumnInfo() {
            PixWidth = ChWidth = 0;
            Sort = false;
            Locked = false;
            Visibility = ColumnVisibilityStatus.Shown;
            Hidden = false;
            OnlySubmitWhenChecked = false;
            Align = GridHAlignmentEnum.Unspecified;
            Icons = 0;
            FilterOptions = new List<FilterOptionEnum>();
        }
    }
}
