/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.PostgreSQL;

namespace YetaWF.Modules.Search.DataProvider.PostgreSQL;

public class PostgreSQLDataProvider : IExternalDataProvider {

    public void Register() {
        DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.SearchConfigDataProvider), typeof(SearchConfigDataProvider));
        DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.SearchDataProvider), typeof(SearchDataProvider));
        DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.SearchResultDataProvider), typeof(SearchResultDataProvider));
        DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.SearchDataUrlDataProvider), typeof(SearchDataUrlDataProvider));
    }
    class SearchConfigDataProvider : SQLSimpleObject<int, SearchConfigData> {
        public SearchConfigDataProvider(Dictionary<string, object> options) : base(options) { }
    }
    class SearchDataProvider : SQLSimpleObject<int, SearchData>, ISearchDataProviderIOMode {

        public SearchDataProvider(Dictionary<string, object> options) : base(options) { }

        public async Task RemoveUnusedUrlsAsync(DataProvider.SearchDataProvider searchDP) {//$$$$
            using (DataProvider.SearchDataUrlDataProvider searchUrlDP = new DataProvider.SearchDataUrlDataProvider()) {
                string sql = @"
DELETE FROM {UrlTableName} USING {TableName}
WHERE {UrlTableName}.""SearchDataUrlId"" = {TableName}.""SearchDataUrlId""
        AND {TableName}.""SearchDataUrlId"" IS NULL";
                IPostgreSQLTableInfo info = await searchUrlDP.GetDataProvider().GetIPostgreSQLTableInfoAsync();
                sql = sql.Replace("{UrlTableName}", SQLBuilder.WrapIdentifier(info.GetTableName()));
                await Direct_QueryAsync(sql);
            }
        }

        public async Task MarkUpdatedAsync(int searchDataUrlId) {
            string sql = $@"UPDATE {{TableName}} Set ""DateAdded"" = @p2 WHERE ""SearchDataUrlId"" = @p1 AND {{__Site}}";
            await Direct_QueryAsync(sql, searchDataUrlId, DateTime.UtcNow);
        }
    }
    class SearchResultDataProvider : SQLSimpleObject<int, SearchResult> {
        public SearchResultDataProvider(Dictionary<string, object> options) : base(options) { }
    }
    class SearchDataUrlDataProvider : SQLSimpleIdentityObject<string, SearchDataUrl> {
        public SearchDataUrlDataProvider(Dictionary<string, object> options) : base(options) { }
    }
}
