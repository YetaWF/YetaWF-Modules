/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Caching.DataProvider {

    /// <summary>
    /// Redis pub/sub provider.
    /// </summary>
    /// <remarks>
    /// Uses a Redis server for pub/sub messages.
    /// </remarks>
    internal class PubSubRedisProvider : IPubSubProvider {

        private static ConnectionMultiplexer Redis { get; set; } = null!;
        private static string KeyPrefix { get; set; } = null!;

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

        /// <summary>
        /// Subscribe to a channel.
        /// </summary>
        /// <param name="channel">The channel name.</param>
        /// <param name="callback">The callback invoked when a message is published to the channel.</param>
        public async Task SubscribeAsync(string channel, Func<string, object, Task> callback) {
            ISubscriber subscriber = Redis.GetSubscriber();
            if (YetaWFManager.IsSync()) {
                subscriber.Subscribe($"{KeyPrefix}{channel}", (ch, msg) => {
                    callback(channel, Utility.JsonDeserialize(msg.ToString())).Wait();
                });
            } else {
                await subscriber.SubscribeAsync($"{KeyPrefix}{channel}", async (ch, msg) => {
                    await callback(channel, Utility.JsonDeserialize(msg.ToString()));
                });
            }
        }

        /// <summary>
        /// Unsubscribe from a channel.
        /// </summary>
        /// <param name="channel">The channel name.</param>
        public async Task UnsubscribeAsync(string channel) {
            ISubscriber subscriber = Redis.GetSubscriber();
            if (YetaWFManager.IsSync()) {
                subscriber.Unsubscribe($"{KeyPrefix}{channel}");
            } else {
                await subscriber.UnsubscribeAsync($"{KeyPrefix}{channel}");
            }
        }

        /// <summary>
        /// Publish a message to a channel.
        /// </summary>
        /// <param name="channel">The channel name.</param>
        /// <param name="message">The message object.</param>
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
