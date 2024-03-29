/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Panels.Models {

    [TemplateAction(TemplateName)]
    public class PanelInfo : ITemplateAction {

        public const string TemplateName = "YetaWF_Panels_PanelInfo";

        public enum PanelAction {
            Apply = 0,
            MoveLeft = 1,
            MoveRight = 2,
            Add = 3,
            Insert = 4,
            Remove = 5,
        }
        public enum PanelStyleEnum {
            [EnumDescription("Tabs","Tabs are used to switch between panels")]
            Tabs = 0,
            [EnumDescription("Accordion", "An accordion-style display is used to switch between panels")]
            Accordion = 1,
        }
        public class PanelEntry {

            public const int MaxCaption = 100;
            public const int MaxToolTip = 200;

            [Caption("Module"), Description("The module rendered within this slot")]
            [UIHint("ModuleSelection"), AdditionalMetadata("New", false), Required]
            public Guid Module { get; set; }

            [Caption("Caption"), Description("The caption for this module")]
            [UIHint("MultiString40"), StringLength(MaxCaption), Required, Trim]
            public MultiString Caption { get; set; }

            [Caption("ToolTip"), Description("The optional ToolTip for this module")]
            [UIHint("MultiString80"), StringLength(MaxToolTip), Trim]
            public MultiString ToolTip { get; set; }

            public async Task<ModuleDefinition?> GetModuleAsync() {
                if (_moduleDef == null) {
                    _moduleDef = NoModule;
                    ModuleDefinition? mod = await ModuleDefinition.LoadAsync(Module, AllowNone: true);
                    if (mod != null)
                        _moduleDef = mod;
                }
                if (_moduleDef == NoModule) return null;
                return _moduleDef;
            }
            private ModuleDefinition? _moduleDef = null;
            private static readonly ModuleDefinition NoModule = new ModuleDefinition();

            public async Task<bool> IsAuthorizedAsync() {
                ModuleDefinition? mod = await GetModuleAsync();
                if (mod == null) return false;
                return mod.IsAuthorized(null);
            }

            public PanelEntry() {
                Module = Guid.Empty;
                Caption = new MultiString();
                ToolTip = new MultiString();
            }
        }

        public PanelInfo() {
            Style = PanelStyleEnum.Tabs;
            _ActiveTab = 0;

            Panels = new SerializableList<PanelEntry> {
                new PanelEntry {
                    Caption = this.__ResStr("dfltCaption", "(Caption)"),
                    ToolTip = this.__ResStr("dfltTT", "(ToolTip)"),
                    Module = Guid.Empty,
                },
            };
        }
        public void Saving(string propertyName, Guid moduleGuid) { }

        [Category("Panels"), Caption("Style"), Description("Defines the visual style of the panels used in Site Display Mode")]
        [UIHint("Enum")]
        public PanelStyleEnum Style { get; set; }

        [Data_Binary]
        public SerializableList<PanelEntry> Panels { get; set; }

        [DontSave]
        public int _ActiveTab { get; set; }

        public bool ExecuteAction(int action, bool modelIsValid, object extraData) {
            bool processed = false;
            PanelAction panelAction = (PanelAction)action;
            int panelIndex = Convert.ToInt32((string) extraData);
            switch (panelAction) {
                case PanelAction.Apply:
                    if (modelIsValid)
                        processed = true;
                    break;
                case PanelAction.Insert:
                    if (modelIsValid) {
                        InsertPanel(panelIndex);
                        processed = true;
                    }
                    break;
                case PanelAction.Add:
                    if (modelIsValid) {
                        AddPanel(panelIndex);
                        processed = true;
                    }
                    break;
                case PanelAction.Remove:
                    RemovePanel(panelIndex);
                    processed = true;
                    break;
                case PanelAction.MoveLeft:
                    if (modelIsValid) {
                        MovePanelLeft(panelIndex);
                        processed = true;
                    }
                    break;
                case PanelAction.MoveRight:
                    if (modelIsValid) {
                        MovePanelRight(panelIndex);
                        processed = true;
                    }
                    break;
                default:
                    throw new InternalError("Invalid action {0}", panelAction);
            }
            _ActiveTab = Math.Min(Panels.Count - 1, Math.Max(0, _ActiveTab));
            return processed;
        }
        private void InsertPanel(int panelIndex) {
            Panels.Insert(panelIndex, new PanelEntry());
            _ActiveTab = panelIndex;
        }
        private void AddPanel(int panelIndex) {
            Panels.Insert(panelIndex+1, new PanelEntry());
            _ActiveTab = panelIndex+1;
        }
        private void RemovePanel(int panelIndex) {
            Panels.RemoveAt(panelIndex);
            _ActiveTab = panelIndex;
        }
        private void MovePanelLeft(int panelIndex) {
            if (panelIndex <= 0) throw new InternalError("Invalid panel index");
            PanelEntry panel = Panels[panelIndex];
            Panels.RemoveAt(panelIndex);
            Panels.Insert(panelIndex - 1, panel);
            _ActiveTab = panelIndex-1;
        }
        private void MovePanelRight(int panelIndex) {
            if (panelIndex >= Panels.Count-1) throw new InternalError("Invalid panel index");
            PanelEntry panel = Panels[panelIndex];
            Panels.RemoveAt(panelIndex);
            Panels.Insert(panelIndex + 1, panel);
            _ActiveTab = panelIndex+1;
        }
    }
}
