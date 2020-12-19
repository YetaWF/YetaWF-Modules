/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Components {

    public abstract class RoleIdComponent : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(RoleIdComponent), name, defaultValue, parms); }

        public const string TemplateName = "RoleId";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// Displays the model's role ID as a role name with a tooltip describing the role.
    /// </summary>
    /// <remarks>
    /// For information about roles in YetaWF, see the Authorization topic.
    /// </remarks>
    /// <example>
    /// [Caption("Limit To Role"), Description("Defines which role must be present for this action to be shown")]
    /// [UIHint("YetaWF_Identity_RoleId"), ReadOnly]
    /// public int LimitToRole { get; set; }
    /// </example>
    public class RoleIdDisplayComponent : RoleIdComponent, IYetaWFComponent<int> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<string> RenderAsync(int model) {

            YTagBuilder tag = new YTagBuilder("span");
            FieldSetup(tag, FieldType.Anonymous);

            using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
                RoleDefinition role = dataProvider.GetRoleById(model);
                tag.SetInnerText(role.Name);
                tag.Attributes.Add(Basics.CssTooltipSpan, role.Description);
            }

            return Task.FromResult(tag.ToString(YTagRenderMode.Normal));
        }
    }

    /// <summary>
    /// Allows selection of a role. The model represents a role ID.
    /// </summary>
    /// <remarks>
    /// For information about roles in YetaWF, see the Authorization topic.
    /// </remarks>
    /// <example>
    /// [Caption("Limit To Role"), Description("Defines which role must be present for this action to be shown")]
    /// [UIHint("YetaWF_Identity_RoleId"), AdditionalMetadata("ShowDefault", true)]
    /// public int LimitToRole { get; set; }
    /// </example>
    [UsesAdditional("ShowDefault", "bool", "true", "Defines whether a \"(none)\" entry is added with a value of 0.")]
    public class RoleIdEditComponent : RoleIdComponent, IYetaWFComponent<int> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<string> RenderAsync(int model) {

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

            bool showDefault = PropData.GetAdditionalAttributeValue("ShowDefault", true);
            if (model == 0 || showDefault)
                list.Insert(0, new SelectionItem<int> { Text = __ResStr("select", "(none)"), Value = 0 });

            return await DropDownListIntComponent.RenderDropDownListAsync(this, model, list, "yt_yetawf_identity_roleid");
        }
    }
}
