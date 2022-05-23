using Newtonsoft.Json;
using System;
using TestWebApp.Domain.PersonManagement;

namespace TestWebApp.Converters
{
    public class NullableRelationTypeJsonConverter : JsonConverter<RelationType?>
    {
        public override RelationType? ReadJson(JsonReader reader, Type objectType, RelationType? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }

            var value = reader.Value;
            if (int.TryParse(value.ToString(), out int result))
            {
                return (RelationType?)result;
            }

            if (Enum.TryParse(value.ToString(), true, out RelationType relationType))
            {
                return relationType;
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, RelationType? value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}