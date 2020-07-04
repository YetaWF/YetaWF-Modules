/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/Feed#License */

namespace YetaWF_Feed {

    interface Setup {
        Interval: number;
    }

    export class Feed {

        private Div: HTMLDivElement;
        private DivHeader: HTMLDivElement;
        private NextEntry: number = 0;
        private Entries: HTMLAnchorElement[];

        private EntryTimer: number | null = null;

        constructor(divId: string, setup: Setup) {

            this.Div = $YetaWF.getElementById(divId) as HTMLDivElement;
            this.DivHeader = $YetaWF.getElement1BySelector(".t_headerentry", [this.Div]) as HTMLDivElement;
            this.Entries = $YetaWF.getElementsBySelector(".t_entry a", [this.Div]) as HTMLAnchorElement[];

            this.changeEntry();
            if (setup.Interval)
                this.EntryTimer = setInterval(() : void => { this.changeEntry(); }, setup.Interval);

            // change all a tags to open a new window
            var elems = $YetaWF.getElementsBySelector("a", [this.Div]);
            for (let elem of elems) {
                $YetaWF.setAttribute(elem, "target", "_blank");
                $YetaWF.setAttribute(elem, "rel", "noopener noreferrer");
            }

            // Listen for events that the page is changing
            $YetaWF.registerPageChange(true, (): void => {
                // when the page is removed, we need to clean up
                if (this.EntryTimer) {
                    clearInterval(this.EntryTimer);
                    this.EntryTimer = null;
                }
            });
        }
        private changeEntry(): void {

            var entry = this.Entries[this.NextEntry];

            var newsEntry = `<a class='t_title' href='${entry.href}' target='_blank' rel='noopener noreferrer'>${entry.innerText}</a>`;
            var text = $YetaWF.getAttributeCond(entry, "data-text") || "";
            newsEntry += `<div class='t_text'>${text}</div>`;
            var author = $YetaWF.getAttributeCond(entry, "data-author") || "";
            newsEntry += `<div class='t_author'>${author}</div>`;
            var formattedDate = $YetaWF.getAttributeCond(entry, "data-publishedDate") || "";
            newsEntry += `<div class='t_date'>${formattedDate}</div>`;

            this.DivHeader.innerHTML = newsEntry;

            ++this.NextEntry;
            if (this.NextEntry >= this.Entries.length)
                this.NextEntry = 0;
        }
    }
}

