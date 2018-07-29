/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract partial class PropertyListComponentBase {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(PropertyListComponentBase), name, defaultValue, parms); }

        private static void RenderHeader(HtmlBuilder hb, ClassData classData) {
            if (!string.IsNullOrWhiteSpace(classData.Header)) {
                hb.Append("<div class='y_header'>");
                if (classData.Header.StartsWith("-"))
                    hb.Append(classData.Header.Substring(1));
                else
                    hb.Append(YetaWFManager.HtmlEncode(classData.Header));
                hb.Append("</div>");
            }
        }
        private static void RenderFooter(HtmlBuilder hb, ClassData classData) {
            if (!string.IsNullOrWhiteSpace(classData.Footer)) {
                hb.Append("<div class='y_footer'>");
                if (classData.Footer.StartsWith("-"))
                    hb.Append(classData.Footer.Substring(1));
                else
                    hb.Append(YetaWFManager.HtmlEncode(classData.Footer));
                hb.Append("</div>");
            }
        }
        private async Task<YHtmlString> RenderHiddenAsync(object model) {
            HtmlBuilder hb = new HtmlBuilder();
            List<PropertyListEntry> properties = GetHiddenProperties(model);
            foreach (var property in properties) {
                hb.Append(await HtmlHelper.ForEditAsync(model, property.Name));// hidden fields are edit by definition otherwise they're useless
            }
            return hb.ToYHtmlString();
        }
        private async Task<YHtmlString> RenderListAsync(object model, string category, bool showVariables, bool readOnly) {

            bool focusSet = Manager.WantFocus ? false : true;
            List<PropertyListEntry> properties = GetPropertiesByCategory(model, category);
            HtmlBuilder hb = new HtmlBuilder();
            ScriptBuilder sb = new ScriptBuilder();

            foreach (PropertyListEntry property in properties) {
                bool haveValue = false;
                YHtmlString shtml = null;
                if (property.Restricted) {
                    shtml = new YHtmlString(__ResStr("demo", "This property is not available in Demo Mode"));
                    haveValue = true;
                } else if (readOnly || !property.Editable) {
                    shtml = new YHtmlString((await HtmlHelper.ForDisplayAsync(model, property.Name)).ToString());
                    string s = shtml.ToString().Trim();
                    if (string.IsNullOrWhiteSpace(s)) {
                        if (property.SuppressEmpty)
                            continue;
                        shtml = new YHtmlString("&nbsp;");
                    } else
                        haveValue = true;
                } else {
                    shtml = new YHtmlString(await HtmlHelper.ForEditAsync(model, property.Name));
                    haveValue = true;
                }

                hb.Append("<div class='t_row t_{0}'>", property.Name.ToLower());

                bool labelDone = false;
                if (!string.IsNullOrWhiteSpace(property.TextAbove)) {
                    YHtmlString hs = new YHtmlString((await HtmlHelper.ForLabelAsync(model, property.Name, ShowVariable: showVariables, SuppressIfEmpty: true)).ToString());
                    if (!string.IsNullOrWhiteSpace(hs.ToString())) {
                        labelDone = true;
                        hb.Append("<div class='t_labels'>");
                        hb.Append(hs);
                        hb.Append("</div>");
                    }
                    hb.Append("<div class='t_vals t_textabove'>");
                    if (property.TextAbove.StartsWith("-"))
                        hb.Append(property.TextAbove.Substring(1));
                    else
                        hb.Append(YetaWFManager.HtmlEncode(property.TextAbove));
                    hb.Append("</div>");
                }
                if (haveValue || string.IsNullOrWhiteSpace(property.TextAbove)) {
                    if (labelDone) {
                        hb.Append("<div class='t_labels t_fillerabove'>&nbsp;</div>");
                    } else {
                        YHtmlString hs = new YHtmlString((await HtmlHelper.ForLabelAsync(model, property.Name, ShowVariable: showVariables, SuppressIfEmpty: true)).ToString());
                        if (!string.IsNullOrWhiteSpace(hs.ToString())) {
                            hb.Append("<div class='t_labels'>");
                            hb.Append(hs);
                            hb.Append("</div>");
                            labelDone = true;
                        }
                    }
                    if (!readOnly && property.Editable && !property.Restricted) {
                        string cls = "t_vals" + (!focusSet ? " focusonme" : "");
                        switch (property.SubmitType) {
                            default:
                            case SubmitFormOnChangeAttribute.SubmitTypeEnum.None:
                                break;
                            case SubmitFormOnChangeAttribute.SubmitTypeEnum.Submit:
                                cls += " ysubmitonchange";
                                break;
                            case SubmitFormOnChangeAttribute.SubmitTypeEnum.Apply:
                                cls += " yapplyonchange";
                                break;
                        }
                        focusSet = true;
                        hb.Append("<div class='{0}'>", cls);
                        hb.Append(shtml);
                        hb.Append(ValidationMessage(property.Name));
                        hb.Append("</div>");
                    } else {
                        hb.Append("<div class='t_vals t_val'>");
                        hb.Append(shtml);
                        hb.Append("</div>");
                    }
                }

                if (!string.IsNullOrWhiteSpace(property.TextBelow)) {
                    if (labelDone)
                        hb.Append("<div class='t_labels t_fillerbelow'>&nbsp;</div>");

                    hb.Append("<div class='t_vals t_textbelow'>");
                    if (property.TextBelow.StartsWith("-"))
                        hb.Append(property.TextBelow.Substring(1));
                    else
                        hb.Append(YetaWFManager.HtmlEncode(property.TextBelow));
                    hb.Append("</div>");
                }

                hb.Append("</div>");
            }
            return hb.ToYHtmlString();
        }
    }
}
