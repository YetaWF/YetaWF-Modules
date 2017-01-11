/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

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
    public class CommentAddModule : ModuleDefinition {

        public CommentAddModule() {
            Title = this.__ResStr("modTitle", "Add New Comment");
            Name = this.__ResStr("modName", "Add Comment");
            Description = this.__ResStr("modSummary", "Adds a new comment");
            Print = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new CommentAddModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}

