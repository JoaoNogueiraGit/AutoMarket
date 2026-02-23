using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;

namespace ProjetoLAWBD.Attributes {
    public class LocalizacaoValidaAttribute : ValidationAttribute {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
            // Ignora nulos (o [Required] trata disso)
            if (value == null || string.IsNullOrWhiteSpace(value.ToString())) {
                return ValidationResult.Success;
            }

            string input = value.ToString()!.Trim();

            // 1. Obter o Caminho
            var env = (IWebHostEnvironment?)validationContext.GetService(typeof(IWebHostEnvironment));
            if (env == null) return new ValidationResult("Erro Interno: IWebHostEnvironment nulo.");

            string path = Path.Combine(env.WebRootPath, "dados", "locais.json");

            // 2. Verificar Ficheiro
            if (!File.Exists(path)) {
                // DEBUG: Ajuda a ver onde o servidor está a procurar
                System.Diagnostics.Debug.WriteLine($"[LocalizacaoValida] Ficheiro não encontrado em: {path}");
                return new ValidationResult("Erro Interno: Lista de locais não encontrada no servidor.");
            }

            try {
                // 3. Ler JSON
                string json = File.ReadAllText(path);
                var lista = JsonSerializer.Deserialize<List<LocalItem>>(json);

                if (lista == null) return new ValidationResult("Erro Interno: JSON vazio ou inválido.");

                // 4. Validar (Case Insensitive)
                // Aceita apenas Municípios ou Distritos
                bool existe = lista.Any(x =>
                    (x.Type == "municipio" || x.Type == "distrito") &&
                    string.Equals(x.Name, input, StringComparison.OrdinalIgnoreCase));

                if (existe) {
                    return ValidationResult.Success;
                }
                else {
                    return new ValidationResult($"'{input}' não é uma localidade válida. Selecione da lista.");
                }
            } catch (Exception ex) {
                return new ValidationResult($"Erro ao processar localidade: {ex.Message}");
            }
        }

        // Classe interna para mapear o JSON
        private class LocalItem {
            [JsonPropertyName("name")]
            public string Name { get; set; } = "";

            [JsonPropertyName("type")]
            public string Type { get; set; } = "";
        }
    }
}