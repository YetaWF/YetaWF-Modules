/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateDateTimeModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateDateTimeModule>, IInstallableModel { }

    [ModuleGuid("{2c538164-9151-46b1-8118-c93bbcedd23f}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateDateTimeModule : ModuleDefinition {

        public TemplateDateTimeModule() {
            Title = this.__ResStr("modTitle", "DateTime Test Component");
            Name = this.__ResStr("modName", "Component Test - DateTime");
            Description = this.__ResStr("modSummary", "Test module for the DateTime component. A test page for this module can be found at Tests > Templates > DateTime (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateDateTimeModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Trim]
        public class Model {

            public enum ControlStatusEnum { Normal, Disabled, }

            [Category("Date/Time"), Caption("Date/Time (Required)"), Description("Date/Time (Required)")]
            [UIHint("DateTime")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public DateTime DateTimeReq { get; set; }
            //public DateTimeEditComponent.DateTimeSetup DateTimeReq_Setup {
            //    get {
            //        return new DateTimeEditComponent.DateTimeSetup {
            //            MinDate = new DateTime(2011, 1, 1),
            //            MaxDate = new DateTime(2029, 12, 31),
            //            MinTime = new TimeSpan(8, 0, 0).TotalMinutes,
            //            MaxTime = new TimeSpan(18, 0, 0).TotalMinutes,
            //        };
            //    } 
            //}

            [Category("Date/Time"), Caption("Date/Time"), Description("Date/Time")]
            [UIHint("DateTime")]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public DateTime? DateTimeOpt { get; set; }

            [Category("Date/Time"), Caption("Date/Time (Read/Only)"), Description("Date/Time (Read/only)")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime DateTimeRO { get; set; }

            [Category("Date/Time"), Caption("Date (Required)"), Description("Date (Required)")]
            [UIHint("Date")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public DateTime DateReq { get; set; }

            [Category("Date/Time"), Caption("Date"), Description("Date/Time")]
            [UIHint("Date")]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public DateTime? DateOpt { get; set; }

            [Category("Date/Time"), Caption("Date (Read/Only)"), Description("Date/Time (Read/only)")]
            [UIHint("Date"), ReadOnly]
            public DateTime DateRO { get; set; }

            //[Category("Date/Time"), Caption("Date UTC Midnight (Required)"), Description("Date (Required)")]
            //[UIHint("Date"), AdditionalMetadata("UTCMidnight", true)]
            //[RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            //[ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            //public DateTime DateUtcMidnightReq { get; set; }

            [Category("Date/Time"), Caption("Time Of Day (Required)"), Description("Time Of Day (Required)")]
            [UIHint("TimeOfDay")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public TimeOfDay TimeOfDayReq { get; set; }

            [Category("Date/Time"), Caption("Time Of Day"), Description("Time Of Day")]
            [UIHint("TimeOfDay")]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public TimeOfDay TimeOfDay { get; set; }

            [Category("Date/Time"), Caption("Time Of Day (Read/Only)"), Description("Time Of Day (Read/Only)")]
            [UIHint("TimeOfDay"), ReadOnly]
            public TimeOfDay TimeOfDayRO { get; set; }

            [Category("Date/Time"), Caption("Control Status"), Description("Defines the processing status of the controls")]
            [UIHint("Enum")]
            public ControlStatusEnum ControlStatus { get; set; }


            [Category("TimeSpan"), Caption("Timespan (Required)"), Description("Timespan (Required)")]
            [UIHint("TimeSpan")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public TimeSpan TimeSpanReq { get; set; }

            [Category("TimeSpan"), Caption("Timespan"), Description("Timespan")]
            [UIHint("TimeSpan")]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public TimeSpan TimeSpanOpt { get; set; }

            [Category("TimeSpan"), Caption("Timespan (Read/Only)"), Description("Timespan (Read/only)")]
            [UIHint("TimeSpan"), ReadOnly]
            public TimeSpan TimeSpanRO { get; set; }

            [Category("TimeSpan"), Caption("Timespan (Read/Only)"), Description("Timespan (Read/only)")]
            [UIHint("TimeSpan"), ReadOnly]
            public TimeSpan TimeSpanHMSRO { get; set; }

            [TextAbove("Weekly Hours")]
            [Category("Hours"), Caption(""), Description("")]
            [UIHint("WeeklyHours")]
            public WeeklyHours OpeningHours { get; set; }

            public Model() {
                DateTimeReq = DateTime.UtcNow;
                DateTimeOpt = DateTime.UtcNow;
                DateTimeRO = DateTime.UtcNow;
                DateReq = DateTime.UtcNow;
                DateOpt = DateTime.UtcNow;
                DateRO = DateTime.UtcNow;
                TimeOfDayReq = new TimeOfDay(9, 0, 0);
                TimeOfDay = new TimeOfDay(9, 0, 0);
                TimeOfDayRO = new TimeOfDay(9, 0, 0);
                TimeSpanRO = new TimeSpan(3, 13, 25, 11, 933);
                TimeSpanHMSRO = new TimeSpan(0, 13, 25, 11, 933);
                OpeningHours = new WeeklyHours();
            }
        }

        public async Task<ActionInfo> RenderModuleAsync() {
            Model model = new Model {
                OpeningHours = WeeklyHours.WorkWeek,
            };
            return await RenderAsync(model);
        }

        [ConditionalAntiForgeryToken]
        public async Task<IResult> UpdateModuleAsync(Model model) {
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            return await FormProcessedAsync(model, this.__ResStr("ok", "OK"));
        }
    }
}
