﻿using System;
using System.Diagnostics;
using Instapaper.Core.Converters;
using Newtonsoft.Json;

namespace Instapaper.Core.Models
{
    [DebuggerDisplay("Id: {Id}, Title: {Title}")]
    public class Bookmark
    {
        [JsonProperty("bookmark_id")]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string Hash { get; set; }

        [JsonProperty("private_source")]
        public string PrivateSource { get; set; }

        [JsonProperty("progress_timestamp")]
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime? ProgressTimestamp { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime? Time { get; set; }
        public double Progress { get; set; }
        
        // TODO: bool converter
        public string Starred { get; set; }
    }
}
