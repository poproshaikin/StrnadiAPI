using System.Text.Json;

namespace StrnadiAPI.Services;

public class JsonHelper : IDisposable
{
    private string _json;
    private JsonDocument _jsonDoc;
    private JsonElement _root;

    public Dictionary<string, string> Properties { get; private set; }

    public JsonHelper(string json)
    {
        _json = json;
        
        _jsonDoc = JsonDocument.Parse(json);
        _root = _jsonDoc.RootElement;

        Properties = _root.EnumerateObject().ToDictionary(prop => prop.Name, prop => prop.Value.ToString());
    }

    public void Dispose()
    {
    }
}