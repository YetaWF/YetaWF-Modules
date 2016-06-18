/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Web.Mvc;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Blog.Modules {

    public class CategoriesListModuleDataProvider : ModuleDefinitionDataProvider<Guid, CategoriesListModule>, IInstallableModel { }

    [ModuleGuid("{9d83d810-2c2d-44eb-a177-c2c00198e9e8}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class CategoriesListModule : ModuleDefinition {

        public CategoriesListModule() {
            Title = this.__ResStr("modTitle", "Blog Categories");
            Name = this.__ResStr("modName", "Blog Categories List");
            Description = this.__ResStr("modSummary", "Allows the site visitor to select among all available blog categories - usually used in a side bar");
            Style = StyleEnum.Dropdown;
            ListEntries = 10;
            ShowTitle = false;
            WantFocus = false;
            WantSearch = false;
            Print = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new CategoriesListModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

        [Category("General"), Caption("Default Category"), Description("The default category")]
        [UIHint("YetaWF_Blog_Category"), AdditionalMetadata("ShowAll", true)]
        public int DefaultCategory { get; set; }

        [Category("General"), Caption("Style"), Description("Defines how the blog categories are displayed")]
        [UIHint("Enum"), Required]
        public StyleEnum Style { get; set; }

        public enum StyleEnum {
            [EnumDescription("List", "Blog categories are displayed as a list")]
            List = 0,
            [EnumDescription("DropDown", "Blog categories are displayed in a dropdown")]
            Dropdown = 1,
        }

        [Category("General"), Caption("List Entries"), Description("The number of list entries shown before a scrollbar is added - this property is only used if the Style property is set to List")]
        [UIHint("IntValue2"), Range(1, 99), Required, Trim]
        public int ListEntries { get; set; }
    }
}