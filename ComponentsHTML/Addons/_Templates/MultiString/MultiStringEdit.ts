/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export interface IPackageLocs {
        Languages: string[];
    }
    export interface IPackageConfigs {
        Localization: boolean;
    }

    //interface Setup {
    //}

    export class MultiStringEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_multistring";
        public static readonly SELECTOR: string = ".yt_multistring.t_edit";

        //private Setup: Setup;
        private Hidden: HTMLInputElement;
        private SelectLang: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private InputText: HTMLInputElement;

        constructor(controlId: string/*, setup: Setup*/) {
            super(controlId, MultiStringEditComponent.TEMPLATE, MultiStringEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: (control: MultiStringEditComponent): string | null => {
                    return this.Hidden.value;
                },
                Enable: (control: MultiStringEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            });

            //this.Setup = setup;

            this.Hidden = $YetaWF.getElement1BySelector("input.t_multistring_hidden", [this.Control]) as HTMLInputElement;
            this.SelectLang = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select", DropDownListEditComponent.SELECTOR, [this.Control]);
            this.InputText = $YetaWF.getElement1BySelector("input.t_multistring_text", [this.Control]) as HTMLInputElement;

            // selection change (put language specific text into text box)
            this.SelectLang.Control.addEventListener("dropdownlist_change", (evt: Event): void => {
                let sel = this.SelectLang.selectedIndex;
                let hid = $YetaWF.getElement1BySelector(`input[name$='[${sel}].value']`, [this.Control]) as HTMLInputElement;
                let newText = hid.value;
                if (newText.length === 0 && sel > 0) {
                    var hid0 = $YetaWF.getElement1BySelector("input[name$='[0].value']", [this.Control]) as HTMLInputElement;
                    newText = hid0.value;
                    hid.value = newText;
                }
                this.InputText.value = newText;
            });

            // textbox change (save text in language specific hidden fields)
            $YetaWF.registerEventHandler(this.InputText, "input", null, (ev: Event): boolean => {
                let newText = this.InputText.value;
                let sel = this.SelectLang.selectedIndex;
                let hid = $YetaWF.getElement1BySelector(`input[name$='[${sel}].value']`, [this.Control]) as HTMLInputElement;
                hid.value = newText;
                if (sel === 0)
                    this.Hidden.value = newText;
                this.updateSelectLang();
                return false;
            });
            $YetaWF.registerEventHandler(this.InputText, "blur", null, (ev: Event): boolean => {
                let sel = this.SelectLang.selectedIndex;
                if (sel === 0) {
                    let hid0 = $YetaWF.getElement1BySelector("input[name$='[0].value']", [this.Control]) as HTMLInputElement;
                    let text = hid0.value;
                    if (text.length === 0) {
                        // the default text was cleared, clear all languages
                        let count = YLocs.YetaWF_ComponentsHTML.Languages.length;
                        for (var index = 0; index < count; ++index) {
                            var hid = $YetaWF.getElement1BySelector(`input[name$='[${index}].value']`, [this.Control]) as HTMLInputElement;
                            hid.value = "";
                        }
                        this.Hidden.value = "";
                    }
                    this.updateSelectLang();
                }
                return false;
            });
        }

        private updateSelectLang(): void {
            if (this.SelectLang.selectedIndex === 0)
                this.SelectLang.enable(YConfigs.YetaWF_ComponentsHTML.Localization && this.InputText.value.length > 0);
        }

        // API

        public enable(enabled: boolean): void {
            $YetaWF.elementEnableToggle(this.InputText, enabled);
            this.updateSelectLang();
        }
        public clear(): void {
            var hids = $YetaWF.getElementsBySelector(`input[name$='.value']`, [this.Control]) as HTMLInputElement[];
            for (let hid of hids)
                hid.value = "";
            this.Hidden.value = "";
            this.InputText.value = "";
            this.SelectLang.clear();
            this.updateSelectLang();
        }
        get defaultValue(): string {
            return this.InputText.value;
        }
        get value(): object {
            var data = {};

            var newText = this.InputText.value;
            var sel = this.SelectLang.selectedIndex;
            var hid = $YetaWF.getElement1BySelector(`input[name$='[${sel}].value']`, [this.Control]) as HTMLInputElement;
            hid.value = newText;

            var count = YLocs.YetaWF_ComponentsHTML.Languages.length;
            for (var index = 0; index < count; ++index) {
                hid = $YetaWF.getElement1BySelector(`input[name$='[${index}].value']`, [this.Control]) as HTMLInputElement;
                var langText = hid.value;
                if (langText === "")
                    langText = newText;
                var lang = YLocs.YetaWF_ComponentsHTML.Languages[index];
                data[lang] = langText;
            }
            return data;
        }
        set value(data: object) {
            var textDefault = this.findLanguageText(data, YLocs.YetaWF_ComponentsHTML.Languages[0]);
            var count = YLocs.YetaWF_ComponentsHTML.Languages.length;
            for (var index = 0; index < count; ++index) {
                var s = "";
                var lang = YLocs.YetaWF_ComponentsHTML.Languages[index];
                var text = this.findLanguageText(data, lang);
                if (text)
                    s = text;
                else if (textDefault)
                    s = textDefault;// use default for languages w/o data
                var hid = $YetaWF.getElement1BySelector(`input[name$='[${index}].value']`, [this.Control]) as HTMLInputElement;
                hid.value = s;
                if (index === 0) {
                    this.Hidden.value = s;
                    this.InputText.value = s;
                }
            }
            this.SelectLang.clear();
            this.updateSelectLang();
        }
        public hasChanged(data: object): boolean {
            var textDefault = this.findLanguageText(data, YLocs.YetaWF_ComponentsHTML.Languages[0]);
            var count = YLocs.YetaWF_ComponentsHTML.Languages.length;
            for (var index = 0; index < count; ++index) {
                var hid = $YetaWF.getElement1BySelector(`input[name$='[${index}].value']`, [this.Control]) as HTMLInputElement;
                var langText = hid.value;
                if (langText === "")
                    langText = textDefault||"";
                var lang = YLocs.YetaWF_ComponentsHTML.Languages[index];
                var ms = this.findLanguageText(data, lang);
                if (!ms)
                    ms = textDefault;
                if (!$YetaWF.stringCompare(ms, langText))
                    return true;
            }
            return false;
        }
        private findLanguageText(data: object, lang: string): string | null {
            if (!data.hasOwnProperty(lang)) return null;
            return data[lang];
        }
    }
    if (YLocs.YetaWF_ComponentsHTML.Languages === undefined) throw "YLocs.YetaWF_ComponentsHTML.Languages missing";/*DEBUG*/
}


