using System.Text.Json.Serialization;

namespace NomDuFront.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Pseudo { get; set; } = string.Empty;
        [JsonConverter(typeof(JsonStringEnumConverter))]
         public Role Role { get; set; }
        public string MotDePasse { get; set; } = string.Empty;
    }
}

public enum Role
    {
        User,
        Admin
    }