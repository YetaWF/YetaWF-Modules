/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
            // icons used is fas-times
            let dialog = <div id="yDialogContainer" tabindex="-1" role="dialog" aria-describedby="yAlert" aria-labelledby="yAlertTitle">
                <div class="t_titlebar">
                    <span id="yAlertTitle" class="t_title">{setup.title}</span>
                    <button type="button" class="y_buttonlite t_close" data-tooltip={setup.closeText??""}></button>
                </div>
                <div id="yAlert" class="t_content" style="width: auto; min-height: 25px; max-height: none; height: auto;"></div>
                <div class="t_buttonpane">
                    <div class="t_buttons">
                    </div>
                </div>
            </div> as HTMLDivElement;

            $YetaWF.getElement1BySelector("#yAlert", [dialog]).innerHTML = setup.textHTML;

            // handle all buttons
            let buttonSet = $YetaWF.getElement1BySelector(".t_buttons", [dialog]);
            if (setup.buttons) {
                for (let buttonDef of setup.buttons) {
                    let button = <button type="button" class="y_button">{buttonDef.text}</button> as HTMLButtonElement;
                    buttonSet.appendChild(button);
                    $YetaWF.registerEventHandler(button, "click", null, (ev: MouseEvent): boolean => {
                        DialogClass.close();
                        buttonDef.click();
                        return false;
                    });
                }
            }

            // handle close button
            let closeButton = $YetaWF.getElement1BySelector("button.t_close", [dialog]);
            // icon used fax-multiply
            closeButton.innerHTML = "<svg aria-hidden='true' focusable='false' role='img' xmlns='http://www.w3.org/2000/svg' viewBox='0 0 352 512'><path fill='currentColor' d='M242.72 256l100.07-100.07c12.28-12.28 12.28-32.19 0-44.48l-22.24-22.24c-12.28-12.28-32.19-12.28-44.48 0L176 189.28 75.93 89.21c-12.28-12.28-32.19-12.28-44.48 0L9.21 111.45c-12.28 12.28-12.28 32.19 0 44.48L109.28 256 9.21 356.07c-12.28 12.28-12.28 32.19 0 44.48l22.24 22.24c12.28 12.28 32.2 12.28 44.48 0L176 322.72l100.07 100.07c12.28 12.28 32.2 12.28 44.48 0l22.24-22.24c12.28-12.28 12.28-32.19 0-44.48L242.72 256z'></path></svg>";
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
            $YetaWF.registerEventHandler(dialog, "mousedown", ".t_titlebar", (ev: MouseEvent): boolean => {
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
            document.body.appendChild(dialog);

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
            let dialog = <div id="yDialogContainer" tabindex="-1" role="dialog" aria-describedby="yAlert" aria-labelledby="yAlertTitle">
                <div class="t_titlebar">
                    <span id="yAlertTitle" class="t_title">{setup.title}</span>
                </div>
                <div id="yAlert" class="t_content" style="width: auto; min-height: 25px; max-height: none; height: auto;"></div>
            </div> as HTMLDivElement;

            $YetaWF.getElement1BySelector("#yAlert", [dialog]).innerHTML = setup.textHTML;

            dialog.style.display = "none";
            document.body.appendChild(dialog);

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
            let overlay = $YetaWF.getElementById("yDialogOverlay");
            overlay.remove();
        }
        static get isActive(): boolean {
            return $YetaWF.getElementByIdCond("yDialogContainer") != null;
        }

        private static addOverlay(): void {
            let overlay = <div id="yDialogOverlay"></div> as HTMLDivElement;
            document.body.appendChild(overlay);
        }
        private static setupDragDrop(): void {
            let dialog = $YetaWF.getElementById("yDialogContainer");
            $YetaWF.registerEventHandler(dialog, "mousedown", ".t_titlebar", (ev: MouseEvent): boolean => {
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

