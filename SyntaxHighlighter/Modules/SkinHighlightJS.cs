/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.SyntaxHighlighter.Modules {

    public class SkinHighlightJSModuleDataProvider : ModuleDefinitionDataProvider<Guid, SkinHighlightJSModule>, IInstallableModel { }

    [ModuleGuid("{25068AC6-BA74-4644-8B46-9D7FEC291E45}")] // Published Guid
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SkinHighlightJSModule : ModuleDefinition {

        public SkinHighlightJSModule() {
            Title = this.__ResStr("modTitle", "Skin Syntax Highlighter (Highlight.js)");
            Name = this.__ResStr("modName", "Syntax Highlighter Highlight.js (Skin)");
            Description = this.__ResStr("modSummary", "Skin module supporting syntax highlighting in modules, Highlight.js");
            WantFocus = false;
            ShowTitle = false;
            WantSearch = false;
            Invokable = true;
            InvokeInPopup = true;
            InvokeInAjax = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SkinHighlightJSModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
    }
}