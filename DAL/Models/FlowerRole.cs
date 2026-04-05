using System.Text.Json.Serialization;

namespace DAL.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FlowerRole
    {
        Focal,
        Semi,
        Filler,
        Greenery
    }
}
