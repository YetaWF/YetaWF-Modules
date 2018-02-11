/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Collections.Generic;
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
            public void RemoveUnusedUrls(DataProvider.SearchDataProvider searchDP) {
                using (DataProvider.SearchDataUrlDataProvider searchUrlDP = new DataProvider.SearchDataUrlDataProvider()) {
                    string sql = @"
DELETE {UrlTableName}
FROM {UrlTableName}
LEFT JOIN {TableName} ON {UrlTableName}.[SearchDataUrlId] = {TableName}.[SearchDataUrlId]
WHERE {TableName}.[SearchDataUrlId] IS NULL";
                        ISQLTableInfo info = (ISQLTableInfo)searchUrlDP.GetDataProvider();
                        sql = sql.Replace("{UrlTableName}", SQLBuilder.WrapBrackets(info.GetTableName()));
                        Direct_Query(GetTableName(), sql);
                }
            }

            public void MarkUpdated(int searchDataUrlId) {
                string sql = $@"UPDATE {{TableName}} Set DateAdded = GETUTCDATE() WHERE [SearchDataUrlId] = {{UrlId}} AND {{__Site}}";
                sql = sql.Replace("{UrlId}", searchDataUrlId.ToString());
                Direct_Query(GetTableName(), sql);
            }
        }
        class SearchResultDataProvider : SQLSimpleObject<int, SearchResult> {
            public SearchResultDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class SearchDataUrlDataProvider : SQLSimpleIdentityObject<int, SearchDataUrl> {
            public SearchDataUrlDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
