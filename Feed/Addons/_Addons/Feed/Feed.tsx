/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feed#License */

namespace YetaWF_Feed {

    interface Setup {
        Interval: number;
    }

    export class FeedModule extends YetaWF.ModuleBaseDataImpl {

        public static readonly SELECTOR: string = ".YetaWF_Feed_Feed";

        private DivHeader: HTMLDivElement;
        private NextEntry: number = 0;
        private Entries: HTMLAnchorElement[];

        private EntryTimer: number | null = null;

        constructor(id: string, setup: Setup) {
            super(id, FeedModule.SELECTOR, null, (tag: HTMLElement, module: FeedModule) : void => {
                // when the page is removed, we need to clean up
                if (module.EntryTimer) {
                    clearInterval(module.EntryTimer);
                    module.EntryTimer = null;
                }
            });

            this.DivHeader = $YetaWF.getElement1BySelector(".t_headerentry", [this.Module]) as HTMLDivElement;
            this.Entries = $YetaWF.getElementsBySelector(".t_entry a", [this.Module]) as HTMLAnchorElement[];

            this.changeEntry();
            if (setup.Interval) {
                this.EntryTimer = setInterval(() : void => {
                    this.changeEntry();
                }, setup.Interval);
            }

            // change all a tags to open a new window
            let elems = $YetaWF.getElementsBySelector("a", [this.Module]);
            for (let elem of elems) {
                $YetaWF.setAttribute(elem, "target", "_blank");
                $YetaWF.setAttribute(elem, "rel", "noopener noreferrer");
            }
        }

        private changeEntry(): void {

            let entry = this.Entries[this.NextEntry];

            let newsEntry = `<a class='t_title' href='${entry.href}' target='_blank' rel='noopener noreferrer'>${entry.innerText}</a>`;
            let text = $YetaWF.getAttributeCond(entry, "data-text") || "";
            newsEntry += `<div class='t_text'>${text}</div>`;
            let author = $YetaWF.getAttributeCond(entry, "data-author") || "";
            newsEntry += `<div class='t_author'>${author}</div>`;
            let formattedDate = $YetaWF.getAttributeCond(entry, "data-publishedDate") || "";
            newsEntry += `<div class='t_date'>${formattedDate}</div>`;

            this.DivHeader.innerHTML = newsEntry;

            ++this.NextEntry;
            if (this.NextEntry >= this.Entries.length)
                this.NextEntry = 0;
        }
    }
}

