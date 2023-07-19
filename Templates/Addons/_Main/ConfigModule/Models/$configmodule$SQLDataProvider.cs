using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace $companynamespace$.Modules.$projectnamespace$.DataProvider.SQL;

public class $modelname$SQLDataProvider : IExternalDataProvider {
    public void Register() {
        DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.$modelname$DataProvider), typeof($modelname$DataProvider));
    }
    class $modelname$DataProvider : SQLSimpleObject<int, $modelname$> {
        public $modelname$DataProvider(Dictionary<string, object> options) : base(options) { }
    }
}
