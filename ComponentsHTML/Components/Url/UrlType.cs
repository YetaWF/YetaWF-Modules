using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class UrlTypeComponent : YetaWFComponent, IYetaWFComponent<UrlTypeEnum> {

        public const string TemplateName = "UrlType";

        public override ComponentType GetComponentType() { return ComponentType.Edit; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public async Task<YHtmlString> RenderAsync(UrlTypeEnum model) {

            List<SelectionItem<string>> items = new List<SelectionItem<string>>();
            if ((model & UrlTypeEnum.Local) != 0) {
                items.Add(new SelectionItem<string> {
                    Text = this.__ResStr("selLocal", "Designed Page"),
                    Value = ((int)UrlTypeEnum.Local).ToString(),
                    Tooltip = this.__ResStr("selLocalTT", "Select for local, designed pages"),
                });
            }
            if ((model & UrlTypeEnum.Remote) != 0) {
                items.Add(new SelectionItem<string> {
                    Text = this.__ResStr("selRemote", "Local/Remote Url"),
                    Value = ((int)UrlTypeEnum.Remote).ToString(),
                    Tooltip = this.__ResStr("selRemoteTT", "Select to enter a Url (local or remote) - Can contain query string arguments - Local Urls start with /, remote Urls with http:// or https://"),
                });
            }
            if ((model & UrlTypeEnum.New) != 0)
                throw new InternalError("New url not supported by this template");

            return await DropDownListComponent.RenderDropDownListAsync(this, model.ToString(), items, "yt_urltype");
        }
    }
}
