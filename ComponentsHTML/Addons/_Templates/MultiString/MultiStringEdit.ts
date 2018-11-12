/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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

        public static readonly SELECTOR: string = ".yt_multistring.t_edit";

        //private Setup: Setup;
        private SelectLang: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private InputText: HTMLInputElement;

        constructor(controlId: string/*, setup: Setup*/) {
            super(controlId);
            //this.Setup = setup;

            this.SelectLang = YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromSelector("select", [this.Control]);
            this.InputText = $YetaWF.getElement1BySelector("input.yt_multistring_text", [this.Control]) as HTMLInputElement;

            // selection change (put language specific text into text box)
            this.SelectLang.Control.addEventListener("dropdownlist_change", (evt: Event): void => {
                var sel = this.SelectLang.selectedIndex;
                var hid = $YetaWF.getElement1BySelector(`input[name$='[${sel}].value']`, [this.Control]) as HTMLInputElement;
                var newText = hid.value;
                if (newText.length === 0 && sel > 0) {
                    var hid0 = $YetaWF.getElement1BySelector("input[name$='[0].value']", [this.Control]) as HTMLInputElement;
                    newText = hid0.value;
                    hid.value = newText;
                }
                this.InputText.value = newText;
            });

            // textbox change (save text in language specific hidden fields)
            $YetaWF.registerEventHandler(this.InputText, "input", null, (ev: Event): boolean => {
                var sel = this.SelectLang.selectedIndex;
                var newText = this.InputText.value;
                var hid = $YetaWF.getElement1BySelector(`input[name$='[${sel}].value']`, [this.Control]) as HTMLInputElement;
                hid.value = newText;
                if (sel === 0)
                    this.SelectLang.enable(newText.length > 0);
                return false;
            });
            $YetaWF.registerEventHandler(this.InputText, "blur", null, (ev: Event): boolean => {
                var sel = this.SelectLang.selectedIndex;
                if (sel === 0) {
                    var hid0 = $YetaWF.getElement1BySelector("input[name$='[0].value']", [this.Control]) as HTMLInputElement;
                    var text = hid0.value;
                    if (text.length === 0) {
                        // the default text was cleared, clear all languages
                        var count = YLocs.YetaWF_ComponentsHTML.Languages.length;
                        for (var index = 0; index < count; ++index) {
                            var hid = $YetaWF.getElement1BySelector(`input[name$='[${index}].value']`, [this.Control]) as HTMLInputElement;
                            hid.value = "";
                        }
                    }
                }
                return false;
            });
        }

        // API

        public enable(enabled: boolean): void {
            $YetaWF.elementEnableToggle(this.InputText, enabled);
            this.SelectLang.enable(enabled && YConfigs.YetaWF_ComponentsHTML.Localization);
        }
        public clear(): void {
            this.SelectLang.clear();
            var hids = $YetaWF.getElementsBySelector(`input[name$='.value']`, [this.Control]) as HTMLInputElement[];
            for (let hid of hids)
                hid.value = "";
            this.InputText.value = "";
        }
        get defaultValue(): string {
            var hid0 = $YetaWF.getElement1BySelector("input[name$='[0].value']", [this.Control]) as HTMLInputElement;
            return hid0.value;
        }
        public hasChanged(data: string[]): boolean {
            var text = this.InputText.value;
            var count = YLocs.YetaWF_ComponentsHTML.Languages.length;
            for (var index = 0; index < count; ++index) {
                var hid = $YetaWF.getElement1BySelector(`input[name$='[${index}].value']`, [this.Control]) as HTMLInputElement;
                var langText = hid.value;
                if (langText.trim() === "")
                    langText = text;
                var lang = YLocs.YetaWF_ComponentsHTML.Languages[index];
                if (data[lang] != null && !$YetaWF.stringCompare(data[lang], langText))
                    return true;
            }
            return false;
        }
        public retrieveData(data: string[]): boolean {
            var newText = this.InputText.value;
            var sel = this.SelectLang.selectedIndex;
            var hid = $YetaWF.getElement1BySelector(`input[name$='[${sel}].value']`, [this.Control]) as HTMLInputElement;
            hid.value = newText;

            // now check whether it actually changed
            // if nothing is specified for a language, save what is entered in the text box
            var changed = false;
            var count = YLocs.YetaWF_ComponentsHTML.Languages.length;
            for (var index = 0; index < count; ++index) {
                hid = $YetaWF.getElement1BySelector(`input[name$='[${index}].value']`, [this.Control]) as HTMLInputElement;
                var langText = hid.value.trim();
                if (langText === "")
                    langText = newText;
                var lang = YLocs.YetaWF_ComponentsHTML.Languages[index];
                if (!$YetaWF.stringCompare(data[lang], langText)) {
                    changed = true;
                    data[lang] = langText;
                }
            }
            return changed;
        }
        public update(data: string[]): void {
            var count = YLocs.YetaWF_ComponentsHTML.Languages.length;
            for (var index = 0; index < count; ++index) {
                var lang = YLocs.YetaWF_ComponentsHTML.Languages[index];
                var s = "";
                if (data.hasOwnProperty(lang))
                    s = data[lang];
                else if (data.hasOwnProperty(YLocs.YetaWF_ComponentsHTML.Languages[0]))
                    s = data[lang] = data[YLocs.YetaWF_ComponentsHTML.Languages[0]];// use default for languages w/o data
                else
                    throw "No language data";
                var hid = $YetaWF.getElement1BySelector(`input[name$='[${index}].value']`, [this.Control]) as HTMLInputElement;
                hid.value = s;
                if (index === 0)
                    this.InputText.value = s;
            }
            this.SelectLang.clear();
        }
    }
    if (YLocs.YetaWF_ComponentsHTML.Languages === undefined) throw "YLocs.YetaWF_ComponentsHTML.Languages missing";/*DEBUG*/

    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        MultiStringEditComponent.clearDiv(tag, MultiStringEditComponent.SELECTOR);
    });
}


