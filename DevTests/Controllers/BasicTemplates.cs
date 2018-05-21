/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Core.Upload;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.DevTests.Modules;
using YetaWF.Core.Models;
using YetaWF.Core.SendEmail;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Templates;
#if MVC6
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class BasicTemplatesModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.BasicTemplatesModule> {

        public BasicTemplatesModuleController() { }

        [Trim]
        [Header("YetaWF templates test - All fields are required, but some have default values which are acceptable so no warning will be shown. Some templates have their own test page (see Tests > Templates) so they are not included on this page.")]
        public class EditModel {

            [Category("Core"), Caption("Boolean"), Description("Boolean (Required)")]
            [UIHint("Boolean"), Required]
            public bool Boolean { get; set; }

            [Category("Core"), Caption("CountryISO3166"), Description("CountryISO3166 (Required)")]
            [UIHint("CountryISO3166"), StringLength(80), Trim, Required]
            public string Country { get; set; }

            [Category("Core"), Caption("Currency"), Description("Currency (Required) - Uses formatting defined using Site Settings, General tab")]
            [UIHint("Currency"), Required]
            public decimal? Currency { get; set; }

            [Category("Core"), Caption("CurrencyISO4217"), Description("CurrencyISO4217 (Required)")]
            [UIHint("CurrencyISO4217"), StringLength(YetaWF.Core.Templates.CurrencyISO4217.Currency.MaxId), Trim, Required]
            public string CurrencyISO4217 { get; set; }

            [Category("Core"), Caption("Decimal"), Description("Decimal (Required)")]
            [UIHint("Decimal"), Required]
            public decimal? Decimal { get; set; }

            [Category("Core"), Caption("DropDownList"), Description("DropDownList (SelectionRequired)")]
            [UIHint("DropDownList"), StringLength(20), SelectionRequired]
            public string DropDownList { get; set; }
            public List<SelectionItem<string>> DropDownList_List { get; set; }

            [Category("Core"), Caption("DropDownListInt"), Description("DropDownListInt (SelectionRequired)")]
            [UIHint("DropDownListInt"), SelectionRequired]
            public int DropDownListInt { get; set; }
            public List<SelectionItem<int>> DropDownListInt_List { get; set; }

            [Category("Core"), Caption("Enum"), Description("Enum (Required)")]
            [UIHint("Enum"), Required]
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
            [UIHint("FileUpload1"), Required]
            public FileUpload1 FileUpload1 { get; set; }

            [Category("Core"), Caption("Guid"), Description("Guid (Required)")]
            [UIHint("Guid"), GuidValidation, Required]
            public Guid? Guid { get; set; }

            [Category("Core"), Caption("IntValue"), Description("IntValue (Required)")]
            [UIHint("IntValue"), Required]
            public int? IntValue { get; set; }

            [Category("Core"), Caption("IntValue2"), Description("IntValue2 (Required)")]
            [UIHint("IntValue2"), Required]
            public int IntValue2 { get; set; }

            [Category("Core"), Caption("IntValue4"), Description("IntValue4 (Required)")]
            [UIHint("IntValue4"), Required]
            public int IntValue4 { get; set; }

            [Category("Core"), Caption("IntValue6"), Description("IntValue6 (Required)")]
            [UIHint("IntValue6"), Required]
            public int IntValue6 { get; set; }

            [Category("Core"), Caption("JQueryUISkin"), Description("JQueryUISkin (Required)")]
            [UIHint("JQueryUISkin"), AdditionalMetadata("NoDefault", false), Required]
            public string JQueryUISkin { get; set; }

            [Category("Core"), Caption("KendoUISkin"), Description("KendoUISkin (Required)")]
            [UIHint("KendoUISkin"), AdditionalMetadata("NoDefault", false), Required]
            public string KendoUISkin { get; set; }

            [Category("Core"), Caption("LanguageId"), Description("LanguageId (Required)")]
            [UIHint("LanguageId"), AdditionalMetadata("NoDefault", false), AdditionalMetadata("AllLanguages", true), Required]
            public string LanguageId { get; set; }

            [Category("Core"), Caption("LongValue"), Description("LongValue (Required)")]
            [UIHint("LongValue"), Required]
            public long LongValue { get; set; }

            [Category("Core"), Caption("MultiString"), Description("MultiString (Required)")]
            [UIHint("MultiString"), StringLength(200), Required]
            public MultiString MultiString { get; set; }

            [Category("Core"), Caption("MultiString10"), Description("MultiString10 (Required)")]
            [UIHint("MultiString10"), StringLength(10), Required]
            public MultiString MultiString10 { get; set; }

            [Category("Core"), Caption("MultiString20"), Description("MultiString20 (Required)")]
            [UIHint("MultiString20"), StringLength(20), Required]
            public MultiString MultiString20 { get; set; }

            [Category("Core"), Caption("MultiString40"), Description("MultiString40 (Required)")]
            [UIHint("MultiString40"), StringLength(410), Required]
            public MultiString MultiString40 { get; set; }

            [Category("Core"), Caption("MultiString80"), Description("MultiString80 (Required)")]
            [UIHint("MultiString80"), StringLength(80), Required]
            public MultiString MultiString80 { get; set; }

            [Category("Core"), Caption("PageSelection"), Description("PageSelection (Required)")]
            [UIHint("PageSelection"), AdditionalMetadata("New", true), Required]
            public Guid PageSelection { get; set; }

            [Category("Core"), Caption("PaneSelection"), Description("PaneSelection (Required)")]
            [UIHint("PaneSelection"), Required]
            public string PaneSelection { get; set; }
            public List<string> PaneSelection_List { get; set; }

            [Category("Core"), Caption("Password20"), Description("Password20 (Required)")]
            [UIHint("Password20"), StringLength(20), Required]
            public string Password20 { get; set; }

            [Category("Core"), Caption("SMTPServer"), Description("SMTPServer (Required)")]
            [UIHint("SMTPServer"), Required]
            public SMTPServer SMTPServer { get; set; }

            [Category("Core"), Caption("Text"), Description("Text (Required)")]
            [UIHint("Text"), StringLength(200), Required]
            public string Text { get; set; }

            [Category("Core"), Caption("Text10"), Description("Text10 (Required)")]
            [UIHint("Text10"), StringLength(10), Required]
            public string Text10 { get; set; }

            [Category("Core"), Caption("Text20"), Description("Text20 (Required)")]
            [UIHint("Text20"), StringLength(20), Required]
            public string Text20 { get; set; }

            [Category("Core"), Caption("Text40"), Description("Text40 (Required)")]
            [UIHint("Text40"), StringLength(40), Required]
            public string Text40 { get; set; }

            [Category("Core"), Caption("Text80"), Description("Text80 (Required)")]
            [UIHint("Text80"), StringLength(80), AdditionalMetadata("Copy", true), Required]
            public string Text80 { get; set; }

            [Category("Core"), Caption("TextArea"), Description("TextArea (Required)")]
            [UIHint("TextArea"), StringLength(1000), Required]
            public string TextArea { get; set; }

            [Category("Core"), Caption("USState"), Description("USState (Required)")]
            [UIHint("USState"), Required]
            public string USState { get; set; }

            public EditModel() {
                MultiString = new MultiString();
                MultiString10 = new MultiString();
                MultiString20 = new MultiString();
                MultiString40 = new MultiString();
                MultiString80 = new MultiString();

                SMTPServer = new SMTPServer();
            }
            public void UpdateData(BasicTemplatesModule module) {
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
                // FileUpload1
                FileUpload1 = new FileUpload1 {
                    SelectButtonText = this.__ResStr("btnSelect", "Upload a file..."),
                    SaveURL = YetaWFManager.UrlFor(typeof(BasicTemplatesModuleController), "UploadSomething", new { __ModuleGuid = module.ModuleGuid }),
                    RemoveURL = YetaWFManager.UrlFor(typeof(BasicTemplatesModuleController), "RemoveSomething", new { __ModuleGuid = module.ModuleGuid }),
                };
                // PaneSelection
                PaneSelection_List = new List<string> { "Pane 1", "Pane 2", "Pane 3" };
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

        [AllowPost]
        [ExcludeDemoMode]
#if MVC6
        public async Task<ActionResult> UploadSomething(IFormFile __filename)
#else
        public async Task<ActionResult> UploadSomething(HttpPostedFileBase __filename)
#endif
        {
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
                // Upload control considers Json result a success
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append("{{ \"result\": \"Y_Confirm(\\\"{0}\\\", null, function() {{ /*add some javascript like  Y_ReloadPage(true); */ }} ); \" }}",
                    YetaWFManager.JserEncode(YetaWFManager.JserEncode(msg))
                );
                return new YJsonResult { Data = sb.ToString() };
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
