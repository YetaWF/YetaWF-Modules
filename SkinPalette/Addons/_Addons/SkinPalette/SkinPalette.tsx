/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SkinPalette#License */

namespace YetaWF_SkinPalette {

    interface Setup {
        Properties: Entry[];
    }
    interface Entry {
        CSSVarName: string;
        ModelName: string;
        UIHint: string;
    }
    enum BasicThemeEnum {
        Light = 0,
        Dark = 1
    }
    interface GenValues {
        GenTheme: BasicThemeEnum;
        GenBg: string;
        GenClr: string;
        GenBgActive: string;
        GenClrActive: string;
        GenBgShaded: string;
        GenClrShaded: string;
        GenBgShadedActive: string;
        GenClrShadedActive: string;
        GenFont: string;
        GenSmallFont: string;
        GenBorderWidth: number;
        GenBorderClr: string;
        GenBorderRadius: number;
        GenOpacity: number;
    }

    export class SkinPaletteModule extends YetaWF.ModuleBaseDataImpl {

        public static readonly SELECTOR: string = ".YetaWF_SkinPalette_SkinPalette";

        private Setup: Setup;

        private Form: HTMLFormElement;
        private ExpandCollapse: HTMLElement;
        private Tabs: YetaWF_ComponentsHTML.TabsComponent;
        private Contents: HTMLElement;

        private Config: HTMLTextAreaElement;
        private ConfigSave: HTMLElement;
        private AutoGenerate: HTMLElement;
        private Theme: YetaWF_ComponentsHTML.DropDownListEditComponent;

        constructor(id: string, setup: Setup) {
            super(id, SkinPaletteModule.SELECTOR, null);
            this.Setup = setup;

            this.Form = $YetaWF.Forms.getInnerForm(this.Module);
            this.ExpandCollapse = $YetaWF.getElement1BySelector(".t_palette", [this.Module]);
            this.Tabs = YetaWF_ComponentsHTML.TabsComponent.getControlFromSelector(".yt_tabs", YetaWF_ComponentsHTML.TabsComponent.SELECTOR, [this.Module]);
            this.Contents = $YetaWF.getElement1BySelector(".t_contents", [this.Module]);
            this.Config = $YetaWF.getElement1BySelector("textarea[name='CSSVariables']", [this.Module]) as HTMLTextAreaElement;
            this.ConfigSave = $YetaWF.getElement1BySelector("a[data-name='Save']", [this.Module]);
            this.AutoGenerate = $YetaWF.getElement1BySelector("a[data-name='AutoGen']", [this.Module]);
            this.Theme = YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromSelector("[name='GenTheme']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Form]) as YetaWF_ComponentsHTML.DropDownListEditComponent;

            this.populateFromVariables();
            this.updateGenValues();

            $YetaWF.registerEventHandler(this.ExpandCollapse, "click", null, (ev: MouseEvent): boolean => {
                if (this.Contents.style.display === "") {
                    this.Contents.style.opacity = "0";
                    $YetaWF.transitionEnd(this.Contents, (): void =>{
                        this.Contents.style.display = "none";
                    });
                } else {
                    this.Contents.style.opacity = "0";
                    this.Contents.style.display = "";
                    $YetaWF.forceRedraw(this.Contents);
                    this.Contents.style.opacity = "1";
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.ConfigSave, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.setLoading(true);
                $YetaWF.pleaseWait("Saving theme...");
                setTimeout((): void => {
                    if (!$YetaWF.Forms.validate(this.Form)) {
                        $YetaWF.setLoading(false);
                        $YetaWF.Forms.showErrors(this.Form);
                        return;
                    }
                    this.updateSkin();
                    $YetaWF.setLoading(false);
                }, 1);
                return false;
            });
            $YetaWF.registerEventHandler(this.AutoGenerate, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.setLoading(true);
                if (!$YetaWF.Forms.validate(this.Form)) {
                    $YetaWF.setLoading(false);
                    $YetaWF.Forms.showErrors(this.Form);
                    return false;
                }
                this.generateSkin();
                this.populateFromModel();
                $YetaWF.setLoading(false);
                $YetaWF.message("CSS variables successfully generated and updated");
                return false;
            });
            this.Tabs.Control.addEventListener(YetaWF_ComponentsHTML.TabsComponent.EVENTSWITCHED, (evt: Event): void => {
                this.updateConfig();
            });
            this.Theme.Control.addEventListener(YetaWF_ComponentsHTML.DropDownListEditComponent.EVENTCHANGE, (evt: Event): void => {
                this.updateGenValues();
            });
        }
        private populateFromVariables(): void {
            YetaWF_ComponentsHTML_Validation.clearValidation(this.Module);
            let root = document.documentElement;
            let styles = getComputedStyle(root);
            for (let entry of this.Setup.Properties) {
                let val = styles.getPropertyValue(entry.CSSVarName);
                val = val.trim();
                switch (entry.UIHint) {
                    case "Color":
                        let clrControl:YetaWF_ComponentsHTML.ColorEditComponent = YetaWF_ComponentsHTML.ColorEditComponent.getControlFromSelector(`[name="${entry.ModelName}"]`,YetaWF_ComponentsHTML.ColorEditComponent.SELECTOR, [this.Module]);
                        clrControl.value = val;
                        clrControl.Control.addEventListener(YetaWF_ComponentsHTML.ColorEditComponent.EVENTCHANGE, (evt: Event): void => {
                            root.style.setProperty(entry.CSSVarName, clrControl.value);
                        });
                        break;
                    case "Decimal":
                        let decControl:YetaWF_ComponentsHTML.DecimalEditComponent = YetaWF_ComponentsHTML.DecimalEditComponent.getControlFromSelector(`[name="${entry.ModelName}"]`,YetaWF_ComponentsHTML.DecimalEditComponent.SELECTOR, [this.Module]);
                        decControl.value = Number(val);
                        decControl.Control.addEventListener(YetaWF_ComponentsHTML.DecimalEditComponent.EVENT, (evt: Event): void => {
                            root.style.setProperty(entry.CSSVarName, decControl.valueText);
                        });
                        break;
                    default:
                        let elem = $YetaWF.getElement1BySelector(`[name="${entry.ModelName}"]`, [this.Module]) as HTMLInputElement;
                        elem.value = val;
                        $YetaWF.registerEventHandler(elem, "change", null, (ev: Event):boolean =>{
                            root.style.setProperty(entry.CSSVarName, elem.value);
                            return true;
                        });
                        break;
                }
            }
        }
        private populateFromModel(): void {
            let root = document.documentElement;
            for (let entry of this.Setup.Properties) {
                switch (entry.UIHint) {
                    case "Color":
                        let clrControl:YetaWF_ComponentsHTML.ColorEditComponent = YetaWF_ComponentsHTML.ColorEditComponent.getControlFromSelector(`[name="${entry.ModelName}"]`,YetaWF_ComponentsHTML.ColorEditComponent.SELECTOR, [this.Module]);
                        root.style.setProperty(entry.CSSVarName, clrControl.value);
                        break;
                    case "Decimal":
                        let decControl:YetaWF_ComponentsHTML.DecimalEditComponent = YetaWF_ComponentsHTML.DecimalEditComponent.getControlFromSelector(`[name="${entry.ModelName}"]`,YetaWF_ComponentsHTML.DecimalEditComponent.SELECTOR, [this.Module]);
                        root.style.setProperty(entry.CSSVarName, decControl.valueText);
                        break;
                    default:
                        let elem = $YetaWF.getElement1BySelector(`[name="${entry.ModelName}"]`, [this.Module]) as HTMLInputElement;
                        root.style.setProperty(entry.CSSVarName, elem.value);
                        break;
                }
            }
        }
        private updateConfig(): void {
            let text = "@media screen {\n\n:root {\n";
            let root = document.documentElement;
            let styles = getComputedStyle(root);
            for (let entry of this.Setup.Properties) {
                let val = styles.getPropertyValue(entry.CSSVarName);
                val = val.trim();
                text += `   ${entry.CSSVarName}: ${val};\n`;
            }
            text = text + "}\n\n}\n";
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
            this.populateFromVariables();

            $YetaWF.Forms.submit(this.Form, true);
        }
        private updateGenValues(): void {
            let mode = Number(this.Theme.value);
            let values: GenValues;
            if (mode === BasicThemeEnum.Light) {
                values = {
                    GenTheme: BasicThemeEnum.Light,
                    GenBg: "#FFFFFF",
                    GenClr: "#7e7e7e",
                    GenBgActive: "#2fa4e7",
                    GenClrActive: "#9e9e9e",
                    GenBgShaded: "#f6f6f6",
                    GenClrShaded: "#454545",
                    GenBgShadedActive: "#1a99e2",
                    GenClrShadedActive: "#FFFFFF",
                    GenFont: "normal normal normal 1rem 'Open Sans', sans-serif",
                    GenSmallFont: "normal normal normal .7rem 'Open Sans', sans-serif",
                    GenBorderWidth: 1,
                    GenBorderClr: "#c5c5c5",
                    GenBorderRadius: 3,
                    GenOpacity: 0.5,
                };
            } else {
                values = {
                    GenTheme: BasicThemeEnum.Dark,
                    GenBg: "#5c5c5c",
                    GenClr: "#FFFFFF",
                    GenBgActive: "#787878",
                    GenClrActive: "#ffffff",
                    GenBgShaded: "#454545",
                    GenClrShaded: "#f6f6f6",
                    GenBgShadedActive: "#a6a6a6",
                    GenClrShadedActive: "#ffffff",
                    GenFont: "normal normal normal 1rem 'Open Sans', sans-serif",
                    GenSmallFont: "normal normal normal .7rem 'Open Sans', sans-serif",
                    GenBorderWidth: 1,
                    GenBorderClr: "#777",
                    GenBorderRadius: 3,
                    GenOpacity: 0.5,
                };
            }
            this.getColor("GenBg").value = values.GenBg;
            this.getColor("GenClr").value = values.GenClr;
            this.getColor("GenBgActive").value = values.GenBgActive;
            this.getColor("GenClrActive").value = values.GenClrActive;
            this.getColor("GenBgShaded").value = values.GenBgShaded;
            this.getColor("GenClrShaded").value = values.GenClrShaded;
            this.getColor("GenBgShadedActive").value = values.GenBgShadedActive;
            this.getColor("GenClrShadedActive").value = values.GenClrShadedActive;
            this.getInput("GenFont").value = values.GenFont;
            this.getInput("GenSmallFont").value = values.GenSmallFont;
            this.getIntValue("GenBorderWidth").value = values.GenBorderWidth;
            this.getColor("GenBorderClr").value = values.GenBorderClr;
            this.getIntValue("GenBorderRadius").value = values.GenBorderRadius;
            this.getDecimal("BodyDisabledOpacity").value = values.GenOpacity;
        }

        private generateSkin(): void {

            let values = this.getValues();

            this.getColor("BodyBg").value = values.GenBg;
            this.getColor("BodyClr").value = values.GenClr;
            this.getInput("BodyFont").value = values.GenFont;
            this.getDecimal("BodyDisabledOpacity").value = values.GenOpacity;

            this.getColor("OverlayBg").value = values.GenTheme === BasicThemeEnum.Light ? "#aaaaaa" : "#444444";
            this.getDecimal("OverlayOpacity").value = values.GenOpacity;

            this.getInput("ModStandardTitleFont").value = values.GenFont;

            this.getColor("ModPanelBg").value = values.GenBgShaded;
            this.getColor("ModPanelClr").value = values.GenClrShaded;
            this.getInput("ModPanelPadding").value = "1em";
            this.getInput("ModPanelBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("ModPanelBorderRadius").value = `${values.GenBorderRadius}px`;
            this.getInput("ModPanelShadow").value = "none";
            this.getInput("ModPanelTitleFont").value = values.GenFont;
            this.getInput("ModPanelLinkFont").value = values.GenSmallFont;

            this.getInput("MainMenuHorz0Padding").value = "0";
            this.getColor("MainMenuHorz0Clr").value = values.GenClr;
            this.getInput("MainMenuHorz0Font").value = values.GenFont;
            this.getInput("MainMenuHorz0Border").value = "none";
            this.getInput("MainMenuHorz0BorderRadius").value = `${values.GenBorderRadius}px`;
            this.getColor("MainMenuHorz0Bg").value = values.GenBg;
            this.getColor("MainMenuHorz0ABg").value = values.GenBg;
            this.getColor("MainMenuHorz0AClr").value = values.GenClr;
            this.getColor("MainMenuHorz0ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuHorz0AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenuHorz0ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuHorz0AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenuHorz0APadding").value = "0.7em 1.5em";
            this.getInput("MainMenuHorz0DDWidth").value = "16px";
            this.getInput("MainMenuHorz0DDHeight").value = "16px";
            this.getColor("MainMenuHorz1Bg").value = values.GenBg;
            this.getColor("MainMenuHorz1Clr").value = values.GenClr;
            this.getInput("MainMenuHorz1Font").value = values.GenFont;
            this.getInput("MainMenuHorz1Border").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("MainMenuHorz1BorderRadius").value = `${values.GenBorderRadius}px`;
            this.getInput("MainMenuHorz1Padding").value = "0.3em 0";
            this.getColor("MainMenuHorz1ABg").value = values.GenBg;
            this.getColor("MainMenuHorz1AClr").value = values.GenClr;
            this.getColor("MainMenuHorz1ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuHorz1AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenuHorz1ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuHorz1AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenuHorz1APadding").value = "0.1em 1.5em 0.1em 1.5em";
            this.getInput("MainMenuHorz1DDWidth").value = "16px";
            this.getInput("MainMenuHorz1DDHeight").value = "16px";
            this.getInput("MainMenuHorz1MMWidth").value = "800px";
            this.getColor("MainMenuHorz1MMBg").value = values.GenBg;
            this.getColor("MainMenuHorz1MMClr").value = values.GenClr;
            this.getInput("MainMenuHorz1MMFont").value = values.GenFont;
            this.getColor("MainMenuHorz2Bg").value = values.GenBg;
            this.getColor("MainMenuHorz2Clr").value = values.GenClr;
            this.getInput("MainMenuHorz2Font").value = values.GenFont;
            this.getInput("MainMenuHorz2Border").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("MainMenuHorz2BorderRadius").value = `${values.GenBorderRadius}px`;
            this.getInput("MainMenuHorz2Padding").value = "0.3em 0";
            this.getColor("MainMenuHorz2ABg").value = values.GenBg;
            this.getColor("MainMenuHorz2AClr").value = values.GenClr;
            this.getColor("MainMenuHorz2ABgHover").value =  this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuHorz2AClrHover").value =  this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenuHorz2ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuHorz2AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenuHorz2APadding").value = "0.1em 1em 0.1em 1em";
            this.getInput("MainMenuHorz2DDWidth").value = "16px";
            this.getInput("MainMenuHorz2DDHeight").value = "16px";

            this.getInput("MainMenuVert0Padding").value = "0";
            this.getColor("MainMenuVert0Clr").value = values.GenClr;
            this.getInput("MainMenuVert0Font").value = values.GenFont;
            this.getInput("MainMenuVert0Border").value = "none";
            this.getInput("MainMenuVert0BorderRadius").value = `${values.GenBorderRadius}px`;
            this.getColor("MainMenuVert0Bg").value = values.GenBg;
            this.getColor("MainMenuVert0ABg").value = values.GenBg;
            this.getColor("MainMenuVert0AClr").value = values.GenClr;
            this.getColor("MainMenuVert0ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuVert0AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenuVert0ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuVert0AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenuVert0APadding").value = "0.7em 1.5em";
            this.getInput("MainMenuVert0DDWidth").value = "16px";
            this.getInput("MainMenuVert0DDHeight").value = "16px";
            this.getColor("MainMenuVert1Bg").value = values.GenBg;
            this.getColor("MainMenuVert1Clr").value = values.GenClr;
            this.getInput("MainMenuVert1Font").value = values.GenFont;
            this.getInput("MainMenuVert1Border").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("MainMenuVert1BorderRadius").value = `${values.GenBorderRadius}px`;
            this.getInput("MainMenuVert1Padding").value = "0.3em 0";
            this.getColor("MainMenuVert1ABg").value = values.GenBg;
            this.getColor("MainMenuVert1AClr").value = values.GenClr;
            this.getColor("MainMenuVert1ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuVert1AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenuVert1ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuVert1AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenuVert1APadding").value = "0.1em 1.5em 0.1em 1.5em";
            this.getInput("MainMenuVert1DDWidth").value = "16px";
            this.getInput("MainMenuVert1DDHeight").value = "16px";
            this.getInput("MainMenuVert1MMWidth").value = "800px";
            this.getColor("MainMenuVert1MMBg").value = values.GenBg;
            this.getColor("MainMenuVert1MMClr").value = values.GenClr;
            this.getInput("MainMenuVert1MMFont").value = values.GenFont;
            this.getColor("MainMenuVert2Bg").value = values.GenBg;
            this.getColor("MainMenuVert2Clr").value = values.GenClr;
            this.getInput("MainMenuVert2Font").value = values.GenFont;
            this.getInput("MainMenuVert2Border").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("MainMenuVert2BorderRadius").value = `${values.GenBorderRadius}px`;
            this.getInput("MainMenuVert2Padding").value = "0.3em 0";
            this.getColor("MainMenuVert2ABg").value = values.GenBg;
            this.getColor("MainMenuVert2AClr").value = values.GenClr;
            this.getColor("MainMenuVert2ABgHover").value =  this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuVert2AClrHover").value =  this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenuVert2ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuVert2AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenuVert2APadding").value = "0.1em 1em 0.1em 1em";
            this.getInput("MainMenuVert2DDWidth").value = "16px";
            this.getInput("MainMenuVert2DDHeight").value = "16px";

            this.getInput("MainMenuSm0Padding").value = "0";
            this.getColor("MainMenuSm0Clr").value = values.GenClr;
            this.getInput("MainMenuSm0Font").value = values.GenFont;
            this.getInput("MainMenuSm0Border").value = "none";
            this.getInput("MainMenuSm0BorderRadius").value = `${values.GenBorderRadius}px`;
            this.getColor("MainMenuSm0Bg").value = values.GenBg;
            this.getColor("MainMenuSm0ABg").value = values.GenBg;
            this.getColor("MainMenuSm0AClr").value = values.GenClr;
            this.getColor("MainMenuSm0ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuSm0AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenuSm0ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuSm0AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenuSm0APadding").value = "0.7em 1.5em";
            this.getInput("MainMenuSm0DDWidth").value = "16px";
            this.getInput("MainMenuSm0DDHeight").value = "16px";
            this.getColor("MainMenuSm1Bg").value = values.GenBg;
            this.getColor("MainMenuSm1Clr").value = values.GenClr;
            this.getInput("MainMenuSm1Font").value = values.GenFont;
            this.getInput("MainMenuSm1Border").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("MainMenuSm1BorderRadius").value = `${values.GenBorderRadius}px`;
            this.getInput("MainMenuSm1Padding").value = "0.5em 0 0.5em 0";
            this.getColor("MainMenuSm1ABg").value = values.GenBg;
            this.getColor("MainMenuSm1AClr").value = values.GenClr;
            this.getColor("MainMenuSm1ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuSm1AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenuSm1ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuSm1AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenuSm1APadding").value = "0.4em 0.5em 0.4em 0.75em";
            this.getInput("MainMenuSm1DDWidth").value = "16px";
            this.getInput("MainMenuSm1DDHeight").value = "16px";
            this.getInput("MainMenuSm1MMWidth").value = "800px";
            this.getColor("MainMenuSm1MMBg").value = values.GenBg;
            this.getColor("MainMenuSm1MMClr").value = values.GenClr;
            this.getInput("MainMenuSm1MMFont").value = values.GenFont;
            this.getColor("MainMenuSm2Bg").value = values.GenBg;
            this.getColor("MainMenuSm2Clr").value = values.GenClr;
            this.getInput("MainMenuSm2Font").value = values.GenFont;
            this.getInput("MainMenuSm2Border").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("MainMenuSm2BorderRadius").value = `${values.GenBorderRadius}px`;
            this.getInput("MainMenuSm2Padding").value = "0.5em 0";
            this.getColor("MainMenuSm2ABg").value = values.GenBg;
            this.getColor("MainMenuSm2AClr").value = values.GenClr;
            this.getColor("MainMenuSm2ABgHover").value =  this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuSm2AClrHover").value =  this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenuSm2ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuSm2AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenuSm2APadding").value = "0.4em 0.5em 0.4em 0.75em";
            this.getInput("MainMenuSm2DDWidth").value = "16px";
            this.getInput("MainMenuSm2DDHeight").value = "16px";

            this.getInput("MenuSVGSmallBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("MenuSVGSmallBorderRadius").value = `${values.GenBorderRadius}px`;

            this.getColor("MiniScrollClr").value = values.GenClr;

            this.getColor("PopupMenu1Bg").value = values.GenBg;
            this.getColor("PopupMenu1Clr").value = values.GenClr;
            this.getInput("PopupMenu1Font").value = values.GenSmallFont;
            this.getInput("PopupMenu1Border").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("PopupMenu1BorderRadius").value = `${values.GenBorderRadius}px`;
            this.getInput("PopupMenu1Padding").value = "0";
            this.getColor("PopupMenu1ABg").value = values.GenBg;
            this.getColor("PopupMenu1AClr").value = values.GenClr;
            this.getColor("PopupMenu1ABgHover").value =  this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("PopupMenu1AClrHover").value =  this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("PopupMenu1ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("PopupMenu1AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("PopupMenu1APadding").value = "0.4em 0.5em 0.4em 0.5em";
            this.getInput("PopupMenu1DDWidth").value = "16px";
            this.getInput("PopupMenu1DDHeight").value = "16px";
            this.getColor("PopupMenu2Bg").value = values.GenBg;
            this.getColor("PopupMenu2Clr").value = values.GenClr;
            this.getInput("PopupMenu2Font").value = values.GenSmallFont;
            this.getInput("PopupMenu2Border").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("PopupMenu2BorderRadius").value = `${values.GenBorderRadius}px`;
            this.getInput("PopupMenu2Padding").value = "0";
            this.getColor("PopupMenu2ABg").value = values.GenBg;
            this.getColor("PopupMenu2AClr").value = values.GenClr;
            this.getColor("PopupMenu2ABgHover").value =  this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("PopupMenu2AClrHover").value =  this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("PopupMenu2ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("PopupMenu2AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("PopupMenu2APadding").value = "0.4em 0.5em 0.4em 0.5em";
            this.getInput("PopupMenu2DDWidth").value = "16px";
            this.getInput("PopupMenu2DDHeight").value = "16px";

            this.getColor("TTBg").value = values.GenBg;
            this.getColor("TTClr").value = values.GenClr;
            this.getInput("TTFont").value = values.GenFont;
            this.getInput("TTBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("TTBorderRadius").value = `${values.GenBorderRadius}px`;
            this.getInput("TTShadow").value = "0px 0px 5px 0px rgba(0, 0, 0, 0.5)";

            this.getColor("DialogBg").value = values.GenBg;
            this.getColor("DialogClr").value = values.GenClr;
            this.getColor("DialogTitleBg").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBgShaded, 40) : this.Lighten(values.GenBgShaded, 40);
            this.getColor("DialogTitleClr").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClrShaded, 40) : this.Lighten(values.GenClrShaded, 40);
            this.getInput("DialogTitleFont").value = values.GenClr;
            this.getInput("DialogTitleBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("DialogTitleBorderRadius").value = `${values.GenBorderRadius}px`;
            this.getInput("DialogLine").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("DialogLinePadding").value = "1em 0 1em 0";
            this.getInput("DialogBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("DialogBorderRadius").value = `${values.GenBorderRadius}px`;

            this.getInput("TstTitleFont").value = "normal normal normal 1rem/2rem 'Open Sans', sans-serif";
            this.getInput("TstMsgFont").value = values.GenFont;
            this.getInput("TstBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("TstBorderRadius").value = `${values.GenBorderRadius}px`;
            this.getColor("TstInfoBg").value = "#9bef9f";
            this.getColor("TstInfoClr").value = "#555";
            this.getInput("TstInfoShadow").value = "0px 0px 5px 0px rgba(0, 0, 0, 0.5)";
            this.getInput("TstInfoLine").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getColor("TstWarnBg").value = "orange";
            this.getColor("TstWarnClr").value = "rgb(49, 46, 46)";
            this.getInput("TstWarnShadow").value = "0px 0px 5px 0px rgba(0, 0, 0, 0.5)";
            this.getInput("TstWarnLine").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getColor("TstErrorBg").value = "red";
            this.getColor("TstErrorClr").value = "white";
            this.getInput("TstErrorShadow").value = "0px 0px 5px 0px rgba(0, 0, 0, 0.5)";
            this.getInput("TstErrorLine").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;

            this.getColor("BarBg").value = values.GenBgShaded;
            this.getColor("BarClr").value = values.GenClrShaded;
            this.getColor("BarBgHover").value =  this.ToHover(values.GenTheme, values.GenBgShaded);
            this.getColor("BarClrHover").value =  this.ToHover(values.GenTheme, values.GenClrShaded);
            this.getColor("BarBgActive").value = values.GenBgShadedActive;
            this.getColor("BarClrActive").value = values.GenClrShadedActive;
            this.getInput("BarBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("BarBorderRadius").value = `${values.GenBorderRadius}px`;
            this.getInput("BarH1FontSize").value = "1.25rem";

            this.getInput("PropBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("PropBorderRadius").value = `${values.GenBorderRadius}px`;
            this.getColor("PropCatBg").value = values.GenBgShaded;
            this.getColor("PropCatClr").value = values.GenClrShaded;
            this.getInput("PropCatBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("PropCatBorderRadius").value = `${values.GenBorderRadius}px`;

            this.getColor("InputBg").value = values.GenBg;
            this.getColor("InputClr").value = values.GenClr;
            this.getInput("InputBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("InputBorderHover").value = `${values.GenBorderWidth}px solid ${this.ToHover(values.GenTheme, values.GenBorderClr)}`;
            this.getInput("InputBorderFocus").value = `${values.GenBorderWidth}px solid ${this.ToFocus(values.GenTheme, values.GenBorderClr)}`;
            this.getInput("InputBorderRadius").value = `${values.GenBorderRadius}px`;
            this.getColor("InputError").value = "red";
            this.getColor("InputPlaceholderClr").value = values.GenTheme === BasicThemeEnum.Light ? "black" : "white";
            this.getDecimal("InputPlaceholderOpacity").value = values.GenOpacity;


            this.getColor("DDBg").value = values.GenBg;
            this.getColor("DDClr").value = values.GenClr;
            this.getColor("DDBgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("DDClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("DDBgActive").value = values.GenBgActive;
            this.getColor("DDClrActive").value = values.GenClrActive;
            this.getColor("DDBgFocus").value = this.ToFocus(values.GenTheme, values.GenBg);
            this.getColor("DDClrFocus").value = this.ToFocus(values.GenTheme, values.GenClr);
            this.getInput("DDBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("DDBorderHover").value = `${values.GenBorderWidth}px solid ${this.ToHover(values.GenTheme, values.GenBorderClr)}`;
            this.getInput("DDBorderFocus").value = `${values.GenBorderWidth}px solid ${this.ToFocus(values.GenTheme, values.GenBorderClr)}`;
            this.getInput("DDBorderRadius").value = `${values.GenBorderRadius}px`;
            this.getColor("DDListBg").value = values.GenBg;
            this.getColor("DDListClr").value = values.GenClr;
            this.getColor("DDListBgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("DDListClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("DDListBgFocus").value = this.ToFocus(values.GenTheme, values.GenBg);
            this.getColor("DDListClrFocus").value = this.ToFocus(values.GenTheme, values.GenClr);
            this.getInput("DDListBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("DDListBorderRadius").value = `${values.GenBorderRadius}px`;
            this.getInput("DDListShadow").value = "0 2px 2px 0 rgba(0,0,0,.3)";
            this.getInput("DDListShadowFocus").value ="inset 0px 0px 3px 0px rgba(0, 0, 0, 0.25)";

            this.getColor("AnchorClr").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClr, 20) : this.Lighten(values.GenClr, 20);
            this.getInput("AnchorDec").value = "none";
            this.getColor("AnchorClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("AnchorDecHover").value = "underline";
            this.getColor("AnchorClrFocus").value = values.GenClrActive;
            this.getInput("AnchorDecFocus").value = "underline";

            this.getColor("ButtonBg").value = values.GenBg;
            this.getColor("ButtonClr").value = values.GenClr;
            this.getColor("ButtonBgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("ButtonClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("ButtonBgFocus").value = this.ToFocus(values.GenTheme, values.GenBg);
            this.getColor("ButtonClrFocus").value = this.ToFocus(values.GenTheme, values.GenClr);
            this.getInput("ButtonBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("ButtonBorderRadius").value = `${values.GenBorderRadius}px`;
            this.getInput("ButtonShadow").value = "0px 0px 3px 0px rgba(0, 0, 0, 0.25)";
            this.getInput("ButtonShadowFocus").value = "inset 0px 0px 3px 0px rgba(0, 0, 0, 0.25)";

            this.getColor("ButtonLiteBg").value = values.GenBgShaded;
            this.getColor("ButtonLiteClr").value = values.GenClrShaded;
            this.getColor("ButtonLiteBgHover").value = this.ToHover(values.GenTheme, values.GenBgShaded);
            this.getColor("ButtonLiteClrHover").value = this.ToHover(values.GenTheme, values.GenClrShaded);
            this.getColor("ButtonLiteBgFocus").value = this.ToFocus(values.GenTheme, values.GenBgShaded);
            this.getColor("ButtonLiteClrFocus").value = this.ToFocus(values.GenTheme, values.GenClrShaded);
            this.getInput("ButtonLiteBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("ButtonLiteBorderRadius").value = `${values.GenBorderRadius}px`;

            this.getColor("PbarBg").value = values.GenBgShaded;
            this.getColor("PbarValueBg").value = values.GenClrShaded;
            this.getInput("PbarBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("PbarBorderRadius").value = `${values.GenBorderRadius}px`;

            this.getColor("TableBg").value = values.GenBg;
            this.getColor("TableClr").value = values.GenClr;
            this.getColor("TableBgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("TableClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("TableBgActive").value = values.GenBgActive;
            this.getColor("TableClrActive").value  = values.GenClrActive;
            this.getColor("TableBgFocus").value = this.ToFocus(values.GenTheme, values.GenBg);
            this.getColor("TableClrFocus").value = this.ToFocus(values.GenTheme, values.GenClr);
            this.getColor("TableBgHighlight").value = values.GenTheme === BasicThemeEnum.Light ? "silver" : "white";
            this.getColor("TableClrHighlight").value = values.GenTheme === BasicThemeEnum.Light ? "white" : "silver";
            this.getColor("TableBgLowlight").value = values.GenTheme === BasicThemeEnum.Light ? "ghostwhite" : "darkslategray";
            this.getColor("TableClrLowlight").value = values.GenTheme === BasicThemeEnum.Light ? "darkslategray" :"ghostwhite" ;
            this.getInput("TableFont").value = values.GenSmallFont;
            this.getInput("TableBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("TableBorderLite").value = `${values.GenBorderWidth}px solid ${values.GenTheme === BasicThemeEnum.Light ? this.Lighten(values.GenBorderClr, 20) : this.Darken(values.GenBorderClr, 20)}`;
            this.getInput("TableBorderRadius").value = `${values.GenBorderRadius}px`;
            this.getInput("TableShadow").value = "0px 0px 10px 0px rgba(0,0,0,0.1)";
            this.getColor("TableHeaderBg").value = values.GenBgShaded;
            this.getColor("TableHeaderClr").value = values.GenClrShaded;
            this.getColor("TableHeaderBgHover").value = this.ToHover(values.GenTheme, values.GenBgShaded);
            this.getColor("TableHeaderClrHover").value = this.ToHover(values.GenTheme, values.GenClrShaded);
            this.getColor("TableHeaderBgActive").value = values.GenBgShadedActive;
            this.getColor("TableHeaderClrActive").value = values.GenClrShadedActive;
            this.getInput("TableHeaderFont").value = values.GenSmallFont;

            this.getColor("TabsBg").value = values.GenBg;
            this.getColor("TabsClr").value = values.GenClr;
            this.getColor("TabsStripBg").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBgShaded, 40) : this.Lighten(values.GenBgShaded, 40);
            this.getInput("TabsStripBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("TabsStripBorderRadius").value = `${values.GenBorderRadius}px`;
            this.getColor("TabsTabBg").value = values.GenBgShaded;
            this.getColor("TabsTabClr").value = values.GenClrShaded;
            this.getColor("TabsTabBgHover").value = this.ToHover(values.GenTheme, values.GenBgShaded);
            this.getColor("TabsTabClrHover").value = this.ToHover(values.GenTheme, values.GenClrShaded);
            this.getColor("TabsTabBgActive").value = values.GenBgShadedActive;
            this.getColor("TabsTabClrActive").value = values.GenClrShadedActive;
            this.getColor("TabsTabBgFocus").value = this.ToFocus(values.GenTheme, values.GenBgShadedActive);
            this.getColor("TabsTabClrFocus").value = this.ToFocus(values.GenTheme, values.GenClrShadedActive);
            this.getInput("TabsTabBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("TabsTabBorderRadius").value = `${values.GenBorderRadius}px`;

            this.getColor("StepBg").value = values.GenBgShaded;
            this.getColor("StepClr").value = values.GenClrShaded;
            this.getColor("StepBgActive").value = values.GenBgShadedActive;
            this.getColor("StepClrActive").value = values.GenClrShadedActive;

            this.getColor("PanelBg").value = values.GenBgShaded;
            // this.getColor("PanelClr").value = values.GenClrShaded;
            this.getInput("PanelBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("PanelBorderRadius").value = ".25rem";
            this.getInput("PanelShadow").value = "0px 0px 10px 0px rgba(0,0,0,0.1)";

            this.getColor("SwitchBgOn").value = "#9386ec";
            this.getColor("SwitchClrOn").value = "#FFFFFF";
            this.getColor("SwitchBgOff").value = "#9ce1f5";
            this.getColor("SwitchClrOff").value = "#1a4c4c";
            this.getColor("SwitchBgSwitch").value = "#5e65ac";
        }

        private getValues(): GenValues {
            let ddTheme = YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromSelector("[name='GenTheme']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Form]) as YetaWF_ComponentsHTML.DropDownListEditComponent;
            return {
                GenTheme: Number(ddTheme.value),
                GenBg: this.getInput("GenBg").value,
                GenClr: this.getInput("GenClr").value,
                GenBgActive: this.getInput("GenBgActive").value,
                GenClrActive: this.getInput("GenClrActive").value,
                GenBgShaded: this.getInput("GenBgShaded").value,
                GenClrShaded: this.getInput("GenClrShaded").value,
                GenBgShadedActive: this.getInput("GenBgShadedActive").value,
                GenClrShadedActive: this.getInput("GenClrShadedActive").value,
                GenFont: this.getInput("GenFont").value,
                GenSmallFont: this.getInput("GenSmallFont").value,
                GenBorderClr: this.getInput("GenBorderClr").value,
                GenBorderWidth: Number(this.getInput("GenBorderWidth").value),
                GenBorderRadius: Number(this.getInput("GenBorderRadius").value),
                GenOpacity: this.getDecimal("BodyDisabledOpacity").value,
            };
        }
        /** Turn a basic color into a hover color */
        private ToHover(theme: BasicThemeEnum, color: string) : string {
            if (theme === BasicThemeEnum.Light)
                return this.Darken(color, 20);
            else
                return this.Lighten(color, 20);
        }
        /** Turn an active color into a focus color */
        private ToFocus(theme: BasicThemeEnum, color: string) : string {
            if (theme === BasicThemeEnum.Light)
                return this.Darken(color, 40);
            else
                return this.Lighten(color, 40);
        }
        private getInput(name: string): HTMLInputElement {
            return $YetaWF.getElement1BySelector(`[name='${name}']`, [this.Form]) as HTMLInputElement;
        }
        private getColor(name: string): YetaWF_ComponentsHTML.ColorEditComponent {
            let control = YetaWF_ComponentsHTML.ColorEditComponent.getControlFromSelector(`[name='${name}']`, YetaWF_ComponentsHTML.ColorEditComponent.SELECTOR, [this.Form]) as YetaWF_ComponentsHTML.ColorEditComponent;
            return control;
        }
        private getIntValue(name: string): YetaWF_ComponentsHTML.IntValueEditComponent {
            let control = YetaWF_ComponentsHTML.IntValueEditComponent.getControlFromSelector(`[name='${name}']`, YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [this.Form]) as YetaWF_ComponentsHTML.IntValueEditComponent;
            return control;
        }
        private getDecimal(name: string): YetaWF_ComponentsHTML.DecimalEditComponent {
            let control = YetaWF_ComponentsHTML.DecimalEditComponent.getControlFromSelector(`[name='${name}']`, YetaWF_ComponentsHTML.DecimalEditComponent.SELECTOR, [this.Form]) as YetaWF_ComponentsHTML.DecimalEditComponent;
            return control;
        }
        private Lighten(color: string, amount: number): string {
            return this.LightenDarken(color, amount);
        }
        private Darken(color: string, amount: number): string {
            return this.LightenDarken(color, -amount);
        }
        private LightenDarken(color: string, amount: number): string {
            color = color.replace(/^#/, "");
            if (color.length === 3) color = color[0] + color[0] + color[1] + color[1] + color[2] + color[2];

            let arr: RegExpMatchArray|null;
            arr = color.match(/.{2}/g);
            if (!arr) return color;
            let r = parseInt(arr[0], 16) + amount;
            let g = parseInt(arr[1], 16) + amount;
            let b = parseInt(arr[2], 16) + amount;

            let sr = Math.max(Math.min(255, r), 0).toString(16);
            let sg = Math.max(Math.min(255, g), 0).toString(16);
            let sb = Math.max(Math.min(255, b), 0).toString(16);

            const rr = (sr.length < 2 ? "0" : "") + sr;
            const gg = (sg.length < 2 ? "0" : "") + sg;
            const bb = (sb.length < 2 ? "0" : "") + sb;

            return `#${rr}${gg}${bb}`;
        }
    }
}
