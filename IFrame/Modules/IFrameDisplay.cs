/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/IFrame#License */

using System;
using System.Web.Mvc;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Views.Shared;
using YetaWF.DataProvider;

namespace YetaWF.Modules.IFrame.Modules {

    public class IFrameDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, IFrameDisplayModule>, IInstallableModel { }

    [ModuleGuid("{b8004ca1-50bd-45cd-b9c5-77e8ba27234d}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class IFrameDisplayModule : ModuleDefinition {

        public override IModuleDefinitionIO GetDataProvider() { return new IFrameDisplayModuleDataProvider(); }

        public IFrameDisplayModule() {
            Title = this.__ResStr("modTitle", "IFrame");
            Name = this.__ResStr("modName", "IFrame (Display Url)");
            Description = this.__ResStr("modSummary", "Includes another page (Url) and displays the page");
            Url = "http://LinksWithPics.com";
            Width = "100%";
            Height = "800px";
            WantSearch = false;
        }

        [Category("General"), Caption("Url"), Description("The Url used to display the module contents")]
        [StringLength(Globals.MaxUrl), Required, Trim]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local | UrlHelperEx.UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local | UrlHelperEx.UrlTypeEnum.Remote)]
        public string Url { get; set; }

        [Category("General"), Caption("Width"), Description("The width used to display the module contents - used as is as Css width (e.g., 100%, 50px, 40em)")]
        [UIHint("Text20"), StringLength(20), Trim]
        public string Width { get; set; }

        [Category("General"), Caption("Height"), Description("The height used to display the module contents - used as is as Css height (e.g., 800px, 40em)")]
        [UIHint("Text20"), StringLength(20), Trim]
        public string Height { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("IFrameLink", "IFrame"),
                MenuText = this.__ResStr("IFrameText", "IFrame"),
                Tooltip = this.__ResStr("IFrameTooltip", "Display a page in an iframe"),
                Legend = this.__ResStr("IFrameLegend", "Displays a page in an iframe"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}