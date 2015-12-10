/*
* Licensed under the Apache License, Version 2.0 (the "License")
* http://www.apache.org/licenses/LICENSE-2.0
*
* Copyright © Microsoft Open Technologies, Inc.
* All Rights Reserved
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Windows.Networking.PushNotifications;
using Windows.Foundation;
using System.Diagnostics;
using Microsoft.WindowsAzure.Messaging;
using System.Runtime.Serialization;

namespace NotificationHubRuntimeProxy
{
    public sealed class HubApi
    {
        public IAsyncOperation<RegisterResult> RegisterNativeAsync(string notificationHubPath, string connectionString, string channelUri, string tags)
        {
            return this.RegisterNativeAsyncInternal(notificationHubPath, connectionString, channelUri, tags).AsAsyncOperation();
        }

        public async void UnregisterNativeAsync(string notificationHubPath, string connectionString)
        {
            var hub = new Microsoft.WindowsAzure.Messaging.NotificationHub(notificationHubPath, connectionString);

            await hub.UnregisterNativeAsync();
        }

        private async Task<RegisterResult> RegisterNativeAsyncInternal(string notificationHubPath, string connectionString, string channelUri, string tags)
        {
            // Create the notification hub
            var hub = new Microsoft.WindowsAzure.Messaging.NotificationHub(notificationHubPath, connectionString);

            List<string> tagCollection = new List<string>();
            if (tags.Contains(","))
            {
                foreach (string tag in tags.Split(','))
                {
                    tagCollection.Add(tag);
                }
            }
            else
            {
                tagCollection.Add(tags);
            }

            // Register with the Notification Hub, passing the push channel uri and the string array of tags
            var registration = await hub.RegisterNativeAsync(channelUri, tagCollection);

            var regInfo = new RegisterResult();
            regInfo.RegistrationId = registration.RegistrationId;
            regInfo.ChannelUri = registration.ChannelUri;
            regInfo.NotificationHubPath = registration.NotificationHubPath;
            regInfo.Tags = string.Join(",", registration.Tags);

            return regInfo;
        }
    }

    [DataContract]
    public sealed class RegisterResult
    {
        [DataMember(Name = "registrationId", IsRequired = true)]
        public string RegistrationId { get; set; }

        [DataMember(Name = "channelUri", IsRequired = true)]
        public string ChannelUri { get; set; }

        [DataMember(Name = "notificationHubPath", IsRequired = true)]
        public string NotificationHubPath { get; set; }

        [DataMember(Name = "tags", IsRequired = true)]
        public string Tags { get; set; }

    }
}
