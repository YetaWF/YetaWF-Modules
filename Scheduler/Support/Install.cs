/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.Scheduler.DataProvider;

namespace YetaWF.Modules.Scheduler.Support {
    public partial class Scheduler {

        // INSTALL/UNINSTALL
        // INSTALL/UNINSTALL
        // INSTALL/UNINSTALL

        /// <summary>
        /// Install all events for the given package. This is typically used to install scheduler items while installing packages.
        /// </summary>
        public async Task InstallItemsAsync(Package package) {
            List<Type> types = package.GetClassesInPackage<IScheduling>();
            foreach (var type in types)
                await InstallItemsAsync(type);
        }

        /// <summary>
        /// Install all events for the given object type. This is typically used to install scheduler items while installing packages.
        /// </summary>
        private async Task InstallItemsAsync(Type type) {
            string eventType = type.FullName + ", " + type.Assembly.GetName().Name;
            using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
                IScheduling schedEvt;
                try {
                    schedEvt = (IScheduling)Activator.CreateInstance(type);
                } catch (Exception exc) {
                    throw new InternalError("The specified object does not support the required IScheduling interface.", exc);
                }
                try {
                    SchedulerItemBase[] items = schedEvt.GetItems();
                    foreach (var item in items) {
                        SchedulerItemData evnt = new SchedulerItemData();
                        ObjectSupport.CopyData(item, evnt);
                        evnt.Event.Name = item.EventName;
                        evnt.Event.ImplementingAssembly = type.Assembly.GetName().Name;
                        evnt.Event.ImplementingType = type.FullName;
                        await dataProvider.AddItemAsync(evnt);// we ignore whether the add fails - it's OK if it already exists
                    }
                } catch (Exception exc) {
                    throw new InternalError("InstallEvents for the specified type {0} failed.", eventType, exc);
                }
            }
        }

        /// <summary>
        /// Uninstall all events for the given package. This is typically used to uninstall scheduler items while uninstalling packages.
        /// </summary>
        public async Task UninstallItemsAsync(Package package) {
            List<Type> types = Package.GetClassesInPackages<IScheduling>();
            foreach (var type in types)
                await UninstallItemsAsync(type);
        }
        /// <summary>
        /// Uninstall all events for the given object type. This is typically used to uninstall scheduler items while removing packages.
        /// </summary>
        private async Task UninstallItemsAsync(Type type) {
            string asmName = type.Assembly.GetName().Name;
            string eventType = type.FullName + ", " + asmName;
            using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
                IScheduling schedEvt = null;
                try {
                    schedEvt = (IScheduling)Activator.CreateInstance(type);
                } catch (Exception exc) {
                    throw new InternalError("The specified object does not support the required IScheduling interface.", exc);
                }
                if (schedEvt != null) {
                    try {
                        // Event.ImplementingAssembly == asmName
                        List<DataProviderFilterInfo> filters = new List<DataProviderFilterInfo> {
                            new DataProviderFilterInfo {
                                Field = "Event.ImplementingAssembly", Operator = "==", Value = asmName
                            }
                        };
                        await dataProvider.RemoveItemsAsync(filters);// we ignore whether the remove fails
                    } catch (Exception exc) {
                        throw new InternalError("UninstallItems for the specified type {0} failed.", eventType, exc);
                    }
                }
            }
        }
    }
}