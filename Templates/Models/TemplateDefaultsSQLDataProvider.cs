using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.Templates.DataProvider.SQL;

public class TemplateDefaultsSQLDataProvider : IExternalDataProvider {

    public void Register() {
        DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.TemplateDefaultsDataProvider), typeof(SQLSimpleObject<int, TemplateDefaults>));
    }
}
