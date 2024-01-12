using System;
using Newtonsoft.Json;
using SummerRest.Editor.Models;
using SummerRest.Utilities.DataStructures;

namespace SummerRest.Editor.Utilities
{
    internal class EndpointContainerJsonConverter : JsonConverter<EndpointContainer>, ISingleton<EndpointContainerJsonConverter>
    {
        public override void WriteJson(JsonWriter writer, EndpointContainer value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(Endpoint.TypeName));
            serializer.Serialize(writer, value.TypeName);
            writer.WritePropertyName(nameof(Endpoint.EndpointName));
            serializer.Serialize(writer, value.EndpointName);
            writer.WritePropertyName(nameof(EndpointContainer.Requests));
            serializer.Serialize(writer, value.Requests);
            writer.WritePropertyName(nameof(EndpointContainer.Services));
            serializer.Serialize(writer, value.Services);
            writer.WriteEndObject();
        }
        public override EndpointContainer ReadJson(JsonReader reader, Type objectType, EndpointContainer existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException("No need to deserialize an Endpoint");
        }
    }
}