using System.ComponentModel.DataAnnotations;

namespace BlazorMangas.Models.DTOs;

public class CategoriaDTO
{
    public int Id { get; set; }
    [Required(ErrorMessage = "O nome é requerido")]
    public string? Nome { get; set; }
}
