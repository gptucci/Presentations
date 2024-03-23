using System.Text.Json;

namespace MauiAppAkavache.Client;

public static class Serializations
{
    public static string SerializeObj<T>(T modelObject) => JsonSerializer.Serialize(modelObject);

    //Sembra che usare il metodo statico GetJsonSerializerOptions così sia più veloce
    //https://mathieupyle.com/an-easy-way-to-drastically-improve-jsonserializer-performance-system-text-json-use-a-static-variable-for-jsonserializeroptions/
    static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public static T DeserializeJsonString<T>(string jsonString)
    {

        return JsonSerializer.Deserialize<T>(jsonString, GetJsonSerializerOptions())!;
    }
    public static List<T> DeserializeJsonStringList<T>(string jsonString)
    {


        return JsonSerializer.Deserialize<List<T>>(jsonString, GetJsonSerializerOptions())!;
    }


}
