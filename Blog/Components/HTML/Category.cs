/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Blog.Components {

    public abstract class CategoryComponentBase : YetaWFComponent {

        public const string TemplateName = "Category";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    /// <summary>
    /// Allows selection of a blog category by displaying a dropdown list of all available blog categories. The model is the category ID.
    /// </summary>
    /// <example>
    /// [Category("Blog"), Caption("Default Blog Category"), Description("The default blog category displayed when no blog category is selected")]
    /// [UIHint("YetaWF_Blog_Category"), AdditionalMetadata("ShowAll", true), Required]
    /// public int DefaultCategory { get; set; }
    /// </example>
    [UsesAdditional("ShowAll", "bool", "false", "Defines whether an additional entry \"(All)\" is added as the first entry with a value of 0. Only one of ShowAll, ShowSelectIfNone, ShowSelect should be specified.")]
    [UsesAdditional("ShowSelectIfNone", "bool", "false", "Defines whether an additional entry \"(select)\" is added as the first entry with a value of 0 if model's category ID is 0. Only one of ShowAll, ShowSelectIfNone, ShowSelect should be specified.")]
    [UsesAdditional("ShowSelect", "bool", "false", "Defines whether an additional entry \"(select)\" is added as the first entry with a value of 0. Only one of ShowAll, ShowSelectIfNone, ShowSelect should be specified.")]
    public class CategoryEditComponent : CategoryComponentBase, IYetaWFComponent<int> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<string> RenderAsync(int model) {

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
                            Text = this.__ResStr("select", "(select)"),
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
