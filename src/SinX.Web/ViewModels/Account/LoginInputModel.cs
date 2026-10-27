using System.ComponentModel.DataAnnotations;

namespace SinX.Web.ViewModels.Account;

public sealed class LoginInputModel
{
    [Display(Name = "UserName")]
    [Required(ErrorMessage = "ValidationRequired")]
    public string? UserName { get; init; }

    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [Required(ErrorMessage = "ValidationRequired")]
    public string? Password { get; init; }

    public string? ReturnUrl { get; init; }
}
