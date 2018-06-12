using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Components {

    public abstract class RoleIdComponent : YetaWFComponent {

        public const string TemplateName = "RoleId";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    public class RoleIdDisplayComponent : RoleIdComponent, IYetaWFComponent<int> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(int model) {

            YTagBuilder tag = new YTagBuilder("span");
            FieldSetup(tag, FieldType.Anonymous);

            using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
                RoleDefinition role = dataProvider.GetRoleById(model);
                tag.SetInnerText(role.Name);
                tag.Attributes.Add(Basics.CssTooltipSpan, role.Description);
            }

            return Task.FromResult(tag.ToYHtmlString(YTagRenderMode.Normal));
        }
    }
    public class RoleIdEditComponent : RoleIdComponent, IYetaWFComponent<int> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(int model) {

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
                list.Insert(0, new SelectionItem<int> { Text = this.__ResStr("select", "(none)"), Value = 0 });

            return await DropDownListEditComponentBase<int>.RenderDropDownListAsync(this, model, list, "yt_yetawf_identity_roleid");
        }
    }
}
