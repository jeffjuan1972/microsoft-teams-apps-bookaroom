// <copyright file="RoomInfoResponse.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Teams.App.BookAThing.Models
{
    using System.Collections.Generic;
    using Microsoft.Teams.App.BookAThing.Common.Models.Error;
    using Newtonsoft.Json;

    /// <summary>
    /// RoomInfos response class.
    /// </summary>
    public class RoomResponse
    {
        /// <summary>
        /// Gets or sets list of rooms.
        /// </summary>
        [JsonProperty("value")]
        public List<RoomInfo> RoomDetails { get; set; }

        /// <summary>
        /// Gets or sets Graph API error response.
        /// </summary>
        public ErrorResponse ErrorResponse { get; set; }
    }
}
