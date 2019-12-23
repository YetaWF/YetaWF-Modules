/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace Softelvdm.Modules.IVR.DataProvider.SQL {

    public class ConfigDataSQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.VoiceMailDataProvider), typeof(VoiceMailDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.IVRConfigDataProvider), typeof(IVRConfigDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.HolidayEntryDataProvider), typeof(HolidayEntryDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.ExtensionEntryDataProvider), typeof(ExtensionEntryDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.CallLogDataProvider), typeof(CallLogEntryDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.BlockedNumberDataProvider), typeof(BlockedNumberDataProvider));
        }
        class VoiceMailDataProvider : SQLSimpleIdentityObject<string, VoiceMailData> {
            public VoiceMailDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class IVRConfigDataProvider : SQLSimpleObject<int, IVRConfig> {
            public IVRConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class HolidayEntryDataProvider : SQLSimpleIdentityObject<DateTime, HolidayEntry> {
            public HolidayEntryDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class ExtensionEntryDataProvider : SQLSimpleIdentityObject<string, ExtensionEntry> {
            public ExtensionEntryDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class CallLogEntryDataProvider : SQLSimpleIdentityObject<int, CallLogEntry> {
            public CallLogEntryDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class BlockedNumberDataProvider : SQLSimpleIdentityObject<string, BlockedNumberEntry> {
            public BlockedNumberDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
