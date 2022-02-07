Olá Devs, este artigo vai mostrar de forma simples como criar um API de autenticação e autorização de API's através de JWT Token e definindo Roles(cargos) pra cada usuário do sistema. 

# Sumário - Clique e ache rápido o que deseja ler
- [Regra de Negócio e Contextualização](#regra-de-negócio)
- [Banco de dados Postgres](#banco-de-dados-postgres)
- [Criando API Básica com JWT - Models](#criando-uma-api-básica-usando-o-processo-de-autenticação-com-jwt-token)
- [Configuração do JWT](#o-segredo-por-trás-de-tudo)
- [Criptografia da senha de usuário](#criptografia-da-senha-do-usuário)
- [Populando o Banco de Dados](#populando-o-banco-de-dados)
- [Teste com Swagger](#teste-com-swagger)

Regra de Negócio 
# `Módulo 1`

Nesse primeiro módulo, iremos focar na aplicação pura do JWT Token com Roles. Nos próximos módulos teremos os CRUDs completos e complementos de funcionalidades.

- O usuário Admin é cadastrado na mão, no banco de dados e ele é o único que tem o poder de mudar as Roles dos usuários.

- As roles são cadastradas direto no banco de dados também.

- O sistema cadastra um usuário.

- O sistema faz Login do usuário.

## `Spoiler`

Próximo módulo adicionaremos mais 2 regras de negócio.

- Sempre deve ter um usuário Admin no sistema

-  O usuário Admin não pode mudar sua própria Role. 


![Gif](https://media.giphy.com/media/11fot0YzpQMA0g/giphy-downsized-large.gif)

# Importância do Controle de Acesso

Imagina que você tem um sistema que vários usuários com cargos diferentes e você quer que cada cargo tenha um tipo de acesso diferente. Exemplo: O Assistente Financeiro terá acesso a relatórios da área de finanças, mas, apenas o Diretor financeiro terá acesso a liberar verbas, aumentar e diminuir budgets (despesas) de cada departamento. 

Para que você consiga fazer esse controle por tipo de usuário, uma solução pode ser definir `Roles`.

# Roles

![Gif](https://media.giphy.com/media/3o6EhRA9T9NF6mIhjy/giphy.gif)
No nosso caso, no Modulo 1, teremos 2 tipos de usuários: `Admin` e o `User`.

- `Admin` - Acesso a todos os endpoints
- `User` - Acesso ao endpoint de registro de usuários e o de login de usuários

# Qual a diferença entre Autenticação e Autorização?

![Gif](https://media.giphy.com/media/81xwEHX23zhvy/giphy.gif)

- `Autenticação` é o processo de ver quem você é, qual seu nome, senha, email, você realmente é quem você diz ser? Isso é AUTENTICAR

- `Autorização` é o processo de verificar aonde você tem permissão de acessar. Quais endpoints você terá acesso. Isso é AUTORIZAR

Exemplo: Você chega numa balada e logo na entrada precisa ser `Autenticado`, eles verificaram seu nome, seu cartão de crédito e vão verificar pelo seu nome aonde você tem `Autorização` de entrar se você vai apenas na pista é uma pulseira, se pode ir na pista e no camarote, já é outra pulseira. No programação essa pulseira de identificação seria o seu `Token`, contendo suas informações básicas e suas `Claims`, que seriam as `autorizações` que você tem, compactadas naquela pulseira (token).

### Criando uma API básica usando o processo de autenticação COM JWT token

![Gif](https://media.giphy.com/media/SV09Wp6hvMW7m/giphy.gif)

# Banco de Dados Postgres

## `DataContext`

```
public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        private IConfiguration configuration;

        public DataContext(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseNpgsql(configuration["ConnectionString:Postgres"].ToString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new RolesMap());
        }
    }
```
## `ConnectionString - secrets.json`

```
{
  "ConnectionString:Postgres": "Host=localhost;Database=Teste;Username=seu username;Password=sua senha"
}
```
# Token de Assinatura
## `Configuration.cs`
A chave de assinatura ou `JWTKey` é no nosso caso um `GUID` que colocamos dentro da 
```
public static class Configuration
    {
        //Token
        public static string JwtKey { get; set; } = "12345678-1234-1234-1234-123456789123";
    }
```

# O segredo por trás de tudo
# `TokenService.cs`
Bibliotecas usadas

```
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
```
```
public string GenerateToken (User user)
        {
            //Estancia do manipulador de Token
            var tokenHandler = new JwtSecurityTokenHandler();
            //Chave da classe Configuration. O Token Handler espera um Array de Bytes, por isso é necessário converter
            var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
            //
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
```
`tokenHandle` - Chave da classe Configuration. O Token Handler espera um Array de Bytes, por isso é necessário converter
`key` - JWTKey em formato byte
`claims` - Resultado do método GetClaims() que gera as Claims que vão copor o token
`tokenDescriptor = new SecurityTokenDescriptor{}` - Montando o token com todos seus parâmetros.

Há mais comentários detalhando cada linha do código no próprio código.

# `GetClaims()`

Método que adiciona dentro de uma lista todas as informações que você quer export do usuário no Jwt token através da Claim.

```
public static class RoleClaimExtention
    {
        public static IEnumerable<Claim> GetClaims(this User user)
        {
            var result = new List<Claim>
            {
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Roles.Name)
            };
            return result;
        }
    }
```

## Endpoints -

1. **Post** `Register` - **Rota**  - `api/auth/v1/register`
Registra uma nova conta de usuário no banco de dados
2. **Post** `Login` - **Rota**- `api/auth/v1/login`
Autoriza o usuário através da validação das credenciais, e gera um JWT Token
4. **Patch** `ChangeRole` - **Rota**- `api/auth/v1/changeUserRole`
Altera a Role do usuário no banco de dados


### Criptografia da senha do usuário 

Usamos a classe `PasswordHasher` da biblioteca `SecureIdentity` para criptografar a senha digitada pelo usuário e salvar no banco esse senha criptografada. `PasswordHash = PasswordHasher.Hash(model.Password)` [O que é Hash?](https://www.kaspersky.com.br/blog/hash-o-que-sao-e-como-funcionam/2773/)

Para segurança do usuário nunca salvar a senha do usuário sem criptografia no banco de dados.

Na hora de fazer a autenticação, e verificar a senha, é necessário discriptografar a senha do banco de dados e comparar com a senha que o usuário digitou.
`PasswordHasher.Verify(Senha do banco de dados, Senha digitada pelo usuário)`

# Populando o Banco de dados

Da forma que achar melhor, através de GUIs como PgAdmin ou console, popúle a tabela de `Role` como na imagem abaixo, pois ainda não criaram um endpoint para isso.

![ROLES](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/nljlzz0cn46j8m6w3ylg.png)

E na tabela `User` crie um usuário com a role de Admin, pois ele será o ínico que terá acesso ao endpoint `changeRoles`. 
![User](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/6acar5op9w9wxvayxhgm.png)
 

# Teste com Swagger

### `Register`
![Register](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/m6g4kuds5oq8j0x76zn1.png)

### `Login`

![Login](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/a6mu36y64700t5kr71f3.png)

Antes de seguirmos com a validação do token, tente alterar as roles no endpoint `changeRoles` e repare que o retorno é 401, não autorizado. 

![Logins](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/2sstvx3lag88hrfr9y8v.png) 

O retorno que temos do login é o JTW token com as claims que definimos do token service. Para se autorizar, pegue esse Token e se tudo deu certo na configuração do Swagger, você deve ver um ícone no lado direito superior da pagina do Swagger:

![Retornojtw](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/fx87yq7vevto5mei3k7x.png)

Clique nele e insira o token nesse formado `Bearer + Token`, como na imagem abaixo.

![Bearertoken](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/5pjwgopdl7s8uk4qzmr9.png)

Porém, mesmo assim você não terá acesso ao endpoint `changeRoles`, pois quando você cria um usuário, o role dele não é de admin. 

Faça o login com o usuário `Admin` que criou no banco de dados.

Insira o Bearer Token e altere a role do usuário que acabou de criar.

E veja que o resultado é 200, sucesso.

![Sucesso](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/kkhg99ztxxw0207ssuw2.png)

Para verificar o JWT token e suas claims, acesse o site [Jwt.io](https://jwt.io/) e cole o token no quadro de Encoded.

![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/39bs03x5izemommgh4rx.png)
 
Repare o payload do token tem o nome, email, a role e os timestamps de quando foi gerando e até quando dura o token.

Obrigado por ler este artigo. Abaixo colocarei os links dos próximos módulos quando forem lançados, a ideia é evolouirmos pouco a pouco essa aplicação.



 
 

 


















