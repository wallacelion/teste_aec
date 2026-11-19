namespace api.src.modules.auth.application.dtos
{
    public class AuthLoginDto
    {
        public string Username { get; private set; }
        public string Password { get; private set; }

        public AuthLoginDto(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
