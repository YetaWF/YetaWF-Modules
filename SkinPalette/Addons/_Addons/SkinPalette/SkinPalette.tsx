/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SkinPalette#License */

namespace YetaWF_SkinPalette {

    interface Setup {
        Properties: Entry[];
    }
    interface Entry {
        CSSVarName: string;
        ModelName: string;
        UIHint: string;
    }

    export class SkinPaletteModule extends YetaWF.ModuleBaseDataImpl {

        public static readonly SELECTOR: string = ".YetaWF_SkinPalette_SkinPalette";

        private Setup: Setup;
        private ExpandCollapse: HTMLElement;
        private Tabs: YetaWF_ComponentsHTML.TabsComponent;
        private Contents: HTMLElement;

        private Config: HTMLTextAreaElement;
        private ConfigApply: HTMLElement;

        constructor(id: string, setup: Setup) {
            super(id, SkinPaletteModule.SELECTOR, null);
            this.Setup = setup;

            this.ExpandCollapse = $YetaWF.getElement1BySelector(".t_palette", [this.Module]);
            this.Tabs = YetaWF_ComponentsHTML.TabsComponent.getControlFromSelector(".yt_tabs", YetaWF_ComponentsHTML.TabsComponent.SELECTOR, [this.Module]);
            this.Contents = $YetaWF.getElement1BySelector(".t_contents", [this.Module]);
            this.Config = $YetaWF.getElement1BySelector("textarea[name='CSSVariables']", [this.Module]) as HTMLTextAreaElement;
            this.ConfigApply = $YetaWF.getElement1BySelector("a[data-name='Apply']", [this.Module]);

            this.populate();

            $YetaWF.registerEventHandler(this.ExpandCollapse, "click", null, (ev: MouseEvent): boolean => {
                if (this.Contents.style.display === "") {
                    this.Contents.style.display = "none";
                } else {
                    this.Contents.style.display = "";
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.ConfigApply, "click", null, (ev: MouseEvent): boolean => {
                this.updateSkin();
                return false;
            });
            this.Tabs.Control.addEventListener(YetaWF_ComponentsHTML.TabsComponent.EVENTSWITCHED, (evt: Event): void => {
                this.updateConfig();
            });
        }
        private populate(): void {
            YetaWF_ComponentsHTML_Validation.clearValidation(this.Module);
            let root = document.documentElement;
            let styles = getComputedStyle(root);
            for (let entry of this.Setup.Properties) {
                let val = styles.getPropertyValue(entry.CSSVarName);
                val = val.trim();
                switch (entry.UIHint) {
                    case "Color":
                        let clrControl:YetaWF_ComponentsHTML.ColorEditComponent = YetaWF_ComponentsHTML.ColorEditComponent.getControlFromSelector(`input[type="text"][name="${entry.ModelName}"]`,YetaWF_ComponentsHTML.ColorEditComponent.SELECTOR, [this.Module]);
                        clrControl.value = val;
                        clrControl.Control.addEventListener(YetaWF_ComponentsHTML.ColorEditComponent.EVENTCHANGE, (evt: Event): void => {
                            root.style.setProperty(entry.CSSVarName, clrControl.value);
                        });
                        break;
                    default:
                        let elem = $YetaWF.getElement1BySelector(`input[type="text"][name="${entry.ModelName}"]`, [this.Module]) as HTMLInputElement;
                        elem.value = val;
                        $YetaWF.registerEventHandler(elem, "change", null, (ev: Event):boolean =>{
                            root.style.setProperty(entry.CSSVarName, elem.value);
                            return true;
                        });
                        break;
                }
            }
        }
        private updateConfig(): void {
            let text = ":root {\n";
            let root = document.documentElement;
            let styles = getComputedStyle(root);
            for (let entry of this.Setup.Properties) {
                let val = styles.getPropertyValue(entry.CSSVarName);
                val = val.trim();
                text += `   ${entry.CSSVarName}: ${val};\n`;
            }
            text = text + "}\n";
            this.Config.value = text;
        }
        private updateSkin(): void {
            // add a dummy div with new --vars and retrieve the new styles
            let div =
                <div id="YetaWF_SkinPalette_Styles">
                    <style></style>
                    <div class='t_active'></div>
                </div> as HTMLElement;
            let styleElem = $YetaWF.getElement1BySelector("style", [div]);
            let text = this.Config.value;
            text = text.replace(":root", "#YetaWF_SkinPalette_Styles .t_active");// we're replacing :root with the specific id
            styleElem.innerHTML = text;

            let styledElem = $YetaWF.getElement1BySelector(".t_active", [div]);
            document.body.appendChild(div);

            // extract all style values from style section and update root
            let root = document.documentElement;
            let styles = getComputedStyle(styledElem);
            for (let entry of this.Setup.Properties) {
                let val = styles.getPropertyValue(entry.CSSVarName);
                root.style.setProperty(entry.CSSVarName, val);
            }

            // remove style element
            div.remove();
            // update all fields with changes
            this.populate();
        }
    }
}
