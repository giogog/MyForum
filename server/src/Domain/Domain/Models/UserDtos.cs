namespace Domain.Models;
public record UserDto
{
    public int Id { get; init; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}
public record AuthorizedUserDto(int Id,string Name, string Surname, string Username, string Email,Ban Banned, string[] Roles);





