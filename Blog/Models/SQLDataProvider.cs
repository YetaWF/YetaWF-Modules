﻿/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Support;
using YetaWF.DataProvider.SQL2;

namespace YetaWF.Modules.Blog.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.BlogCategoryDataProvider), typeof(BlogCategoryDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.BlogEntryDataProvider), typeof(BlogEntryDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.BlogCommentDataProvider), typeof(BlogCommentDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.BlogConfigDataProvider), typeof(BlogConfigDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.DisqusConfigDataProvider), typeof(DisqusConfigDataProvider));
        }
        class BlogCategoryDataProvider : SQLSimpleObject<int, BlogCategory> {
            public BlogCategoryDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class BlogEntryDataProvider : SQLSimpleObject<int, BlogEntry> {
            public BlogEntryDataProvider(Dictionary<string, object> options) : base(options) { CalculatedPropertyCallback = GetCalculatedProperty; }
            private string GetCalculatedProperty(string name) {
                string sql = null;
                if (name == "CommentsUnapproved") {
                    using (DataProvider.BlogCommentDataProvider commentDP = new DataProvider.BlogCommentDataProvider(-1)) {// we don't know the entry, but it's not needed
                        sql = "SELECT COUNT(*) FROM $BlogComments$ WHERE ($BlogComments$.EntryIdentity = $ThisTable$.[Identity]) AND ($ThisTable$.Published = 1) AND ($BlogComments$.Deleted = 0) AND ($BlogComments$.Approved = 0)";
                        sql = commentDP.ReplaceWithTableName(sql, "$BlogComments$");
                        sql = ReplaceWithTableName(sql, "$ThisTable$");
                        return sql;
                    }
                } else if (name == "Comments") {
                    using (DataProvider.BlogCommentDataProvider commentDP = new DataProvider.BlogCommentDataProvider(-1)) {// we don't know the entry, but it's not needed
                        sql = "SELECT COUNT(*) FROM $BlogComments$ WHERE ($BlogComments$.EntryIdentity = $ThisTable$.[Identity]) AND ($ThisTable$.Published = 1)";
                        sql = commentDP.ReplaceWithTableName(sql, "$BlogComments$");
                        sql = ReplaceWithTableName(sql, "$ThisTable$");
                        return sql;
                    }
                } else
                    throw new InternalError("Unexpected property {0}", name);
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
}
