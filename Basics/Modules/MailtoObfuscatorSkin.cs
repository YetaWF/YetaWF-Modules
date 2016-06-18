﻿/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Basics#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Basics.Modules {

    public class MailtoObfuscatorSkinModuleDataProvider : ModuleDefinitionDataProvider<Guid, MailtoObfuscatorSkinModule>, IInstallableModel { }

    [ModuleGuid("{749d0ca9-75e5-40b8-82e3-466a11d3b1d2}")] // Publiushed Guid
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class MailtoObfuscatorSkinModule : ModuleDefinition {

        public MailtoObfuscatorSkinModule() {
            Title = this.__ResStr("modTitle", "Mailto Obfuscator");
            Name = this.__ResStr("modName", "Mailto Obfuscator (Skin)");
            Description = this.__ResStr("modSummary", "Skin module implementing mailto obfuscation using javascript");
            WantFocus = false;
            WantSearch = false;
            ShowTitle = false;
            Invokable = true;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new MailtoObfuscatorSkinModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    }
}