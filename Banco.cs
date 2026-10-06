using MySql.Data.MySqlClient;
namespace CRUD_Do_Caos;
public static class Banco
{
    public const string Conexao = "Server=localhost;Port=3307;Database=crud_caos;Uid=root;Pwd=;";
    public static MySqlConnection NovaConexao() => new(Conexao);
}
