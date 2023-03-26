/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TestEscapesModuleDataProvider : ModuleDefinitionDataProvider<Guid, TestEscapesModule>, IInstallableModel { }

    [ModuleGuid("{d0071658-35f2-453d-a5ba-2a91bd01ee49}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TestEscapesModule : ModuleDefinition {

        public TestEscapesModule() {
            Title = this.__ResStr("modTitle", "Test Escapes");
            Name = this.__ResStr("modName", "Test Escapes (HTML, Attributes)");
            Description = this.__ResStr("modSummary", "Tests various client-side & server side character translations/escapes. A test page for this module can be found at Tests > Modules > Escapes (standard YetaWF site).");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TestEscapesModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Test(string url) {
            return new ModuleAction() {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { SomeChars = "TEST < > & @ {0} TEST" },
                Image = "#Display",
                LinkText = this.__ResStr("editLink", "Escapes"),
                MenuText = this.__ResStr("editText", "Escapes"),
                Tooltip = this.__ResStr("editTooltip", "Test various client-side & server side character translations/escapes"),
                Legend = this.__ResStr("editLegend", "Tests various client-side & server side character translations/escapes"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,

            };
        }

        [Trim]
        [Header("This test page is used to test correct character encoding - header (HeaderAttribute) <A> &amp; & @ {0}")]
        [Footer("Special characters in the footer (FooterAttribute) <A> &amp; & @ {0}")]
        public class EditModel {

            [TextAbove("Special characters in the text above the field (TextAboveAttribute) <A> &amp; & @ {0}")]
            [TextBelow("Special characters in the text below the field (TextBelowAttribute) <A> &amp; & @ {0}")]
            [Caption("Caption <A> &amp; & @ {0}"), Description("A description <A> &amp; & @ {0}")]
            [UIHint("Text80"), StringLength(80), Trim]
            public string? String1 { get; set; } = null!;

            public EditModel() { }
        }

        public Task<ActionInfo> RenderModuleAsync() {
            EditModel model = new EditModel { };
            return RenderAsync(model);
        }

        public Task<IResult> UpdateModuleAsync(EditModel model) {
            if (!ModelState.IsValid)
                return PartialViewAsync(model);
            return FormProcessedAsync(model, this.__ResStr("okSaved", "Test Done - Here are some special characters in a message: <A> &amp; & @ {0}"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}
