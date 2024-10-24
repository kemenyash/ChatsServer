using Ganss.Xss;
using Newtonsoft.Json;
using System.Web;

public class EscapingStringConverter : JsonConverter<string>
{
    private static readonly HtmlSanitizer sanitizer = new HtmlSanitizer();

    public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
    {
        var sanitizedValue = sanitizer.Sanitize(value);
        sanitizedValue = HttpUtility.HtmlEncode(sanitizedValue);
        writer.WriteValue(sanitizedValue);
    }

    public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
    {

        var value = (string)reader.Value;
        return sanitizer.Sanitize(value);
    }
}
