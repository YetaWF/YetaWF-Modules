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
        GenBorderWidth: number;
        GenBorderClr: string;
        GenBorderRadius: number;
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
                    this.Contents.style.display = "none";
                } else {
                    this.Contents.style.display = "";
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.ConfigSave, "click", null, (ev: MouseEvent): boolean => {
                if (!$YetaWF.Forms.validate(this.Form)) {
                    $YetaWF.Forms.showErrors(this.Form);
                    return false;
                }
                this.updateSkin();
                return false;
            });
            $YetaWF.registerEventHandler(this.AutoGenerate, "click", null, (ev: MouseEvent): boolean => {
                if (!$YetaWF.Forms.validate(this.Form)) {
                    $YetaWF.Forms.showErrors(this.Form);
                    return false;
                }
                this.generateSkin();
                this.populateFromModel();
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
                        let clrControl:YetaWF_ComponentsHTML.ColorEditComponent = YetaWF_ComponentsHTML.ColorEditComponent.getControlFromSelector(`input[type="text"][name="${entry.ModelName}"]`,YetaWF_ComponentsHTML.ColorEditComponent.SELECTOR, [this.Module]);
                        clrControl.value = val;
                        clrControl.Control.addEventListener(YetaWF_ComponentsHTML.ColorEditComponent.EVENTCHANGE, (evt: Event): void => {
                            root.style.setProperty(entry.CSSVarName, clrControl.value);
                        });
                        break;
                    case "Decimal":
                        let decControl:YetaWF_ComponentsHTML.DecimalEditComponent = YetaWF_ComponentsHTML.DecimalEditComponent.getControlFromSelector(`input[type="text"][name="${entry.ModelName}"]`,YetaWF_ComponentsHTML.DecimalEditComponent.SELECTOR, [this.Module]);
                        decControl.value = Number(val);
                        decControl.Control.addEventListener(YetaWF_ComponentsHTML.DecimalEditComponent.EVENTCHANGE, (evt: Event): void => {
                            root.style.setProperty(entry.CSSVarName, decControl.valueText);
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
        private populateFromModel(): void {
            let root = document.documentElement;
            for (let entry of this.Setup.Properties) {
                switch (entry.UIHint) {
                    case "Color":
                        let clrControl:YetaWF_ComponentsHTML.ColorEditComponent = YetaWF_ComponentsHTML.ColorEditComponent.getControlFromSelector(`input[type="text"][name="${entry.ModelName}"]`,YetaWF_ComponentsHTML.ColorEditComponent.SELECTOR, [this.Module]);
                        root.style.setProperty(entry.CSSVarName, clrControl.value);
                        break;
                    case "Decimal":
                        let decControl:YetaWF_ComponentsHTML.DecimalEditComponent = YetaWF_ComponentsHTML.DecimalEditComponent.getControlFromSelector(`input[type="text"][name="${entry.ModelName}"]`,YetaWF_ComponentsHTML.DecimalEditComponent.SELECTOR, [this.Module]);
                        root.style.setProperty(entry.CSSVarName, decControl.valueText);
                        break;
                    default:
                        let elem = $YetaWF.getElement1BySelector(`input[type="text"][name="${entry.ModelName}"]`, [this.Module]) as HTMLInputElement;
                        root.style.setProperty(entry.CSSVarName, elem.value);
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
                    GenClrActive: "#FFFFFF",
                    GenBgShaded: "#f6f6f6",
                    GenClrShaded: "#454545",
                    GenBgShadedActive: "#1a99e2",
                    GenClrShadedActive: "#FFFFFF",
                    GenFont: "normal normal normal 1rem 'Open Sans', sans-serif",
                    GenBorderWidth: 1,
                    GenBorderClr: "#c5c5c5",
                    GenBorderRadius: 3,
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
                    GenBorderWidth: 1,
                    GenBorderClr: "#777",
                    GenBorderRadius: 3,
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
            this.getIntValue("GenBorderWidth").value = values.GenBorderWidth;
            this.getColor("GenBorderClr").value = values.GenBorderClr;
            this.getIntValue("GenBorderRadius").value = values.GenBorderRadius;
        }

        private generateSkin(): void {

            let values = this.getValues();

            this.getColor("BodyBg").value = values.GenBg;
            this.getColor("BodyClr").value = values.GenClr;
            this.getInput("BodyFont").value = values.GenFont;

            this.getColor("AnchorClr").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClr, 20) : this.Lighten(values.GenClr, 20);
            this.getInput("AnchorDec").value = "none";
            this.getColor("AnchorClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("AnchorDecHover").value = "underline";
            this.getColor("AnchorClrFocus").value = values.GenClrActive;
            this.getInput("AnchorDecFocus").value = "underline";

            this.getColor("OverlayBg").value = values.GenTheme === BasicThemeEnum.Light ? "#aaaaaa" : "#444444";
            this.getDecimal("OverlayOpacity").value = values.GenTheme === BasicThemeEnum.Light ? 0.5 : 0.8;

            this.getInput("MainMenu0Padding").value = "0";
            this.getColor("MainMenu0Clr").value = values.GenClr;
            this.getInput("MainMenu0Font").value = values.GenFont;
            this.getInput("MainMenu0Border").value = "none";
            this.getInput("MainMenu0BorderRadius").value = `${values.GenBorderRadius}px`;
            this.getColor("MainMenu0Bg").value = values.GenBg;
            this.getColor("MainMenu0ABg").value = values.GenBg;
            this.getColor("MainMenu0AClr").value = values.GenClr;
            this.getColor("MainMenu0ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenu0AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenu0ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenu0AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenu0APadding").value = "0.7em 1.5em";
            this.getInput("MainMenu0ASmallPadding").value = "0.7em 1.5em";
            this.getInput("MainMenu0DDWidth").value = "16px";
            this.getInput("MainMenu0DDHeight").value = "16px";
            this.getColor("MainMenu1Bg").value = values.GenBg;
            this.getColor("MainMenu1Clr").value = values.GenClr;
            this.getInput("MainMenu1Font").value = values.GenFont;
            this.getInput("MainMenu1Border").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("MainMenu1BorderRadius").value = `${values.GenBorderRadius}px`;
            this.getInput("MainMenu1SmallMargin").value = "0.5em";
            this.getInput("MainMenu1Padding").value = "0.3em 0";
            this.getInput("MainMenu1SmallPadding").value = "0.5em 0 0.5em 0";
            this.getColor("MainMenu1ABg").value = values.GenBg;
            this.getColor("MainMenu1AClr").value = values.GenClr;
            this.getColor("MainMenu1ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenu1AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenu1ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenu1AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenu1APadding").value = "0.1em 1.5em 0.1em 1.5em";
            this.getInput("MainMenu1ASmallPadding").value = "0.4em 0.5em 0.4em 0.75em";
            this.getInput("MainMenu1DDWidth").value = "16px";
            this.getInput("MainMenu1DDHeight").value = "16px";
            this.getInput("MainMenu1MMWidth").value = "800px";
            this.getColor("MainMenu1MMBg").value = values.GenBg;
            this.getColor("MainMenu1MMClr").value = values.GenClr;
            this.getInput("MainMenu1MMFont").value = values.GenFont;
            this.getColor("MainMenu2Bg").value = values.GenBg;
            this.getColor("MainMenu2Clr").value = values.GenClr;
            this.getInput("MainMenu2Font").value = values.GenFont;
            this.getInput("MainMenu2Border").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("MainMenu2BorderRadius").value = `${values.GenBorderRadius}px`;
            this.getInput("MainMenu2SmallMargin").value = "0.5em";
            this.getInput("MainMenu2Padding").value = "0.3em 0";
            this.getInput("MainMenu2SmallPadding").value = "0.5em 0";
            this.getColor("MainMenu2ABg").value = values.GenBg;
            this.getColor("MainMenu2AClr").value = values.GenClr;
            this.getColor("MainMenu2ABgHover").value =  this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenu2AClrHover").value =  this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenu2ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenu2AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenu2APadding").value = "0.1em 1em 0.1em 1em";
            this.getInput("MainMenu2ASmallPadding").value = "0.4em 0.5em 0.4em 0.75em";
            this.getInput("MainMenu2DDWidth").value = "16px";
            this.getInput("MainMenu2DDHeight").value = "16px";
            this.getInput("MainMenuSVGSmallBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("MainMenuSVGSmallBorderRadius").value = `${values.GenBorderRadius}px`;

            this.getColor("PopupMenu1Bg").value = values.GenBg;
            this.getColor("PopupMenu1Clr").value = values.GenClr;
            this.getInput("PopupMenu1Font").value = values.GenFont;
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
            this.getInput("PopupMenu2Font").value = values.GenFont;
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
            this.getInput("DialogLine").value = values.GenBorderClr;
            this.getInput("DialogBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("DialogBorderRadius").value = `${values.GenBorderRadius}px`;

            this.getInput("TstTitleFont").value = values.GenFont;
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
            this.getInput("BarH1FontSize").value = "1.25rem";
            this.getInput("BarH1PaddingBottom").value = ".7rem";

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


            this.getColor("DDBg").value = values.GenBg;
            this.getColor("DDClr").value = values.GenClr;
            this.getColor("DDBgHover").value = values.GenBg;
            this.getColor("DDClrHover").value = values.GenClr;
            this.getColor("DDBgFocus").value = values.GenBg;
            this.getColor("DDClrFocus").value = values.GenClr;
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
            this.getColor("TableBgHighlight").value = "silver";
            this.getColor("TableClrHighlight").value = "white";
            this.getColor("TableBgLowlight").value = "ghostwhite;";
            this.getColor("TableClrLowlight").value = "darkslategray";
            this.getColor("TableHeaderBg").value = values.GenBgShaded;
            this.getColor("TableHeaderClr").value = values.GenClrShaded;
            this.getColor("TableHeaderBgHover").value = this.ToHover(values.GenTheme, values.GenBgShaded);
            this.getColor("TableHeaderClrHover").value = this.ToHover(values.GenTheme, values.GenClrShaded);
            this.getColor("TableHeaderBgActive").value = values.GenBgShadedActive;
            this.getColor("TableHeaderClrActive").value = values.GenClrShadedActive;
            this.getInput("TableBorder").value = `${values.GenBorderWidth}px solid ${values.GenBorderClr}`;
            this.getInput("TableBorderLite").value = `${values.GenBorderWidth}px solid ${values.GenTheme === BasicThemeEnum.Light ? this.Lighten(values.GenBorderClr, 20) : this.Darken(values.GenBorderClr, 20)}`;
            this.getInput("TableBorderRadius").value = `${values.GenBorderRadius}px`;
            this.getInput("TableShadow").value = "0px 0px 10px 0px rgba(0,0,0,0.1)";

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
            this.getColor("TabsTabBgFocus").value = this.ToFocus(values.GenTheme, values.GenBgShaded);
            this.getColor("TabsTabClrFocus").value = this.ToFocus(values.GenTheme, values.GenClrShaded);
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
                GenBorderClr: this.getInput("GenBorderClr").value,
                GenBorderWidth: Number(this.getInput("GenBorderWidth").value),
                GenBorderRadius: Number(this.getInput("GenBorderRadius").value),
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
            return $YetaWF.getElement1BySelector(`input[name='${name}']`, [this.Form]) as HTMLInputElement;
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
