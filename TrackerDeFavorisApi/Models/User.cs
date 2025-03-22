using System.Text.Json.Serialization;

namespace TrackerDeFavorisApi.Models
{
    public class User 
    {
        public int Id { get; set; } 
        public required string Pseudo { get; set; } = string.Empty;
        public required string MotDePasse { get; set; } = string.Empty;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required Role Role { get; set; }
    }

    public enum Role
    {
        User,
        Admin
    }

    public class UserInfo 
    {
        public required string Pseudo { get; set; }
        public required string MotDePasse { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required Role Role { get; set;}
    }

    public class PublicUsers 
    {
        public int Id { get; set; } 
        public required string Pseudo { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required Role Role { get; set; }
    }
}