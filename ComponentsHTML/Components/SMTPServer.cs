/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.SendEmail;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the SMTPServer edit and display components.
    /// </summary>
    public abstract class SMTPServerComponentBase : YetaWFComponent {

        internal const string TemplateName = "SMTPServer";

        /// <summary>
        /// Returns the package implementing the component.
        /// </summary>
        /// <returns>Returns the package implementing the component.</returns>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }

        /// <summary>
        /// Returns the component name.
        /// </summary>
        /// <returns>Returns the component name.</returns>
        /// <remarks>Components in packages whose product name starts with "Component" use the exact name returned by GetTemplateName when used in UIHint attributes. These are considered core components.
        /// Components in other packages use the package's area name as a prefix. E.g., the UserId component in the YetaWF.Identity package is named "YetaWF_Identity_UserId" when used in UIHint attributes.
        ///
        /// The GetTemplateName method returns the component name without area name prefix in all cases.</remarks>
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// Displays email server information based on the model. The model defines an SMTP server used to send emails.
    /// </summary>
    /// <example>
    /// [Category("Email"), Caption("Email Server"), Description("The email server used to send emails from this site")]
    /// [UIHint("SMTPServer"), ReadOnly]
    /// public SMTPServer SMTP { get; set; }
    /// </example>
    public class SMTPServerComponentDisplay : SMTPServerComponentBase, IYetaWFComponent<SMTPServer> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(SMTPServer model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(HE(model.Server));

            return Task.FromResult(hb.ToString());
        }
    }

    /// <summary>
    /// Allows entry of email server information. The model defines an SMTP server used to send emails.
    /// Supports sending a test email before the data is saved.
    /// </summary>
    /// <example>
    /// [Category("Email"), Caption("Email Server"), Description("The email server used to send emails from this site")]
    /// [UIHint("SMTPServer")]
    /// public SMTPServer SMTP { get; set; }
    /// </example>
    public class SMTPServerComponentEdit : SMTPServerComponentBase, IYetaWFComponent<SMTPServer> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(SMTPServer model) {

            using (Manager.StartNestedComponent(FieldName)) {

                HtmlBuilder hb = new HtmlBuilder();
                hb.Append($@"
<div id='{DivId}' class='yt_smtpserver t_edit'>
    {await HtmlHelper.ForEditContainerAsync(model, "PropertyList")}
</div>");

                Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.SMTPServerEdit('{DivId}');");

                return hb.ToString();
            }
        }
    }
}
