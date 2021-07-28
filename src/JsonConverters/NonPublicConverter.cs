using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Penca53.JsonConverters
{
    public class NonPublicConverter<T> : JsonConverter<T> where T : new()
    {
        private Dictionary<string, PropertyInfo> _propertyMap;

        public NonPublicConverter()
            => Initialize();

        public NonPublicConverter(JsonSerializerOptions options)
            => Initialize();

        private void Initialize()
        {
            _propertyMap = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

            foreach (PropertyInfo property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                _propertyMap.Add(property.Name, property);
            }
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected StartObject token");
            }

            // Trick to make struct work too
            object item = new T();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return (T)item;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("Expected PropertyName token");
                }

                string propertyName = reader.GetString();
                if (!_propertyMap.ContainsKey(propertyName))
                {
                    throw new JsonException($"Property name '{propertyName}' not found in the type");
                }

                PropertyInfo property = _propertyMap[propertyName];
                object value = JsonSerializer.Deserialize(ref reader, property.PropertyType, options);
                property.SetValue(item, value);
            }

            throw new JsonException("Expected EndObject token");
        }

        public override void Write(Utf8JsonWriter writer, T item, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (PropertyInfo property in _propertyMap.Values)
            {
                writer.WritePropertyName(property.Name);
                JsonSerializer.Serialize(writer, property.GetValue(item), property.PropertyType, options);
            }

            writer.WriteEndObject();
        }
    }
}