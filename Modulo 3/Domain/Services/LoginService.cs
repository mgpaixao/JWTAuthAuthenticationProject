using Domain.Interfaces;
using Domain.Services.Interfaces;
using SecureIdentity.Password;
using JWTAuthAuthentication3.Services;

namespace Domain.Services
{
    public class LoginService :  ILoginService
    {
        private readonly ILoginRepository _loginRepository;
        private readonly TokenService _tokenService;

        public LoginService(ILoginRepository loginRepository, TokenService tokenService)
        {
            _tokenService = tokenService;
            _loginRepository = loginRepository;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _loginRepository.GetUser(email);

            if (user == null)
                return "User or password invalid";

            if (!PasswordHasher.Verify(user.PasswordHash, password))
                return "User or password invalid";

            try
            {
                var token = _tokenService.GenerateToken(user);
                return token;
            }
            catch
            {
                return "Internal Error";
            }
        }

        public void Dispose()
        {
            _loginRepository?.Dispose();
        }
    }
}
