using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Wikiled.DashButton.Config
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ButtonActionType
    {
        Simple
    }
}
