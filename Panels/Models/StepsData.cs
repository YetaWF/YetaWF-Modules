/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Panels.Models {

    [TemplateAction(TemplateName)]
    public class StepInfo : ITemplateAction {

        public const string TemplateName = "YetaWF_Panels_StepInfo";

        public class StepEntry {

            public const int MaxName = 80;
            public const int MaxCaption = 40;
            public const int MaxDescription = 80;

            [Caption("Name"), Description("The name for this step - This name is used by other modules for notification purposes - Modules that support displaying steps have standardized names for steps, which should be documented in their documentation")]
            [UIHint("Text40"), StringLength(MaxName), Required, Trim]
            public string Name { get; set; }

            [Caption("Caption"), Description("The caption for this step")]
            [UIHint("MultiString40"), StringLength(MaxCaption), Required, Trim]
            public MultiString Caption { get; set; }
            [Caption("Description"), Description("The description for this step")]
            [UIHint("MultiString40"), StringLength(MaxDescription), Required, Trim]
            public MultiString Description { get; set; }

            public StepEntry() {
                Caption = new MultiString();
                Description = new MultiString();
            }
        }

        public StepInfo() {
            _ActiveTab = 0;

            Steps = new SerializableList<StepEntry> {
                new StepEntry {
                    Caption = this.__ResStr("dfltCaption", "(Caption)"),
                    Description= this.__ResStr("dfltDesription", "(Description)"),
                },
            };
        }

        [Data_Binary]
        public SerializableList<StepEntry> Steps { get; set; }

        public void Saving(string propertyName, Guid moduleGuid) { }

        [DontSave]
        public int _ActiveTab { get; set; }

        public bool ExecuteAction(int action, bool modelIsValid, object extraData) {
            bool processed = false;
            PanelInfo.PanelAction stepAction = (PanelInfo.PanelAction)action;
            int panelIndex = Convert.ToInt32((string)extraData);
            switch (stepAction) {
                case PanelInfo.PanelAction.Apply:
                    if (modelIsValid)
                        processed = true;
                    break;
                case PanelInfo.PanelAction.Insert:
                    if (modelIsValid) {
                        InsertPanel(panelIndex);
                        processed = true;
                    }
                    break;
                case PanelInfo.PanelAction.Add:
                    if (modelIsValid) {
                        AddPanel(panelIndex);
                        processed = true;
                    }
                    break;
                case PanelInfo.PanelAction.Remove:
                    RemovePanel(panelIndex);
                    processed = true;
                    break;
                case PanelInfo.PanelAction.MoveLeft:
                    if (modelIsValid) {
                        MovePanelLeft(panelIndex);
                        processed = true;
                    }
                    break;
                case PanelInfo.PanelAction.MoveRight:
                    if (modelIsValid) {
                        MovePanelRight(panelIndex);
                        processed = true;
                    }
                    break;
                default:
                    throw new InternalError("Invalid action {0}", stepAction);
            }
            _ActiveTab = Math.Min(Steps.Count - 1, Math.Max(0, _ActiveTab));
            return processed;
        }
        private void InsertPanel(int panelIndex) {
            Steps.Insert(panelIndex, new StepEntry());
            _ActiveTab = panelIndex;
        }
        private void AddPanel(int panelIndex) {
            Steps.Insert(panelIndex + 1, new StepEntry());
            _ActiveTab = panelIndex + 1;
        }
        private void RemovePanel(int panelIndex) {
            Steps.RemoveAt(panelIndex);
            _ActiveTab = panelIndex;
        }
        private void MovePanelLeft(int panelIndex) {
            if (panelIndex <= 0) throw new InternalError("Invalid panel index");
            StepEntry panel = Steps[panelIndex];
            Steps.RemoveAt(panelIndex);
            Steps.Insert(panelIndex - 1, panel);
            _ActiveTab = panelIndex - 1;
        }
        private void MovePanelRight(int panelIndex) {
            if (panelIndex >= Steps.Count - 1) throw new InternalError("Invalid panel index");
            StepEntry panel = Steps[panelIndex];
            Steps.RemoveAt(panelIndex);
            Steps.Insert(panelIndex + 1, panel);
            _ActiveTab = panelIndex + 1;
        }
    }
}
