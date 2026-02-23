namespace ProjetoLAWBD.Services {
    public class ReCAPTCHAService : IReCAPTCHAService {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public ReCAPTCHAService(IHttpClientFactory httpClientFactory, IConfiguration config) {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<bool> IsValid(string token) {
            if (string.IsNullOrEmpty(token)) {
                return false;
            }

            try {
                var secretKey = _config["reCAPTCHA:SecretKey"];
                var client = _httpClientFactory.CreateClient();

                // Faz o pedido POST à Google para verificar o token
                var response = await client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}", null);

                if (response.IsSuccessStatusCode) {
                    var json = await response.Content.ReadFromJsonAsync<reCAPTCHAResponse>();
                    return json.Success;
                }
            } catch (Exception) {
                // Se falhar (ex: rede), não valida
                return false;
            }

            return false;
        }
    }
}
