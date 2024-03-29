/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Blog.Modules {
    public class CommentAddModuleDataProvider : ModuleDefinitionDataProvider<Guid, CommentAddModule>, IInstallableModel { }

    [ModuleGuid("{07c08323-3153-474c-a974-8f5c168eef8f}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    [ModuleCategory("Comments")]
    public class CommentAddModule : ModuleDefinition {

        public CommentAddModule() {
            Title = this.__ResStr("modTitle", "Add New Comment");
            Name = this.__ResStr("modName", "Add Comment");
            Description = this.__ResStr("modSummary", "Adds a new comment. The comment is added the blog entry defined by the page URL's BlogEntry argument. This module is typically used on the same page as the Entry Display Module.");
            Print = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new CommentAddModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}

