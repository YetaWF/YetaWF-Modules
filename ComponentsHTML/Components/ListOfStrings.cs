/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class ListOfStringsComponentBase : YetaWFComponent {

        public const string TemplateName = "ListOfStrings";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class ListOfStringsDisplayComponent : ListOfStringsComponentBase, IYetaWFComponent<List<string>> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(List<string> model) {
            HtmlBuilder hb = new HtmlBuilder();

            string delim = PropData.GetAdditionalAttributeValue("Delimiter", ", ");

            hb.Append($@"
<div class='yt_listofstrings t_display'>");

            bool first = true;
            foreach (var s in model) {
                if (first)
                    first = false;
                else
                    hb.Append(delim);
                hb.Append(YetaWFManager.HtmlEncode(s));
            }
            hb.Append(@"
</div>");

            return Task.FromResult(hb.ToYHtmlString());
        }
    }
}
