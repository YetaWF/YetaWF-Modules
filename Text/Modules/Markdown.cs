/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Text#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Search;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.ComponentsHTML.Components;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Text.Modules {

    public class MarkdownModuleDataProvider : ModuleDefinitionDataProvider<Guid, MarkdownModule>, IInstallableModel { }

    public class MarkdownString : MarkdownStringBase {
        [StringLength(0)]
        public override string Text { get { return base.Text; } set { base.Text = value; } }
        [StringLength(0)]
        public override string HTML { get { return base.HTML; } set { base.HTML = value; } }
    }

    [ModuleGuid("{5EAF62EB-9B05-45a1-A530-4A721D2F1C33}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class MarkdownModule : ModuleDefinition {

        public const int MaxContents = 1024 * 1024;

        public MarkdownModule() : base() {
            Title = this.__ResStr("modTitle", "Edit");
            Name = this.__ResStr("modName", "Text (Markdown)");
            Description = this.__ResStr("modSummary", "Displays user editable text contents (using Markdown)");
            Contents = new MarkdownString();
        }

        public override IModuleDefinitionIO GetDataProvider() { return new MarkdownModuleDataProvider(); }

        public MarkdownString Contents { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return EditorLevel_DefaultAllowedRoles; } }

        // SEARCH
        // SEARCH
        // SEARCH

        public override void CustomSearch(ISearchWords searchWords) {
            searchWords.AddObjectContents(Contents);
        }
    }
}