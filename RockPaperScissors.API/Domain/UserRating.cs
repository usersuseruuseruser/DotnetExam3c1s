using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.Domain;

public class UserRating
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }

    public int Rating { get; set; }
}