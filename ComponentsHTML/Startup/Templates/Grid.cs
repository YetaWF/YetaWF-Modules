/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    public class Grid : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string area = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddConfigOption(area, "allRecords", GridDefinition.MaxPages);

            scripts.AddLocalization(area, "allRecords", this.__ResStr("allRecords", "All"));

            scripts.AddLocalization(area, "recordtext", this.__ResStr("recordtext", "{0} - {1} of {2} items"));//{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
            scripts.AddLocalization(area, "emptyrecords", this.__ResStr("emptyrecords", "No items to display"));
            scripts.AddLocalization(area, "loadtext", this.__ResStr("loadtext", "Loading..."));
            scripts.AddLocalization(area, "pgtext", this.__ResStr("pgtext", "Page {0} of {1}"));//{0} is total amount of pages
            scripts.AddLocalization(area, "pgsearchTB", this.__ResStr("pgsearchTB", "Show/hide search toolbar"));
            scripts.AddLocalization(area, "pgfirst", this.__ResStr("pgfirst", "Display first page"));
            scripts.AddLocalization(area, "pglast", this.__ResStr("pglast", "Display last page"));
            scripts.AddLocalization(area, "pgnext", this.__ResStr("pgnext", "Display next page"));
            scripts.AddLocalization(area, "pgprev", this.__ResStr("pgprev", "Display previous page"));
            scripts.AddLocalization(area, "pgrecs", this.__ResStr("pgrecs", "Select number of items shown per page"));

            scripts.AddLocalization(area, "eq", this.__ResStr("eq", "Equal"));
            scripts.AddLocalization(area, "ne", this.__ResStr("ne", "Not equal"));
            scripts.AddLocalization(area, "lt", this.__ResStr("lt", "Less"));
            scripts.AddLocalization(area, "le", this.__ResStr("le", "Less or equal"));
            scripts.AddLocalization(area, "gt", this.__ResStr("gt", "Greater"));
            scripts.AddLocalization(area, "ge", this.__ResStr("ge", "Greater or equal"));
            scripts.AddLocalization(area, "bw", this.__ResStr("bw", "Begins with"));
            scripts.AddLocalization(area, "bn", this.__ResStr("bn", "Does not begin with"));
            scripts.AddLocalization(area, "inx", this.__ResStr("in", "Is in"));
            scripts.AddLocalization(area, "ni", this.__ResStr("ni", "Is not in"));
            scripts.AddLocalization(area, "ew", this.__ResStr("ew", "Ends with"));
            scripts.AddLocalization(area, "en", this.__ResStr("en", "Does not end with"));
            scripts.AddLocalization(area, "cn", this.__ResStr("cn", "Contains"));
            scripts.AddLocalization(area, "nc", this.__ResStr("nc", "Does not contain"));
            scripts.AddLocalization(area, "nu", this.__ResStr("nu", "Is null"));
            scripts.AddLocalization(area, "nn", this.__ResStr("nn", "Is not null"));

            return Task.CompletedTask;
        }
    }
}

