namespace ProjetoLAWBD.Services {
    public interface IReCAPTCHAService {

        Task<bool> IsValid(string token);
    }
}
