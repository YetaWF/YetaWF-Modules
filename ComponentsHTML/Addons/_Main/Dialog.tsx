/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export interface DialogSetup {
        width: number;
        height: number;
        title: string;
        textHTML: string;
        closeText?: string;
        close?: () => void;
        buttons?: ButtonDefinition[];
    }
    export interface ButtonDefinition {
        text: string;
        click: () => void;
    }

    export class DialogClass {

        private static Setup: DialogSetup;
        public static DragDropInProgress: boolean = false;
        public static XOffsetDD: number = 0;
        public static YOffsetDD: number = 0;

        public static open(setup: DialogSetup): void {

            DialogClass.Setup = setup;
            DialogClass.DragDropInProgress = false;
            let dialog = <div id="yDialogContainer" tabindex="-1" role="dialog" class="ui-dialog ui-corner-all ui-widget ui-widget-content ui-front ui-dialog-buttons ui-draggable" aria-describedby="yAlert" aria-labelledby="yAlertTitle">
                <div class="ui-dialog-titlebar ui-corner-all ui-widget-header ui-helper-clearfix ui-draggable-handle">
                    <span id="yAlertTitle" class="ui-dialog-title">{setup.title}</span>
                    <button type="button" class="ui-button ui-corner-all ui-widget ui-button-icon-only ui-dialog-titlebar-close" data-tooltip={setup.closeText??""}>
                        <span class="ui-button-icon ui-icon ui-icon-closethick"></span>
                        <span class="ui-button-icon-space"> </span>
                        Close
                    </button>
                </div>
                <div id="yAlert" class="ui-dialog-content ui-widget-content" style="width: auto; min-height: 25px; max-height: none; height: auto;"></div>
                <div class="ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
                    <div class="ui-dialog-buttonset">
                    </div>
                </div>
            </div> as HTMLDivElement;

            $YetaWF.getElement1BySelector("#yAlert", [dialog]).innerHTML = setup.textHTML;

            // handle all buttons
            let buttonSet = $YetaWF.getElement1BySelector(".ui-dialog-buttonset", [dialog]);
            if (setup.buttons) {
                for (let buttonDef of setup.buttons) {
                    let button = <button type="button" class="ui-button ui-corner-all ui-widget">{buttonDef.text}</button> as HTMLButtonElement;
                    buttonSet.append(button);
                    $YetaWF.registerEventHandler(button, "click", null, (ev: MouseEvent): boolean => {
                        DialogClass.close();
                        buttonDef.click();
                        return false;
                    });
                }
            }

            // handle close button
            let closeButton = $YetaWF.getElement1BySelector(".ui-dialog-titlebar button", [dialog]);
            $YetaWF.registerEventHandler(closeButton, "click", null, (ev: MouseEvent): boolean => {
                DialogClass.close();
                if (setup.close) setup.close();
                return false;
            });

            $YetaWF.registerEventHandler(dialog, "keydown", null, (ev: KeyboardEvent): boolean => {
                if (ev.key === "Escape") {
                    DialogClass.close();
                    if (setup.close) setup.close();
                    return false;
                }
                return true;
            });
            $YetaWF.registerEventHandler(dialog, "mousedown", ".ui-dialog-titlebar", (ev: MouseEvent): boolean => {
                let dialog = $YetaWF.getElementById("yDialogContainer");
                let drect = dialog.getBoundingClientRect();
                DialogClass.XOffsetDD = ev.clientX - drect.left;
                DialogClass.YOffsetDD = ev.clientY - drect.top;
                DialogClass.DragDropInProgress = true;
                return false;
            });
            $YetaWF.registerEventHandler(dialog, "mouseup", null, (ev: MouseEvent): boolean => {
                DialogClass.DragDropInProgress = false;
                return false;
            });

            dialog.style.display = "none";
            document.body.append(dialog);

            DialogClass.addOverlay();

            dialog.style.display = "";
            DialogClass.reposition();

            // set focus on first button
            $YetaWF.getElementsBySelector("button", [buttonSet])[0].focus();

            DialogClass.setupDragDrop();
        }
        public static openSimple(setup: DialogSetup): void {

            DialogClass.Setup = setup;
            DialogClass.DragDropInProgress = false;
            let dialog = <div id="yDialogContainer" tabindex="-1" role="dialog" class="ui-dialog ui-corner-all ui-widget ui-widget-content ui-front ui-dialog-buttons ui-draggable" aria-describedby="yAlert" aria-labelledby="yAlertTitle">
                <div class="ui-dialog-titlebar ui-corner-all ui-widget-header ui-helper-clearfix ui-draggable-handle">
                    <span id="yAlertTitle" class="ui-dialog-title">{setup.title}</span>
                </div>
                <div id="yAlert" class="ui-dialog-content ui-widget-content" style="width: auto; min-height: 25px; max-height: none; height: auto;"></div>
            </div> as HTMLDivElement;

            $YetaWF.getElement1BySelector("#yAlert", [dialog]).innerHTML = setup.textHTML;

            dialog.style.display = "none";
            document.body.append(dialog);

            DialogClass.addOverlay();

            dialog.style.display = "";
            DialogClass.reposition();
            DialogClass.setupDragDrop();
        }

        public static close(): void {
            DialogClass.DragDropInProgress = false;
            let dialog = $YetaWF.getElementByIdCond("yDialogContainer");
            if (!dialog) return;
            dialog.remove();
            let overlay = $YetaWF.getElement1BySelector(".ui-widget-overlay.ui-front");
            overlay.remove();
        }
        static get isActive(): boolean {
            return $YetaWF.getElementByIdCond("yDialogContainer") != null;
        }

        private static addOverlay(): void {
            let overlay = <div class="ui-widget-overlay ui-front"></div> as HTMLDivElement;
            document.body.append(overlay);
        }
        private static setupDragDrop(): void {
            let dialog = $YetaWF.getElementById("yDialogContainer");
            $YetaWF.registerEventHandler(dialog, "mousedown", ".ui-dialog-titlebar", (ev: MouseEvent): boolean => {
                let dialog = $YetaWF.getElementById("yDialogContainer");
                let drect = dialog.getBoundingClientRect();
                DialogClass.XOffsetDD = ev.clientX - drect.left;
                DialogClass.YOffsetDD = ev.clientY - drect.top;
                DialogClass.DragDropInProgress = true;
                return false;
            });
            $YetaWF.registerEventHandler(dialog, "mouseup", null, (ev: MouseEvent): boolean => {
                DialogClass.DragDropInProgress = false;
                return false;
            });
        }

        public static reposition(): void {
            DialogClass.DragDropInProgress = false;
            let dialog = $YetaWF.getElementByIdCond("yDialogContainer");
            if (!dialog) return;
            let setup = DialogClass.Setup;

            if (window.innerWidth <= setup.width) {
                dialog.style.width = `${window.innerWidth - 20}px`;
                dialog.style.height = setup.height > 0 ? `${setup.height}px` : "auto";
            } else {
                dialog.style.width = `${setup.width}px`;
                dialog.style.height = "auto";
            }

            // set maxheight for message text
            let text = $YetaWF.getElementById("yAlert");
            text.style.maxHeight = `${window.innerHeight * 3 / 4}px`;

            // center
            let drect = dialog.getBoundingClientRect();
            let left = (window.innerWidth - drect.width) / 2;
            let top = (window.innerHeight - drect.height) / 2;
            dialog.style.left = `${left}px`;  // or + window.pageXOffset if position:absolute
            dialog.style.top = `${top}px`; //  + window.pageYOffset
        }
    }
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, (ev: Event): boolean => {
        DialogClass.reposition();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: Event): boolean => {
        DialogClass.reposition();
        return true;
    });
    $YetaWF.registerEventHandlerBody("mousemove", null, (ev: MouseEvent): boolean => {
        if (DialogClass.DragDropInProgress) {
            let dialog = $YetaWF.getElementById("yDialogContainer");
            let drect = dialog.getBoundingClientRect();
            let left = ev.clientX - DialogClass.XOffsetDD;
            if (left + drect.width > window.innerWidth) left = window.innerWidth - drect.width;
            if (left < 0) left = 0;
            let top = ev.clientY - DialogClass.YOffsetDD;
            if (top + drect.height > window.innerHeight) top = window.innerHeight - drect.height;
            if (top < 0) top = 0;
            dialog.style.left = `${left}px`;
            dialog.style.top = `${top}px`;
            return false;
        }
        return true;
    });
}

