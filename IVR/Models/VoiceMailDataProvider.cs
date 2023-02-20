/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Softelvdm.Modules.TwilioProcessor.DataProvider;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using YetaWF.Core;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace Softelvdm.Modules.IVR.DataProvider;

public class VoiceMailData {

    public const int MaxSid = 100;
    public const int MaxExtension = 10;
    public const int MaxCity = 100;
    public const int MaxState = 30;
    public const int MaxZip = 20;
    public const int MaxCountry = 100;

    [Data_Identity]
    public int Id { get; set; }

    public DateTime Created { get; set; }

    [Data_PrimaryKey, StringLength(MaxSid)]
    public string CallSid { get; set; } = null!;
    [Data_PrimaryKey, StringLength(MaxSid)]
    public string RecordingSid { get; set; } = null!;
    [Data_Index, StringLength(Globals.MaxPhoneNumber)]
    public string To { get; set; } = null!;
    [Data_Index, StringLength(MaxExtension)]
    public string Extension { get; set; } = null!;

    [StringLength(Globals.MaxUrl)]
    public string RecordingUrl { get; set; } = null!;

    public bool Heard { get; set; }

    [StringLength(Globals.MaxPhoneNumber)]
    public string Caller { get; set; } = null!;
    [StringLength(MaxCity)]
    public string CallerCity { get; set; } = null!;
    [StringLength(MaxState)]
    public string CallerState { get; set; } = null!;
    [StringLength(MaxZip)]
    public string CallerZip { get; set; } = null!;
    [StringLength(MaxCountry)]
    public string CallerCountry { get; set; } = null!;

    public int Duration { get; set; }

    public VoiceMailData() { }
}

public class VoiceMailDataProvider : DataProviderImpl, IInstallableModel {

    // IMPLEMENTATION
    // IMPLEMENTATION
    // IMPLEMENTATION

    public VoiceMailDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
    public VoiceMailDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

    private IDataProviderIdentity<string, object, VoiceMailData> DataProvider { get { return GetDataProvider(); } }

    private IDataProviderIdentity<string, object, VoiceMailData>? CreateDataProvider() {
        Package package = Softelvdm.Modules.IVR.AreaRegistration.CurrentPackage;
        return MakeDataProvider(package, package.AreaName + "_VoiceMails", SiteIdentity: SiteIdentity, Cacheable: true);
    }

    public async Task SetupClient() {

        if (!Initialized) {
            TwilioData config = await TwilioConfigDataProvider.GetConfigAsync();
            if (!config.IsConfigured())
                throw new Error(this.__ResStr("notConfigured", "Twilio is not configured."));
            string? acctSid = null, acctToken = null;
            if (config.TestMode) {
                acctSid = config.TestAccountSid;
                acctToken = config.TestAuthToken;
            } else {
                acctSid = config.LiveAccountSid;
                acctToken = config.LiveAuthToken;
            }
            TwilioClient.Init(acctSid, acctToken);
            Initialized = true;
        }
    }
    private static bool Initialized;

    // LOAD/SAVE
    // LOAD/SAVE
    // LOAD/SAVE

    public Task<VoiceMailData?> GetItemAsync(string callSid) {
        return DataProvider.GetAsync(callSid, null);
    }
    public Task<VoiceMailData?> GetItemByIdentityAsync(int id) {
        return DataProvider.GetByIdentityAsync(id);
    }
    public async Task<bool> AddItemAsync(VoiceMailData data) {
        data.Created = DateTime.UtcNow;
        if (!await DataProvider.AddAsync(data))
            return false;
        await Auditing.AddAuditAsync($"{nameof(VoiceMailDataProvider)}.{nameof(AddItemAsync)}", Dataset, Guid.Empty,
            $"Add Voice Mail Entry {data.Id}",
            DataBefore: null,
            DataAfter: data
        );
        return true;
    }
    public async Task UpdateItemAsync(VoiceMailData data) {
        VoiceMailData? origEmail = Auditing.Active ? await GetItemAsync(data.CallSid) : null;
        UpdateStatusEnum status = await DataProvider.UpdateByIdentityAsync(data.Id, data);
        if (status != UpdateStatusEnum.OK)
            throw new InternalError("Unexpected error {0} updating item", status);
        await Auditing.AddAuditAsync($"{nameof(VoiceMailDataProvider)}.{nameof(UpdateItemAsync)}", Dataset, Guid.Empty,
            $"Update Voice Mail Entry {data.Id}",
            DataBefore: origEmail,
            DataAfter: data
        );
    }
    public async Task<bool> RemoveItemByIdentityAsync(int id) {

        VoiceMailData? origData = Auditing.Active ? await GetItemByIdentityAsync(id) : null;

        if (!await DataProvider.RemoveByIdentityAsync(id))
            return false;

        await SetupClient();
        try {
            if (origData != null) {
                if (YetaWFManager.IsSync())
                    RecordingResource.Delete(pathSid: origData.RecordingSid);
                else
                    await RecordingResource.DeleteAsync(pathSid: origData.RecordingSid);
            }
        } catch (Exception) { }

        await Auditing.AddAuditAsync($"{nameof(VoiceMailDataProvider)}.{nameof(RemoveItemByIdentityAsync)}", Dataset, Guid.Empty,
            $"Remove Voice Mail Entry {id}",
            DataBefore: origData,
            DataAfter: null
        );
        return true;
    }
    public Task<DataProviderGetRecords<VoiceMailData>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) {
        return DataProvider.GetRecordsAsync(skip, take, sort, filters);
    }
}
