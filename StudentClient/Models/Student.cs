using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StudentClient.Models;

public class Student
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите ФИО.")]
    [StringLength(100, ErrorMessage = "ФИО не должно превышать 100 символов.")]
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите группу.")]
    [StringLength(30, ErrorMessage = "Название группы не должно превышать 30 символов.")]
    [JsonPropertyName("group")]
    public string Group { get; set; } = string.Empty;

    [Range(1, 6, ErrorMessage = "Курс должен быть от 1 до 6.")]
    public int Course { get; set; }

    [Required(ErrorMessage = "Введите Email.")]
    [EmailAddress(ErrorMessage = "Введите корректный Email.")]
    public string Email { get; set; } = string.Empty;
}
