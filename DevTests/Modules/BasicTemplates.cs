/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.SendEmail;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.DevTests.Endpoints;

namespace YetaWF.Modules.DevTests.Modules {

    public class BasicTemplatesModuleDataProvider : ModuleDefinitionDataProvider<Guid, BasicTemplatesModule>, IInstallableModel { }

    [ModuleGuid("{479f90d6-e15b-41cc-9117-53fb42a10a9e}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class BasicTemplatesModule : ModuleDefinition {

        public BasicTemplatesModule() {
            Title = this.__ResStr("modTitle", "Basic Components Test");
            Name = this.__ResStr("modName", "Basic Components Test");
            Description = this.__ResStr("modSummary", "Test module for many basic built-in YetaWF.Core standard templates. A test page for this module can be found at Tests > Templates > Basic Templates (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new BasicTemplatesModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction() {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("editLink", "Basic Components"),
                MenuText = this.__ResStr("editText", "Basic Components"),
                Tooltip = this.__ResStr("editTooltip", "Used to test all generally available components"),
                Legend = this.__ResStr("editLegend", "Used to test all generally available components"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,

            };
        }

        public enum ControlStatusEnum { Normal, Disabled, }

        [Trim]
        [Header("YetaWF components test - All fields are required, but some have default values which are acceptable so no warning will be shown. Some components have their own test page (see Tests > Components) so they are not included on this page.")]
        public class EditModel {

            [Category("Core"), Caption("Boolean"), Description("Boolean (Required)")]
            [UIHint("Boolean")]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public bool Boolean { get; set; } = true;

            [Category("Core"), Caption("BooleanText"), Description("BooleanText (Required)")]
            [UIHint("BooleanText")]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public bool BooleanText { get; set; } = true;
            public string BooleanText_Text { get; set; } = "Accept";

            [Category("Core"), Caption("CAProvince"), Description("CAProvince (Required)")]
            [UIHint("CAProvince")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? CAProvince { get; set; } = "AB";

            [Category("Core"), Caption("CountryISO3166"), Description("CountryISO3166 (Required)")]
            [UIHint("CountryISO3166"), StringLength(80), Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Country { get; set; } = "Switzerland";

            [Category("Core"), Caption("Currency"), Description("Currency (Required) - Uses formatting defined using Site Settings, General tab")]
            [UIHint("Currency")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public decimal? Currency { get; set; } = 10.0M;
            public string Currency_PlaceHolder { get { return this.__ResStr("currencyPH", "(Amount)"); } }

            [Category("Core"), Caption("CurrencyISO4217"), Description("CurrencyISO4217 (Required)")]
            [UIHint("CurrencyISO4217"), StringLength(YetaWF.Core.Components.CurrencyISO4217.Currency.MaxId), Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? CurrencyISO4217 { get; set; } = "USD";

            [Category("Core"), Caption("Decimal"), Description("Decimal (Required)")]
            [UIHint("Decimal")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public decimal? Decimal { get; set; } = 0.12M;
            public string Decimal_PlaceHolder { get { return this.__ResStr("decimalPH", "(Number)"); } }

            [Category("Core"), Caption("DropDownList"), Description("DropDownList (SelectionRequired)")]
            [UIHint("DropDownList"), StringLength(20)]
            [SelectionRequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? DropDownList { get; set; }
            public List<SelectionItem<string>> DropDownList_List { get; set; }

            [Category("Core"), Caption("DropDownListInt"), Description("DropDownListInt (SelectionRequired)")]
            [UIHint("DropDownListInt")]
            [SelectionRequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public int DropDownListInt { get; set; }
            public List<SelectionItem<int>> DropDownListInt_List { get; set; }

            [Category("Core"), Caption("Enum"), Description("Enum (Required)")]
            [UIHint("Enum")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public SampleEnum Enum { get; set; } = SampleEnum.Value1;

            // ENUM
            public enum SampleEnum {
                [EnumDescription("Enum 1", "Tooltip for Enum 1")]
                Value1 = 1,
                [EnumDescription("Enum 2", "Tooltip for Enum 2")]
                Value2 = 2,
                [EnumDescription("Enum 3", "Tooltip for Enum 3")]
                Value3 = 3,
            }

            [Category("Core"), Caption("FileUpload1"), Description("FileUpload1")]
            [UIHint("FileUpload1")]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public FileUpload1? FileUpload1 { get; set; }

            [Category("Core"), Caption("Guid"), Description("Guid (Required)")]
            [UIHint("Guid"), GuidValidation]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public Guid? Guid { get; set; } = new Guid();

            [Category("Core"), Caption("IntValue"), Description("IntValue (Required)")]
            [UIHint("IntValue")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public int? IntValue { get; set; } = 99;
            public string IntValue_PlaceHolder { get { return this.__ResStr("intvaluePH", "(Number)"); } }

            [Category("Core"), Caption("IntValue2"), Description("IntValue2 (Required)")]
            [UIHint("IntValue2")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public int? IntValue2 { get; set; } = 22;

            [Category("Core"), Caption("IntValue4"), Description("IntValue4 (Required)")]
            [UIHint("IntValue4")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public int? IntValue4 { get; set; } = 4444;

            [Category("Core"), Caption("IntValue6"), Description("IntValue6 (Required)")]
            [UIHint("IntValue6")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public int? IntValue6 { get; set; } = 666666;

            [Category("Core"), Caption("LongValue"), Description("LongValue (Required)")]
            [UIHint("LongValue")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public long? LongValue { get; set; } = 99999999;

            [Category("Core"), Caption("LanguageId"), Description("LanguageId (Required)")]
            [UIHint("LanguageId"), AdditionalMetadata("NoDefault", false), AdditionalMetadata("AllLanguages", true)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? LanguageId { get; set; } = "en-US";


            [Category("Core"), Caption("PageSelection"), Description("PageSelection (Required)")]
            [UIHint("PageSelection"), AdditionalMetadata("New", true)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public Guid? PageSelection { get; set; } = new Guid("2ddebd7e-8989-45c4-9407-8f4d05d3967f");

            [Category("Core"), Caption("PaneSelection"), Description("PaneSelection (Required)")]
            [UIHint("PaneSelection")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? PaneSelection { get; set; }
            public List<string> PaneSelection_List { get; set; }

            [Category("Core"), Caption("Password20"), Description("Password20 (Required)")]
            [UIHint("Password20"), StringLength(20)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Password20 { get; set; } = "123";
            public string Password20_PlaceHolder { get { return this.__ResStr("password20PH", "(Password)"); } }

            [Category("Core"), Caption("SMTPServer"), Description("SMTPServer (Required)")]
            [UIHint("SMTPServer")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public SMTPServer? SMTPServer { get; set; }

            [Category("Core"), Caption("Switch"), Description("Switch")]
            [UIHint("Switch"), Required]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public bool Switch { get; set; } = true;
            public string Switch_Text { get; set; } = "Lorem ipsum";
            public string Switch_Tooltip { get; set; } = "Lorem ipsum tooltip";
            public string Switch_On { get; set; } = "Enabled";
            public string Switch_Off { get; set; } = "Disabled";
            public string Switch_Size { get; set; } = "md";

            [Category("Core"), Caption("Switch (Disabled)"), Description("Switch (Disabled)")]
            [UIHint("Switch"), ReadOnly]
            public bool SwitchDis { get; set; } = true;
            public string SwitchDis_Text { get; set; } = "Lorem ipsum";
            public string SwitchDis_Tooltip { get; set; } = "Lorem ipsum tooltip";
            public string SwitchDis_On { get; set; } = "On";
            public string SwitchDis_Off { get; set; } = "Off";
            //public string SwitchDis_Size { get; set; } = "md";

            [Category("Core"), Caption("Text"), Description("Text (Required)")]
            [UIHint("Text"), StringLength(200)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Text { get; set; } = "Text";
            public string Text_PlaceHolder { get { return this.__ResStr("textPH", "(This is a placeholder)"); } }

            [Category("Core"), Caption("Text10"), Description("Text10 (Required)")]
            [UIHint("Text10"), StringLength(10)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Text10 { get; set; } = "Text10";

            [Category("Core"), Caption("Text20"), Description("Text20 (Required)")]
            [UIHint("Text20"), StringLength(20)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Text20 { get; set; } = "Text20";

            [Category("Core"), Caption("Text40"), Description("Text40 (Required)")]
            [UIHint("Text40"), StringLength(40)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Text40 { get; set; } = "Text40";

            [Category("Core"), Caption("Text80"), Description("Text80 (Required)")]
            [UIHint("Text80"), StringLength(80), AdditionalMetadata("Copy", true)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Text80 { get; set; } = "Text80";

            [Category("Core"), Caption("TextArea"), Description("TextArea (Required)")]
            [UIHint("TextArea"), AdditionalMetadata("ImageBrowse", true), AdditionalMetadata("PageBrowse", true)]
            [StringLength(1000)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? TextArea { get; set; } = "Text Area";

            [Category("Core"), Caption("USState"), Description("USState (Required)")]
            [UIHint("USState")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? USState { get; set; } = "FL";

            [Category("Core"), Caption("Control Status"), Description("Defines the processing status of the controls")]
            [UIHint("Enum")]
            public ControlStatusEnum ControlStatus { get; set; } = ControlStatusEnum.Normal;

            public EditModel() {
                // DropDownList
                DropDownList = "Text1";
                DropDownList_List = new List<SelectionItem<string>> {
                    new SelectionItem<string> { Text= "(select)", Value = "", Tooltip = "No selection" },
                    new SelectionItem<string> { Text= "Text1", Value="1", Tooltip = "Tooltip1" },
                    new SelectionItem<string> { Text= "Text2", Value="2", Tooltip = "Tooltip2" },
                    new SelectionItem<string> { Text= "Text3", Value="3", Tooltip = "Tooltip3" },
                };
                // DropDownListInt
                DropDownListInt = 1;
                DropDownListInt_List = new List<SelectionItem<int>> {
                    new SelectionItem<int> { Text= "(select)", Value = 0, Tooltip = "No selection" },
                    new SelectionItem<int> { Text= "Item1", Value=1, Tooltip = "Tooltip1" },
                    new SelectionItem<int> { Text= "Item2", Value=2, Tooltip = "Tooltip2" },
                    new SelectionItem<int> { Text= "Item3", Value=3, Tooltip = "Tooltip3" },
                };
                // PaneSelection
                PaneSelection = "Pane 1";
                PaneSelection_List = new List<string> { "Pane 1", "Pane 2", "Pane 3" };

                SMTPServer = new SMTPServer();
            }
            public void UpdateData(BasicTemplatesModule module) {
                // FileUpload1
                FileUpload1 = new FileUpload1 {
                    SelectButtonText = this.__ResStr("btnSelect", "Upload a file..."),
                    SaveURL = Utility.UrlFor(typeof(BasicTemplatesModuleEndpoints), nameof(BasicTemplatesModuleEndpoints.UploadSomething)),
                    RemoveURL = Utility.UrlFor(typeof(BasicTemplatesModuleEndpoints), nameof(BasicTemplatesModuleEndpoints.RemoveSomething)),
                };
            }
        }

        public async Task<ActionInfo> RenderModuleAsync() {
            EditModel model = new EditModel();
            model.UpdateData(this);
            return await RenderAsync(model);
        }

        public async Task<IResult> UpdateModuleAsync(EditModel model) {
            model.UpdateData(this);
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            return await FormProcessedAsync(model, this.__ResStr("ok", "OK"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}
