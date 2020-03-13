/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.PostgreSQL;

namespace YetaWF.Modules.Visitors.DataProvider.PostgreSQL {

    public class PostgreSQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.VisitorEntryDataProvider), typeof(VisitorEntryDataProvider));
        }
        class VisitorEntryDataProvider : SQLSimpleObject<int, VisitorEntry>, VisitorEntryDataProviderIOMode {

            public VisitorEntryDataProvider(Dictionary<string, object> options) : base(options) { }

            public async Task<DataProvider.VisitorEntryDataProvider.Info> GetStatsAsync() {//$$$$$$$$$$$
                DataProvider.VisitorEntryDataProvider.Info info = new DataProvider.VisitorEntryDataProvider.Info();
                DateTime now = DateTime.Now.Date.ToUniversalTime();
                string startDate = string.Format("{0}", now);
                string endDate = string.Format("{0}", now.AddDays(1));
                info.TodaysAnonymous = await Direct_ScalarIntAsync(
                    "SELECT Count(DISTINCT SessionId) FROM {TableName} Where " +
                        string.Format("AccessDateTime >= '{0}' AND AccessDateTime < '{1}'", startDate, endDate) +
                        " AND [UserId] = 0" +
                        " AND {__Site}"
                );
                info.TodaysUsers = await Direct_ScalarIntAsync(
                    "SELECT Count(DISTINCT UserId) FROM {TableName} Where " +
                        string.Format("AccessDateTime >= '{0}' AND AccessDateTime < '{1}'", startDate, endDate) +
                        " AND [UserId] <> 0" +
                        " AND {__Site}"
                );
                startDate = string.Format("{0}", now.AddDays(-1));
                endDate = string.Format("{0}", now);
                info.YesterdaysAnonymous = await Direct_ScalarIntAsync(
                    "SELECT Count(DISTINCT SessionId) FROM {TableName} Where " +
                        string.Format("AccessDateTime >= '{0}' AND AccessDateTime < '{1}'", startDate, endDate) +
                        " AND [UserId] = 0" +
                        " AND {__Site}"
                );
                info.YesterdaysUsers = await Direct_ScalarIntAsync(
                    "SELECT Count(DISTINCT UserId) FROM {TableName} Where " +
                        string.Format("AccessDateTime >= '{0}' AND AccessDateTime < '{1}'", startDate, endDate) +
                        " AND [UserId] <> 0" +
                        " AND {__Site}"
                );
                return info;
            }
            public async Task UpdateSameIPAddressesAsync(VisitorEntry visitorEntry) {
                string sql = $@"
                    UPDATE {GetTableName()}
                    SET [ContinentCode] = @p1, [CountryCode] = @p2, [RegionCode] = @p3, [City] = @p4
                    WHERE [IPAddress] = @p5 AND [ContinentCode] = @p6";

                await base.Direct_QueryAsync(sql,
                    visitorEntry.ContinentCode, visitorEntry.CountryCode, visitorEntry.RegionCode, visitorEntry.City,
                    visitorEntry.IPAddress, VisitorEntry.Unknown);
            }
        }
    }
}
