using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class UrlDesignedPageComponentBase : YetaWFComponent {

        public const string TemplateName = "UrlDesignedPage";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    public class UrlDesignedPageEditComponent : UrlDesignedPageComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(string model) {

            List<string> pages = await PageDefinition.GetDesignedUrlsAsync();

            // get list of desired pages (ignore users that are invalid, they may have been deleted)
            List<SelectionItem<string>> list = new List<SelectionItem<string>>();
            foreach (var page in pages) {
                list.Add(new SelectionItem<string> {
                    Text = page,
                    //Tooltip = __ResStr("selPage", "Select page {0}", page),
                    Value = page,
                });
            }
            list = (from l in list orderby l.Text select l).ToList();
            list.Insert(0, new SelectionItem<string> { Text = this.__ResStr("select", "(select)"), Value = "" });

            return await DropDownListComponent.RenderDropDownListAsync(model, list, this, "yt_urldesignedpage");
        }
    }
}
