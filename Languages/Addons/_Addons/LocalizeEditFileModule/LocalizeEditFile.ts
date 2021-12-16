
namespace YetaWF {
    export interface ILocs {
        YetaWF_Languages: YetaWF_Languages.IPackageLocs;
    }
}

namespace YetaWF_Languages {

    export interface IPackageLocs {
        ConfirmResetText: string;
    }

    export class LocalizeEditFileModule extends YetaWF.ModuleBaseNoDataImpl {

        public static readonly SELECTOR: string = ".YetaWF_Languages_LocalizeEditFile";
        private ResetButton: HTMLInputElement;

        constructor(id: string) {
            super(id, LocalizeEditFileModule.SELECTOR, null);

            this.ResetButton = $YetaWF.getElement1BySelector("input[name='Reset']", [this.Module]) as HTMLInputElement;

            let form = $YetaWF.Forms.getInnerForm(this.Module);
            $YetaWF.registerEventHandler(this.ResetButton, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.alertYesNo(YLocs.YetaWF_Languages.ConfirmResetText, undefined, (): void => {
                    $YetaWF.Forms.submit(form, true, "RestoreDefaults=true");
                });
                return false;
            });
        }
    }
}
