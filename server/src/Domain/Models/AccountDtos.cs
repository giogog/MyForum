namespace Domain.Models;

public record LoginDto(string Username, string Password);


public record RegisterDto(string Name, string Surname, string Username, string Email, string Password);
public record LoginResponseDto(int Id, string Username, string Token);

public record ResetPasswordDto(string Email, string Token, string NewPassword);

