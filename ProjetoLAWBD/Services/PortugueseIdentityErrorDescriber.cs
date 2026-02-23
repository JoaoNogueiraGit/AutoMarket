using Microsoft.AspNetCore.Identity;

namespace ProjetoLAWBD.Services {
    public class PortugueseIdentityErrorDescriber : IdentityErrorDescriber {

        public override IdentityError DuplicateEmail(string email) {
            return new IdentityError {
                Code = nameof(DuplicateEmail),
                Description = $"O email '{email}' já está a ser utilizado. Por favor, tente outro."
            };
        }

        public override IdentityError DuplicateUserName(string userName) {
            return new IdentityError {
                Code = nameof(DuplicateUserName),
                Description = $"O nome de utilizador '{userName}' já está a ser utilizado. Por favor, tente outro."
            };
        }

        // --- ERROS DE PASSWORD ---

        public override IdentityError PasswordTooShort(int length) {
            return new IdentityError {
                Code = nameof(PasswordTooShort),
                Description = $"A palavra-passe deve ter pelo menos {length} caracteres."
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric() {
            return new IdentityError {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "A palavra-passe deve conter pelo menos um caracter especial (ex: !, @, #, $)."
            };
        }

        public override IdentityError PasswordRequiresDigit() {
            return new IdentityError {
                Code = nameof(PasswordRequiresDigit),
                Description = "A palavra-passe deve conter pelo menos um número ('0'-'9')."
            };
        }

        public override IdentityError PasswordRequiresLower() {
            return new IdentityError {
                Code = nameof(PasswordRequiresLower),
                Description = "A palavra-passe deve conter pelo menos uma letra minúscula ('a'-'z')."
            };
        }

        public override IdentityError PasswordRequiresUpper() {
            return new IdentityError {
                Code = nameof(PasswordRequiresUpper),
                Description = "A palavra-passe deve conter pelo menos uma letra maiúscula ('A'-'Z')."
            };
        }

        // ERROS DE LOGIN

        public override IdentityError InvalidEmail(string email) {
            return new IdentityError {
                Code = nameof(InvalidEmail),
                Description = $"O email '{email}' é inválido."
            };
        }

        public override IdentityError PasswordMismatch() {
            return new IdentityError {
                Code = nameof(PasswordMismatch),
                Description = "Palavra-passe incorreta." // (Mensagem genérica para segurança)
            };
        }

        public override IdentityError InvalidUserName(string userName) {
            return new IdentityError {
                Code = nameof(InvalidUserName),
                Description = $"O nome de utilizador '{userName}' é inválido."
            };
        }

        public override IdentityError DefaultError() {
            return new IdentityError {
                Code = nameof(DefaultError),
                Description = "Ocorreu um erro desconhecido."
            };
        }
    }
}
