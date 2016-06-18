/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core.Addons;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Views;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Views.Shared {

    public class RoleId<TModel> : RazorTemplate<TModel> { }

    public static class RoleIdHelper {

        public static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(RoleIdHelper), name, defaultValue, parms); }

        public static MvcHtmlString RenderRoleIdDisplay(this HtmlHelper htmlHelper, string name, int model, object HtmlAttributes = null) {
            TagBuilder tag = new TagBuilder("span");
            htmlHelper.FieldSetup(tag, name, HtmlAttributes: HtmlAttributes, Validation: false, Anonymous: true);

            using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
                RoleDefinition role = dataProvider.GetRoleById(model);
                tag.SetInnerText(role.Name);
                tag.Attributes.Add(Basics.CssTooltipSpan, role.Description);
                return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
            }
        }

        public static MvcHtmlString RenderRoleId(this HtmlHelper htmlHelper, string name, int model) {

            List<SelectionItem<int>> list;
            list = (
                from RoleInfo role in Resource.ResourceAccess.GetDefaultRoleList()
                orderby role.Name
                select
                    new SelectionItem<int> {
                        Text = role.Name,
                        Value = role.RoleId,
                        Tooltip = role.Description,
                    }).ToList<SelectionItem<int>>();

            bool showDefault = htmlHelper.GetControlInfo<bool>(name, "ShowDefault", true);
            if (model == 0 || showDefault)
                list.Insert(0, new SelectionItem<int> { Text = __ResStr("select", "(none)"), Value = 0 });

            return htmlHelper.RenderDropDownSelectionList<int>(name, model, list);
        }
    }
}