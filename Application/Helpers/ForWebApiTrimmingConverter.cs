using Newtonsoft.Json;
using System;
namespace Application.Helper
{
    public class ForWebApiTrimmingConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
         JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String && reader.Value != null)
            {
                var toReturn = (reader.Value as string).Trim();

                if (toReturn.Contains("\n"))
                {
                    toReturn = toReturn.Replace("\n", " ");
                }

                return toReturn;
            }
            return reader.Value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer
         serializer)
        {
            string str = (string)value;

            if (str == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(str.Trim());
            }

        }
    }
}
