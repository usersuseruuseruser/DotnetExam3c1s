using System.Text.Json.Serialization;

namespace api.Contracts;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Figure
{
    Rock,
    Scissors,
    Paper
}