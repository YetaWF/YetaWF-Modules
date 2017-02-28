﻿/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Addons;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Views;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Identity.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Views.Shared {

    public class RoleId<TModel> : RazorTemplate<TModel> { }

    public static class RoleIdHelper {

        public static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(RoleIdHelper), name, defaultValue, parms); }
#if MVC6
        public static HtmlString RenderRoleIdDisplay(this IHtmlHelper htmlHelper, string name, int model, object HtmlAttributes = null)
#else
        public static HtmlString RenderRoleIdDisplay(this HtmlHelper htmlHelper, string name, int model, object HtmlAttributes = null)
#endif
        {
            TagBuilder tag = new TagBuilder("span");
            htmlHelper.FieldSetup(tag, name, HtmlAttributes: HtmlAttributes, Validation: false, Anonymous: true);

            using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
                RoleDefinition role = dataProvider.GetRoleById(model);
                tag.SetInnerText(role.Name);
                tag.Attributes.Add(Basics.CssTooltipSpan, role.Description);
                return tag.ToHtmlString(TagRenderMode.Normal);
            }
        }
#if MVC6
        public static HtmlString RenderRoleId(this IHtmlHelper htmlHelper, string name, int model, object HtmlAttributes = null)
#else
        public static HtmlString RenderRoleId(this HtmlHelper htmlHelper, string name, int model, object HtmlAttributes = null)
#endif
        {
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

            return htmlHelper.RenderDropDownSelectionList<int>(name, model, list, HtmlAttributes: HtmlAttributes);
        }
    }
}