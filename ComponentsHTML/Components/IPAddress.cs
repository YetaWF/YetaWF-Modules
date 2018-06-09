using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class IPAddressComponentBase : YetaWFComponent {

        public const string TemplateName = "IPAddress";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class IPAddressDisplayComponent : IPAddressComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(string model) {
            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(@"<div class='yt_ipaddress t_display'>");

            if (!string.IsNullOrWhiteSpace(model)) {

                hb.Append(model);

                bool lookup = PropData.GetAdditionalAttributeValue("Lookup", true);
                if (lookup) {
                    ModuleDefinition modDisplay = await ModuleDefinition.LoadAsync(new Guid("{ad95564e-8eb7-4bcb-be64-dc6f1cd6b55d}"), AllowNone: true);
                    if (modDisplay != null) {
                        ModuleAction actionDisplay = await modDisplay.GetModuleActionAsync("DisplayHostName", null, model);
                        if (modDisplay != null)
                            hb.Append(await actionDisplay.RenderAsync(ModuleAction.RenderModeEnum.IconsOnly));
                        actionDisplay = await modDisplay.GetModuleActionAsync("DisplayGeoData", null, model);
                        if (modDisplay != null)
                            hb.Append(await actionDisplay.RenderAsync(ModuleAction.RenderModeEnum.IconsOnly));
                    }
                }
            }
            hb.Append(@"</div>");
            return hb.ToYHtmlString();
        }
    }
}
