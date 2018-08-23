﻿/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.Views {

    public class NeedNewPasswordDisplayView : YetaWFView, IYetaWFView<NeedNewPasswordDisplayModule, NeedNewPasswordDisplayModuleController.DisplayModel> {

        public const string ViewName = "NeedNewPasswordDisplay";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(NeedNewPasswordDisplayModule module, NeedNewPasswordDisplayModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='t_message'>
    {HE(this.__ResStr("needNewPassword", "Please change your login password - "))}
    {await model.ChangePasswordAction.RenderAsLinkAsync()}
</div>
<script>
 $('#{module.ModuleHtmlId}').prependTo('body');
</script>
");

            return hb.ToYHtmlString();
        }
    }
}