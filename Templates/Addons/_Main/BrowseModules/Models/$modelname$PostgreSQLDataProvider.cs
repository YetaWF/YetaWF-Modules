using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.PostgreSQL;

namespace $companynamespace$.Modules.$projectnamespace$.DataProvider.PostgreSQL;

public class $modelname$SQLDataProvider : IExternalDataProvider {

    public void Register() {
        DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.$modelname$DataProvider), typeof(SQLSimpleObject<int, $modelname$>));
    }
}
