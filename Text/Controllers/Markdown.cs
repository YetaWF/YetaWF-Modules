/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Text#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Text.Modules;
using YetaWF.Core.Models;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Text.Controllers {

    public class MarkdownModuleController : ControllerImpl<YetaWF.Modules.Text.Modules.MarkdownModule> {

        public class MarkdownEdit : MarkdownString {
            [Required, AdditionalMetadata("EmHeight", 25)]
            public override string? Text { get; set; } 
        }
        public class MarkdownDisplay : MarkdownStringBase {
            [ReadOnly, AdditionalMetadata("EmHeight", 25)]
            public override string? Text { get; set; } 
        }

        public class ModelEdit {
            [UIHint("Markdown"), AdditionalMetadata("EmHeight", 25)]
            public MarkdownEdit Contents { get; set; }

            public ModelEdit() {
                Contents = new MarkdownEdit();
            }
        }
        public class ModelDisplay {
            [UIHint("Markdown"), ReadOnly]
            public MarkdownDisplay Contents { get; set; }

            public ModelDisplay() {
                Contents = new MarkdownDisplay();
            }
        }

        [AllowGet]
        public ActionResult Markdown() {
            if (Manager.EditMode && Module.IsAuthorized(ModuleDefinition.RoleDefinition.Edit)) {
                ModelEdit model = new ModelEdit();
                ObjectSupport.CopyData(Module.Contents, model.Contents);
                return View(model);
            } else {
                ModelDisplay model = new ModelDisplay();
                ObjectSupport.CopyData(Module.Contents, model.Contents);
                return View("MarkdownDisplay", model);
            }
        }

        [AllowPost]
        [Permission("Edit")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Markdown_Partial(ModelEdit model) {
            ObjectSupport.CopyData(model.Contents, Module.Contents);
            await Module.SaveAsync();
            if (IsApply) {
                return FormProcessed(model, "Contents saved", OnClose: OnCloseEnum.Nothing, OnPopupClose: OnPopupCloseEnum.ReloadModule, OnApply: OnApplyEnum.ReloadModule);
            } else {
                if (Manager.IsInPopup) throw new InternalError("Save & Display not available in a popup window");
                Manager.EditMode = false;
                return Redirect(Manager.ReturnToUrl, SetCurrentEditMode: true);
            }
        }
    }
}
