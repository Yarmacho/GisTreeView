﻿using System;
using System.Text.Json.Serialization;

namespace Entities.Entities
{
    [Serializable]
    public class RoutePoint : EntityBase<int>
    {
        [JsonPropertyName("routeId")]
        public int RouteId { get; set; }

        [JsonPropertyName("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        public double Y { get; set; }
    }
}
