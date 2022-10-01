/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.Search.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

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
            public async Task RemoveUnusedUrlsAsync(DataProvider.SearchDataProvider searchDP) {
                using (DataProvider.SearchDataUrlDataProvider searchUrlDP = new DataProvider.SearchDataUrlDataProvider()) {
                    string sql = @"
DELETE {UrlTableName}
FROM {UrlTableName}
LEFT JOIN {TableName} ON {UrlTableName}.[SearchDataUrlId] = {TableName}.[SearchDataUrlId]
WHERE {TableName}.[SearchDataUrlId] IS NULL";
                    ISQLTableInfo info = await searchUrlDP.GetDataProvider().GetISQLTableInfoAsync();
                    sql = sql.Replace("{UrlTableName}", SQLBuilder.WrapIdentifier(info.GetTableName()));
                    await Direct_QueryAsync(sql);
                }
            }

            public async Task MarkUpdatedAsync(int searchDataUrlId) {
                string sql = $@"UPDATE {{TableName}} Set DateAdded = GETUTCDATE() WHERE [SearchDataUrlId] = {{UrlId}} AND {{__Site}}";
                sql = sql.Replace("{UrlId}", searchDataUrlId.ToString());
                await Direct_QueryAsync(sql);
            }
        }
        class SearchResultDataProvider : SQLSimpleObject<int, SearchResult> {
            public SearchResultDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class SearchDataUrlDataProvider : SQLSimpleIdentityObject<string, SearchDataUrl> {
            public SearchDataUrlDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
