/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Support;
using YetaWF.Modules.Caching.Startup;

namespace YetaWF.Modules.Caching.DataProvider {

    /// <summary>
    /// Redis pub/sub provider.
    /// </summary>
    /// <remarks>
    /// Uses a Redis server for pub/sub messages.
    /// </remarks>
    public class PubSubRedisProvider : IPubSubProvider {

        private static ConnectionMultiplexer Redis { get; set; }
        private static string KeyPrefix { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() { Dispose(true); }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to release the DisposableTracker reference count, false otherwise.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                DisposableTracker.RemoveObject(this);
            }
        }

        // Implementation

        /// <summary>
        /// Constructor.
        /// </summary>
        public PubSubRedisProvider() { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configString">The Redis configuration string used to connect to the Redis server.</param>
        /// <param name="keyPrefix">The string used to prefix all channel names used.</param>
        public PubSubRedisProvider(string configString, string keyPrefix) {
            Redis = ConnectionMultiplexer.Connect(configString);
            KeyPrefix = keyPrefix;
            DisposableTracker.AddObject(this);
        }

        // API

        public async Task SubscribeAsync(string channel, Action<string, object> callback) {
            ISubscriber subscriber = Redis.GetSubscriber();
            if (YetaWFManager.IsSync()) {
                subscriber.Subscribe($"{KeyPrefix}{channel}", (ch, msg) => {
                    callback(channel, Utility.JsonDeserialize(msg.ToString()));
                });
            } else {
                await subscriber.SubscribeAsync($"{KeyPrefix}{channel}", (ch, msg) => {
                    callback(channel, Utility.JsonDeserialize(msg.ToString()));
                });
            }
        }

        public async Task UnsubscribeAsync(string channel) {
            ISubscriber subscriber = Redis.GetSubscriber();
            if (YetaWFManager.IsSync()) {
                subscriber.Unsubscribe($"{KeyPrefix}{channel}");
            } else {
                await subscriber.UnsubscribeAsync($"{KeyPrefix}{channel}");
            }
        }

        public async Task PublishAsync(string channel, object message) {
            ISubscriber subscriber = Redis.GetSubscriber();
            if (YetaWFManager.IsSync()) {
                subscriber.Publish($"{KeyPrefix}{channel}", Utility.JsonSerialize(message));
            } else {
                await subscriber.PublishAsync($"{KeyPrefix}{channel}", Utility.JsonSerialize(message));
            }
        }
    }
}
