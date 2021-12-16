namespace YetaWF_Basics {

    export class ModuleSkinInfoModule extends YetaWF.ModuleBaseNoDataImpl {

        public static readonly SELECTOR: string = ".YetaWF_Languages_ModuleSkinInfo";

        constructor(id: string) {
            super(id, ModuleSkinInfoModule.SELECTOR, null);

            // The text is embedded in a <span> which allows us the get the exact width (instead of the div's 100%)
            // get width/height
            const textElem = $YetaWF.getElement1BySelector(".t_row.t_characters .yt_textarea .t_chars", [this.Module]);
            const rect = textElem.getBoundingClientRect();
            const width = rect.width;
            const height = rect.height;
            // show width/height
            const letterWidthDisp = $YetaWF.getElement1BySelector(".t_row.t_letterswidth .t_vals", [this.Module]);
            letterWidthDisp.textContent = width.toFixed(2);
            const letterHeightDisp = $YetaWF.getElement1BySelector(".t_row.t_lettersheight .t_vals", [this.Module]);
            letterHeightDisp.textContent = height.toFixed(2);
            // calculate width/height and show in property list
            const widthDisp = $YetaWF.getElement1BySelector(".t_row.t_width .t_vals", [this.Module]);
            widthDisp.textContent = ( width /(26+26+10)).toFixed(2);
            const heightDisp = $YetaWF.getElement1BySelector(".t_row.t_height .t_vals", [this.Module]);
            heightDisp.textContent = (height/2).toFixed(2);
        }
    }
}
