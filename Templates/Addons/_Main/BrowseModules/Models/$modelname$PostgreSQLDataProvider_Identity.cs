$pp $dp$ == SQLIdentity
using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.PostgreSQL;

namespace $companynamespace$.Modules.$projectnamespace$.DataProvider.PostgreSQL;

public class $modelname$SQLDataProvider : IExternalDataProvider {

    public void Register() {
        DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.$modelname$DataProvider), typeof($modelname$DataProvider));
    }
    class $modelname$DataProvider : SQLSimpleIdentityObject<$modelkey$, $modelname$> {
        public $modelname$DataProvider(Dictionary<string, object> options) : base(options) { }
    }
}
