﻿using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.SendEmail;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class SMTPServerComponent : YetaWFComponent, IYetaWFComponent<SMTPServer> {

        public const string TemplateName = "SMTPServer";

        public override ComponentType GetComponentType() { return ComponentType.Edit; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public async Task<YHtmlString> RenderAsync(SMTPServer model) {

            using (Manager.StartNestedComponent(FieldName)) {

                HtmlBuilder hb = new HtmlBuilder();
                hb.Append($@"
<div id='{DivId}' class='yt_smtpserver t_edit'>
    {await HtmlHelper.RenderPropertyListAsync("", model)}
</div>
<script>
    YetaWF_SMTPServer.init('{DivId}');
</script>");
                return hb.ToYHtmlString();
            }
        }
    }
}
