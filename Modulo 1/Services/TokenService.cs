using JWTAuthAuthentication.Extentions;
using JWTAuthAuthentication.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuthAuthentication.Services
{
    //Gerador de Token
    public class TokenService
    {
        public string GenerateToken (User user)
        {
            //Estancia do manipulador de Token
            var tokenHandler = new JwtSecurityTokenHandler();
            //Chave da classe Configuration. O Token Handler espera um Array de Bytes, por isso é necessário converter
            var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
            //Convertendo JWTKey em byte
            var claims = user.GetClaims();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), //Claims que vão compor o token
                Expires = DateTime.UtcNow.AddHours(8), //Por quanto tempo vai valer o token?
                SigningCredentials = //Assinatura do token, serve para identificar que mandou o token e garantir que o token não foi alterado no meio do caminho.
                new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            //Gerando o token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //Retornando tudo como uma string
            return tokenHandler.WriteToken(token);
        }
    }
}
