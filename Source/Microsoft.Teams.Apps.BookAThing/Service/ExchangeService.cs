// <copyright file="ExchangeSyncHelper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.BookAThing.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.Identity.Client;
    using Microsoft.Teams.App.BookAThing.Common.Models.Error;
    using Microsoft.Teams.App.BookAThing.Common.Models.Response;
    using Microsoft.Teams.App.BookAThing.Models;
    using Microsoft.Teams.Apps.BookAThing.Common;
    using Microsoft.Teams.Apps.BookAThing.Common.Helpers;
    using Microsoft.Teams.Apps.BookAThing.Common.Models.TableEntities;
    using Microsoft.Teams.Apps.BookAThing.Common.Providers.Storage;
    using Newtonsoft.Json;
    using Polly;
    using Polly.Contrib.WaitAndRetry;
    using Polly.Retry;

    /// <summary>
    /// Methods for performing exchange to table storage sync operation.
    /// </summary>
    public class ExchangeService : IExchangeService
    {
        /// <summary>
        /// Graph API to get building list using application token. (Replace {id} with user Id).
        /// </summary>
        private readonly string graphAPIAppFindRoomList = "/beta/me/findRooms";
        /// <summary>
        /// Api helper service for making post and get calls to Graph.
        /// </summary>
        private readonly IGraphApiHelper apiHelper;

        /// <summary>
        /// Telemetry service to log events and errors.
        /// </summary>
        private readonly TelemetryClient telemetryClient;


        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeSyncHelper"/> class.
        /// </summary>
        /// <param name="apiHelper">Api helper service for making post and get calls to Graph.</param>
        /// <param name="telemetryClient">Telemetry service to log events and errors.</param>
        public ExchangeService(IGraphApiHelper apiHelper, TelemetryClient telemetryClient)
        {
            this.apiHelper = apiHelper;
            this.telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Process exchange to storage sync.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task<List<MeetingRoomEntity>> FindAllRooms(string token)
        {
            this.telemetryClient.TrackTrace("ExchangeSerivce:FindAllRooms started");
            if (token == null)
            {
                this.telemetryClient.TrackTrace("ExchangeSerivce:FindAllRooms - Application access token is null.");
                return null;
            }

            RoomResponse buildings = new RoomResponse();
            var httpResponseMessage = await this.apiHelper.GetAsync(this.graphAPIAppFindRoomList, token).ConfigureAwait(false);
            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                buildings = JsonConvert.DeserializeObject<RoomResponse>(content);
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(content);
                this.telemetryClient.TrackTrace($"ExchangeSerivce:FindAllRooms - Graph API failure- url: {this.graphAPIAppFindRoomList}, response-code: {errorResponse.Error.StatusCode}, response-content: {errorResponse.Error.ErrorMessage}, request-id: {errorResponse.Error.InnerError.RequestId}", SeverityLevel.Warning);
            }

            this.telemetryClient.TrackTrace($"ExchangeSerivce:FindAllRooms - Building count: {buildings?.RoomDetails?.Count}");

            var buildingsPerBatch = 10;
            if (buildings?.RoomDetails?.Count > 0)
            {
                int count = (int)Math.Ceiling((double)buildings.RoomDetails.Count / buildingsPerBatch);
                var meetingRooms = new List<MeetingRoomEntity>();
                for (int i = 0; i < count; i++)
                {
                    var buildingsBatch = buildings.RoomDetails.Skip(i * buildingsPerBatch).Take(buildingsPerBatch);
                    meetingRooms.AddRange(buildingsBatch.Select(roomDetail => new MeetingRoomEntity
                    {
                        BuildingName = roomDetail.Name,
                        Key = "",
                        BuildingEmail = roomDetail.Address,
                        RoomName = roomDetail.Name,
                        RoomEmail = roomDetail.Address,
                        IsDeleted = false,
                    }).ToList());
                }
                return meetingRooms;
            }
            else
            {
                this.telemetryClient.TrackTrace("ExchangeSerivce:FindAllRooms- Buildings count is 0");
            }

            return null;
        }

        public async Task<List<MeetingRoomEntity>> FindRooms(string token, string address)
        {
            List<MeetingRoomEntity> rooms = await FindAllRooms(token);
            return rooms.Where(room => room.RoomEmail == address).ToList();
        }


        public async Task<List<MeetingRoomEntity>> FilterRooms(string token, string query)
        {
            List<MeetingRoomEntity> rooms = await FindAllRooms(token);
            return rooms.Where(room => room.RoomName.Contains(query)).ToList();
        }
    }
}
