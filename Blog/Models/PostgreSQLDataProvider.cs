/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Support;
using YetaWF.DataProvider.PostgreSQL;

namespace YetaWF.Modules.Blog.DataProvider.PostgreSQL;

public class PostgreSQLDataProvider : IExternalDataProvider {

    public void Register() {
        DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.BlogCategoryDataProvider), typeof(BlogCategoryDataProvider));
        DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.BlogEntryDataProvider), typeof(BlogEntryDataProvider));
        DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.BlogCommentDataProvider), typeof(BlogCommentDataProvider));
        DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.BlogConfigDataProvider), typeof(BlogConfigDataProvider));
        DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.DisqusConfigDataProvider), typeof(DisqusConfigDataProvider));
    }
    class BlogCategoryDataProvider : SQLSimpleObject<int, BlogCategory> {
        public BlogCategoryDataProvider(Dictionary<string, object> options) : base(options) { }
    }
    class BlogEntryDataProvider : SQLSimpleObject<int, BlogEntry> {
        public BlogEntryDataProvider(Dictionary<string, object> options) : base(options) { CalculatedPropertyCallbackAsync = GetCalculatedPropertyAsync; }
        private async Task<string> GetCalculatedPropertyAsync(string name) {
            string sql;
            if (name == "CommentsUnapproved") {
                using (DataProvider.BlogCommentDataProvider commentDP = new DataProvider.BlogCommentDataProvider(-1)) {// we don't know the entry, but it's not needed
                    sql = "SELECT COUNT(*) FROM $BlogComments$ WHERE ($BlogComments$.\"EntryIdentity\" = $ThisTable$.\"Identity\") AND ($ThisTable$.\"Published\" = True) AND ($BlogComments$.\"Deleted\" = False) AND ($BlogComments$.\"Approved\" = False)";
                    IPostgreSQLTableInfo info = await commentDP.GetDataProvider().GetIPostgreSQLTableInfoAsync();
                    sql = info.ReplaceWithTableName(sql, "$BlogComments$");
                    sql = ReplaceWithTableName(sql, "$ThisTable$");
                }
            } else if (name == "Comments") {
                using (DataProvider.BlogCommentDataProvider commentDP = new DataProvider.BlogCommentDataProvider(-1)) {// we don't know the entry, but it's not needed
                    sql = "SELECT COUNT(*) FROM $BlogComments$ WHERE ($BlogComments$.\"EntryIdentity\" = $ThisTable$.\"Identity\") AND ($ThisTable$.\"Published\" = True)";
                    IPostgreSQLTableInfo info = await commentDP.GetDataProvider().GetIPostgreSQLTableInfoAsync();
                    sql = info.ReplaceWithTableName(sql, "$BlogComments$");
                    sql = ReplaceWithTableName(sql, "$ThisTable$");
                }
            } else
                throw new InternalError("Unexpected property {0}", name);
            return sql;
        }
    }
    class BlogCommentDataProvider : SQLSimpleObject<int, BlogComment> {
        public BlogCommentDataProvider(Dictionary<string, object> options) : base(options) { }
    }
    class BlogConfigDataProvider : SQLSimpleObject<int, BlogConfigData> {
        public BlogConfigDataProvider(Dictionary<string, object> options) : base(options) { }
    }
    class DisqusConfigDataProvider : SQLSimpleObject<int, DisqusConfigData> {
        public DisqusConfigDataProvider(Dictionary<string, object> options) : base(options) { }
    }
}
