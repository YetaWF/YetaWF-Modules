/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Blog.Components {

    public abstract class CategoryComponentBase : YetaWFComponent {

        public const string TemplateName = "Category";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    public class CategoryDisplayComponent : CategoryComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(string model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"{(string.IsNullOrEmpty(model) ? "&nbsp;" : YetaWFManager.HtmlEncode(model))}");

            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public class CategoryEditComponent : CategoryComponentBase, IYetaWFComponent<int> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(int model) {

            using (BlogCategoryDataProvider categoryDP = new BlogCategoryDataProvider()) {
                DataProviderGetRecords<BlogCategory> data = await categoryDP.GetItemsAsync(0, 0, null, null);
                List<SelectionItem<int>> list = (from c in data.Data orderby c.Category.ToString() select new SelectionItem<int> {
                    Text = c.Category.ToString(),
                    Tooltip = c.Description,
                    Value = c.Identity
                }).ToList();

                if (list.Count == 0) {
                    list.Insert(0, new SelectionItem<int> {
                        Text = this.__ResStr("none", "(None Available)"),
                        Tooltip = this.__ResStr("noneTT", "There are no blog categories"),
                        Value = 0,
                    });
                } else {
                    bool showSelect = false;
                    bool showAll = PropData.GetAdditionalAttributeValue("ShowAll", false);
                    if (model == 0 && !showAll)
                        showSelect = PropData.GetAdditionalAttributeValue("ShowSelectIfNone", false);
                    if (!showAll && !showSelect)
                        showSelect = PropData.GetAdditionalAttributeValue("ShowSelect", false);
                    if (showAll) {
                        list.Insert(0, new SelectionItem<int> {
                            Text = this.__ResStr("all", "(All)"),
                            Tooltip = this.__ResStr("allTT", "Displays blogs from all available blog categories"),
                            Value = 0,
                        });
                    } else if (showSelect) {
                        list.Insert(0, new SelectionItem<int> {
                            Text = this.__ResStr("select", "(Select)"),
                            Tooltip = this.__ResStr("selectTT", "Please select one of the available blog categories"),
                            Value = 0,
                        });
                    }
                }
                return await DropDownListIntComponent.RenderDropDownListAsync(this, model, list, "yt_yetawf_blog_category");
            }
        }
    }
}
