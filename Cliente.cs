namespace CRUD_Do_Caos;
public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public int Idade { get; set; }
    public string Cidade { get; set; } = "";
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
}
