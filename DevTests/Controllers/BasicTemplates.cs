/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.SendEmail;
using YetaWF.Core.Support;
using YetaWF.Core.Upload;
using YetaWF.Modules.DevTests.Modules;

namespace YetaWF.Modules.DevTests.Controllers {

    public class BasicTemplatesModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.BasicTemplatesModule> {

        public BasicTemplatesModuleController() { }

        public enum ControlStatusEnum { Normal, Disabled, }

        [Trim]
        [Header("YetaWF components test - All fields are required, but some have default values which are acceptable so no warning will be shown. Some components have their own test page (see Tests > Components) so they are not included on this page.")]
        public class EditModel {

            [Category("Core"), Caption("Boolean"), Description("Boolean (Required)")]
            [UIHint("Boolean")]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public bool Boolean { get; set; }

            [Category("Core"), Caption("BooleanText"), Description("BooleanText (Required)")]
            [UIHint("BooleanText")]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public bool BooleanText { get; set; }
            public string BooleanText_Text { get; set; }

            [Category("Core"), Caption("CAProvince"), Description("CAProvince (Required)")]
            [UIHint("CAProvince")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? CAProvince { get; set; }

            [Category("Core"), Caption("CountryISO3166"), Description("CountryISO3166 (Required)")]
            [UIHint("CountryISO3166"), StringLength(80), Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Country { get; set; }

            [Category("Core"), Caption("Currency"), Description("Currency (Required) - Uses formatting defined using Site Settings, General tab")]
            [UIHint("Currency")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public decimal? Currency { get; set; }
            public string Currency_PlaceHolder { get { return this.__ResStr("currencyPH", "(Amount)"); } }

            [Category("Core"), Caption("CurrencyISO4217"), Description("CurrencyISO4217 (Required)")]
            [UIHint("CurrencyISO4217"), StringLength(YetaWF.Core.Components.CurrencyISO4217.Currency.MaxId), Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? CurrencyISO4217 { get; set; }

            [Category("Core"), Caption("Decimal"), Description("Decimal (Required)")]
            [UIHint("Decimal")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public decimal? Decimal { get; set; }
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
            public SampleEnum Enum { get; set; }

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
            public Guid? Guid { get; set; }

            [Category("Core"), Caption("IntValue"), Description("IntValue (Required)")]
            [UIHint("IntValue")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public int? IntValue { get; set; }
            public string IntValue_PlaceHolder { get { return this.__ResStr("intvaluePH", "(Number)"); } }

            [Category("Core"), Caption("IntValue2"), Description("IntValue2 (Required)")]
            [UIHint("IntValue2")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public int? IntValue2 { get; set; }

            [Category("Core"), Caption("IntValue4"), Description("IntValue4 (Required)")]
            [UIHint("IntValue4")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public int? IntValue4 { get; set; }

            [Category("Core"), Caption("IntValue6"), Description("IntValue6 (Required)")]
            [UIHint("IntValue6")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public int? IntValue6 { get; set; }

            [Category("Core"), Caption("LanguageId"), Description("LanguageId (Required)")]
            [UIHint("LanguageId"), AdditionalMetadata("NoDefault", false), AdditionalMetadata("AllLanguages", true)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? LanguageId { get; set; }

            [Category("Core"), Caption("LongValue"), Description("LongValue (Required)")]
            [UIHint("LongValue")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public long? LongValue { get; set; }

            [Category("Core"), Caption("PageSelection"), Description("PageSelection (Required)")]
            [UIHint("PageSelection"), AdditionalMetadata("New", true)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public Guid? PageSelection { get; set; }

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
            public string? Password20 { get; set; }
            public string Password20_PlaceHolder { get { return this.__ResStr("password20PH", "(Password)"); } }

            [Category("Core"), Caption("SMTPServer"), Description("SMTPServer (Required)")]
            [UIHint("SMTPServer")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public SMTPServer? SMTPServer { get; set; }

            [Category("Core"), Caption("Text"), Description("Text (Required)")]
            [UIHint("Text"), StringLength(200)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Text { get; set; }
            public string Text_PlaceHolder { get { return this.__ResStr("textPH", "(This is a placeholder)"); } }

            [Category("Core"), Caption("Text10"), Description("Text10 (Required)")]
            [UIHint("Text10"), StringLength(10)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Text10 { get; set; }

            [Category("Core"), Caption("Text20"), Description("Text20 (Required)")]
            [UIHint("Text20"), StringLength(20)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Text20 { get; set; }

            [Category("Core"), Caption("Text40"), Description("Text40 (Required)")]
            [UIHint("Text40"), StringLength(40)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Text40 { get; set; }

            [Category("Core"), Caption("Text80"), Description("Text80 (Required)")]
            [UIHint("Text80"), StringLength(80), AdditionalMetadata("Copy", true)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Text80 { get; set; }

            [Category("Core"), Caption("TextArea"), Description("TextArea (Required)")]
            [UIHint("TextArea"), AdditionalMetadata("ImageBrowse", true), AdditionalMetadata("PageBrowse", true)]
            [StringLength(1000)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? TextArea { get; set; }

            [Category("Core"), Caption("USState"), Description("USState (Required)")]
            [UIHint("USState")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true )]
            public string? USState { get; set; }

            [Category("Core"), Caption("Control Status"), Description("Defines the processing status of the controls")]
            [UIHint("Enum")]
            public ControlStatusEnum ControlStatus { get; set; }

            public EditModel() {
                // BooleanText
                BooleanText_Text = "Accept";
                // DropDownList
                DropDownList_List = new List<SelectionItem<string>> {
                    new SelectionItem<string> { Text= "(select)", Value = "", Tooltip = "No selection" },
                    new SelectionItem<string> { Text= "Text1", Value="1", Tooltip = "Tooltip1" },
                    new SelectionItem<string> { Text= "Text2", Value="2", Tooltip = "Tooltip2" },
                    new SelectionItem<string> { Text= "Text3", Value="3", Tooltip = "Tooltip3" },
                };
                // DropDownListInt
                DropDownListInt_List = new List<SelectionItem<int>> {
                    new SelectionItem<int> { Text= "(select)", Value = 0, Tooltip = "No selection" },
                    new SelectionItem<int> { Text= "Item1", Value=1, Tooltip = "Tooltip1" },
                    new SelectionItem<int> { Text= "Item2", Value=2, Tooltip = "Tooltip2" },
                    new SelectionItem<int> { Text= "Item3", Value=3, Tooltip = "Tooltip3" },
                };
                // PaneSelection
                PaneSelection_List = new List<string> { "Pane 1", "Pane 2", "Pane 3" };
                
                SMTPServer = new SMTPServer();
            }
            public void UpdateData(BasicTemplatesModule module) {
                // FileUpload1
                FileUpload1 = new FileUpload1 {
                    SelectButtonText = this.__ResStr("btnSelect", "Upload a file..."),
                    SaveURL = Utility.UrlFor(typeof(BasicTemplatesModuleController), "UploadSomething", new { __ModuleGuid = module.ModuleGuid }),
                    RemoveURL = Utility.UrlFor(typeof(BasicTemplatesModuleController), "RemoveSomething", new { __ModuleGuid = module.ModuleGuid }),
                };
            }
        }

        [AllowGet]
        public ActionResult BasicTemplates() {
            EditModel model = new EditModel();
            model.UpdateData(Module);
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult BasicTemplates_Partial(EditModel model) {
            model.UpdateData(Module);
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }

        // FileUpload1

        public class UploadResponse {
            public string Result { get; set; } = null!;
            public string FileName { get; set; } = null!;
            public string FileNamePlain { get; set; } = null!;
            public string RealFileName { get; set; } = null!;
            public string Attributes { get; set; } = null!;
            public string List { get; set; } = null!;
        }

        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> UploadSomething(IFormFile __filename) {
            // Save the uploaded file as a temp file
            FileUpload upload = new FileUpload();
            string tempName = await upload.StoreTempImageFileAsync(__filename);
            // do something with the uploaded file "tempName"
            //...
            // Delete the temp file just uploaded
            await FileSystem.TempFileSystemProvider.DeleteFileAsync(tempName);

            bool success = true;
            string msg = this.__ResStr("uploadSuccess", "File {0} successfully uploaded", __filename.FileName);

            if (success) {
                UploadResponse resp = new UploadResponse {
                    Result = $"$YetaWF.confirm('{Utility.JserEncode(msg)}', null, function() {{ /*add some javascript like  $YetaWF.reloadPage(true); */ }} );",
                };
                return new YJsonResult { Data = resp };
            } else {
                // Anything else is a failure
                throw new Error(msg);
            }
        }
        [AllowPost]
        [ExcludeDemoMode]
        public ActionResult RemoveSomething(string filename) {
            // there is nothing to remove because we already deleted the file right after uploading it
            return new EmptyResult();
        }

    }
}
