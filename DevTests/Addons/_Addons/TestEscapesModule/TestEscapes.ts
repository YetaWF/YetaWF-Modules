/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

namespace YetaWF_DevTests {

    declare var YetaWF_Panels: any; // don't want to reference package for this

    export class TestEscapesModule extends YetaWF.ModuleBaseNoDataImpl {

        public static readonly SELECTOR: string = ".YetaWF_DevTests_TestEscapes";

        constructor(id: string) {
            super(id, TestEscapesModule.SELECTOR, null);

            $YetaWF.registerEventHandler(this.Module, "click", "input[name='message']", (ev: MouseEvent): boolean => {
                $YetaWF.message("TEST <A> &amp; & @ {0} TEST");
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='error']", (ev: MouseEvent): boolean => {
                $YetaWF.error("TEST <A> &amp; & @ {{0}} TEST");
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='alert']", (ev: MouseEvent): boolean => {
                $YetaWF.alert("TEST <A> &amp; & @ {{0}} TEST", "TITLE <A> &amp; & @ {{0}} TEST");
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='confirm']", (ev: MouseEvent): boolean => {
                $YetaWF.confirm("TEST <A> &amp; & @ {{0}} TEST");
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='alertYesNo']", (ev: MouseEvent): boolean => {
                $YetaWF.alertYesNo("TEST <A> &amp; & @ {{0}} TEST", "TITLE <A> &amp; & @ {{0}} TEST");
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='pleaseWait']", (ev: MouseEvent): boolean => {
                $YetaWF.pleaseWait("Reload page to continue\n\nTEST <A> &amp; & @ {{0}} TEST", "TITLE <A> &amp; & @ {{0}} TEST");
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='jserror']", (ev: MouseEvent): boolean => {
                // generate a javascript error (use eval to prevent build errors)
                let s = "aaa.bbb.ccc = 10;";
                // tslint:disable-next-line:no-eval
                eval(s);
                return true;
            });
        }
    }
}

