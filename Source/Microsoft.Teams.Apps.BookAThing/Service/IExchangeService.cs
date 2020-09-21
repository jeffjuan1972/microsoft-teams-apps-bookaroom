// <copyright file="IExchangeSyncHelper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.BookAThing.Service
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.BookAThing.Common.Models.TableEntities;

    /// <summary>
    /// Methods for performing exchange to table storage sync operation.
    /// </summary>
    public interface IExchangeService
    {
        /// <summary>
        /// Process exchange to storage sync.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        Task<List<MeetingRoomEntity>> FindAllRooms(string token);
        Task<List<MeetingRoomEntity>> FindRooms(string token, string address);

        Task<List<MeetingRoomEntity>> FilterRooms(string token, string query);
    }
}