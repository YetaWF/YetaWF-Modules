using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.PostgreSQL;

namespace YetaWF.Modules.Templates.DataProvider.PostgreSQL;

public class TemplateDefaultsSQLDataProvider : IExternalDataProvider {

    public void Register() {
        DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.TemplateDefaultsDataProvider), typeof(SQLSimpleObject<int, TemplateDefaults>));
    }
}
