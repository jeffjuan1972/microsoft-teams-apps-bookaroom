// <copyright file="PlaceInfo.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Teams.App.BookAThing.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Class describing place information.
    /// </summary>
    public class RoomInfo
    {

        /// <summary>
        /// Gets or sets name of place.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets email address associated with place.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }
    }
}
