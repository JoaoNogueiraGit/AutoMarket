using System.Text.Json.Serialization;

namespace ProjetoLAWBD.Services {
    public class reCAPTCHAResponse {

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error-codes")]
        public string[] ErrorCodes { get; set; }
    }
}
