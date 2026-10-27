using System.ComponentModel.DataAnnotations;

namespace SinX.Web.ViewModels.Setup;

public sealed class SetupInputModel
{
    [Display(Name = "UserName")]
    [Required(ErrorMessage = "ValidationRequired")]
    public string? UserName { get; init; }

    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [Required(ErrorMessage = "ValidationRequired")]
    public string? Password { get; init; }

    [DataType(DataType.Password)]
    [Display(Name = "ConfirmPassword")]
    [Compare(nameof(Password), ErrorMessage = "ValidationPasswordMismatch")]
    [Required(ErrorMessage = "ValidationRequired")]
    public string? ConfirmPassword { get; init; }
}
