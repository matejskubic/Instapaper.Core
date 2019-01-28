using System;
using Newtonsoft.Json;

namespace Instapaper.Core.Converters
{
    public class BoolConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((bool)value) ? 1 : 0);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value != null && (reader.Value.ToString() == "1");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }
    }

    public class EpochDateTimeConverter : JsonConverter
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var date = (DateTime?)value;
            var epoch = date.HasValue ? (date.Value - Epoch).TotalSeconds : 0;
            writer.WriteValue(epoch);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var timestamp = float.Parse(reader.Value.ToString());
            if (timestamp == 0) return null;
            DateTime? date = Epoch.AddSeconds(timestamp);
            return date;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime?);
        }
    }
}
