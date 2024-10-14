namespace MinimalApi.Domain.ModelViews;
public record LoggedAdministrator
{
    public string Email { get; set; } = default!;

    public string Role { get; set; } = default!;
    public string Token { get; set; } = default!;
}