/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Modules;

namespace YetaWF.Modules.Panels.Models {

    public class SplitterInfo {

        public int Height { get; set; }
        public Guid ModuleLeft { get; set; }
        public int MinWidth { get; set; }// pixels
        public int Width { get; set; }// percentage
        public Guid ModuleRight { get; set; }

        public string TitleText { get; set; }
        public string TitleTooltip { get; set; }
        public string CollapseText { get; set; }
        public string CollapseToolTip { get; set; }
        public string ExpandToolTip { get; set; }

        public SplitterInfo() { }

        public async Task<ModuleDefinition> GetModuleLeftAsync() {
            if (_moduleLeftDef == null) {
                _moduleLeftDef = NoModule;
                ModuleDefinition mod = await ModuleDefinition.LoadAsync(ModuleLeft, AllowNone: true);
                if (mod != null)
                    _moduleLeftDef = mod;
            }
            if (_moduleLeftDef == NoModule) return null;
            return _moduleLeftDef;
        }
        private ModuleDefinition _moduleLeftDef = null;

        public async Task<ModuleDefinition> GetModuleRightAsync() {
            if (_moduleRightDef == null) {
                _moduleRightDef = NoModule;
                ModuleDefinition mod = await ModuleDefinition.LoadAsync(ModuleRight, AllowNone: true);
                if (mod != null)
                    _moduleRightDef = mod;
            }
            if (_moduleRightDef == NoModule) return null;
            return _moduleRightDef;
        }
        private ModuleDefinition _moduleRightDef = null;

        private static readonly ModuleDefinition NoModule = new ModuleDefinition();

        public async Task<bool> IsAuthorizedLeftAsync() {
            ModuleDefinition mod = await GetModuleLeftAsync();
            if (mod == null) return true;
            return mod.IsAuthorized(null);
        }
        public async Task<bool> IsAuthorizedRightAsync() {
            ModuleDefinition mod = await GetModuleRightAsync();
            if (mod == null) return true;
            return mod.IsAuthorized(null);
        }
    }
}
