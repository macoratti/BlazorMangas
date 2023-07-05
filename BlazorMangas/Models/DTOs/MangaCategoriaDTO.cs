namespace BlazorMangas.Models.DTOs;

public class MangaCategoriaDTO
{
    public int Id { get; set; }
    public string? Titulo { get; set; }
    public string? Descricao { get; set; }
    public string? Autor { get; set; }
    public string? Editora { get; set; }
    public int Paginas { get; set; }
    public DateTime Publicacao { get; set; }
    public string? Formato { get; set; }
    public string? Cor { get; set; }
    public string? Origem { get; set; }
    public string? Imagem { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public string? CategoriaNome { get; set; }
}
