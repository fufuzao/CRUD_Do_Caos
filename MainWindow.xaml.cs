using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CRUD_Do_Caos;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<Cliente> clientes = new();
    private int idSelecionado = 0;

    public MainWindow()
    {
        InitializeComponent();
        dgClientes.ItemsSource = clientes;
        CarregarClientes();
        txtStatus.Text = "Sistema carregado com sucesso.";
    }

    private void CarregarClientes(string pesquisa = "")
    {
        try
        {
            using var conexao = Banco.NovaConexao();
            conexao.Open();
            var sql = "SELECT id, nome, email, idade, cidade, ativo, criado_em FROM clientes";
            if (!string.IsNullOrWhiteSpace(pesquisa))
                sql += " WHERE nome LIKE @pesquisa OR email LIKE @pesquisa OR cidade LIKE @pesquisa";
            sql += " ORDER BY id DESC";
            using var cmd = new MySqlCommand(sql, conexao);
            if (!string.IsNullOrWhiteSpace(pesquisa)) cmd.Parameters.AddWithValue("@pesquisa", pesquisa + "%");
            using var reader = cmd.ExecuteReader();
            clientes.Clear();
            while (reader.Read())
            {
                clientes.Add(new Cliente
                {
                    Id = reader.GetInt32("id"),
                    Nome = reader.GetString("nome"),
                    Email = reader.GetString("email"),
                    Idade = reader.GetInt32("idade"),
                    Cidade = reader.GetString("cidade"),
                    Ativo = reader.GetBoolean("ativo"),
                    CriadoEm = reader.GetDateTime("criado_em")
                });
            }
            txtStatus.Text = $"{clientes.Count} cliente(s) carregado(s).";
        }
        catch (Exception ex)
        {
            MessageBox.Show("Não foi possível carregar os clientes.\n" + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool ValidarFormulario()
    {
        if (string.IsNullOrWhiteSpace(txtNome.Text) && string.IsNullOrWhiteSpace(txtEmail.Text))
        {
            MessageBox.Show("Nome e e-mail são obrigatórios.");
            return false;
        }
        if (!int.TryParse(txtIdade.Text, out int idade))
        {
            MessageBox.Show("Informe uma idade numérica.");
            return false;
        }
        if (idade <= 1 || idade > 90)  //////////////////////////////////////////////////////////////////////////////
        {
            MessageBox.Show("Idade inválida.");
            return false;
        }
        if (!txtEmail.Text.Contains("@") || !txtEmail.Text.Contains(".")) ///////////////////////////////////////////////////////////////////
        {
            MessageBox.Show("E-mail inválido.");
            return false;
        }
        if (string.IsNullOrWhiteSpace(txtCidade.Text||))////////////////////==========================================
        {
            MessageBox.Show("Informe a cidade.");
            return false;/////////////////////////////////////////////////===========================================
        }
        return true;
    }

    private void btnSalvar_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidarFormulario()) return;
        MessageBox.Show("Cliente cadastrado com sucesso!", "Cadastro");
        try
        {

            using var conexao = Banco.NovaConexao();
            conexao.Open();
            const string sql = "INSERT INTO clientes (nome, email, idade, cidade, ativo) VALUES (@nome, @email, @idade, @cidade, @ativo)";
            using var cmd = new MySqlCommand(sql, conexao);
            cmd.Parameters.AddWithValue("@nome", txtNome.Text.Trim());/////////////////////////////////////////////////////////////////////////////////////
            cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());///////////////////////////////////////////////////////////////////////////////////
            cmd.Parameters.AddWithValue("@idade", Convert.ToInt32(txtIdade.Text));
            cmd.Parameters.AddWithValue("@cidade", txtCidade.Text.Trim());
            cmd.Parameters.AddWithValue("@ativo", chkAtivo.IsChecked == true);//------------------------------------------------------------
            cmd.ExecuteNonQuery();
            LimparCampos(false);
            txtStatus.Text = "Cadastro concluído.";
            CarregarClientes(txtPesquisa.Text.Trim()); /////////////////////////////////////////////////////////////////////
        }
        catch (Exception ex)
        {
            MessageBox.Show("Falha ao cadastrar: " + ex.Message);
        }
    }

    private void btnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (idSelecionado == 0)
        {
            MessageBox.Show("Selecione um cliente para editar.");
            return;
        }
        if (!ValidarFormulario()) return;
        try
        {
            using var conexao = Banco.NovaConexao();
            conexao.Open();
            const string sql = "UPDATE clientes SET nome=@nome, email=@email, idade=@idade, cidade=@cidade, ativo=@ativo WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conexao);
            cmd.Parameters.AddWithValue("@nome", txtNome.Text.Trim());
            cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
            cmd.Parameters.AddWithValue("@idade", int.Parse(txtIdade.Text));
            cmd.Parameters.AddWithValue("@cidade", txtCidade.Text.Trim());
            cmd.Parameters.AddWithValue("@ativo", chkAtivo.IsChecked == true);
            cmd.Parameters.AddWithValue("@id", idSelecionado);///////////////////////////////////////////////////////////////////
            var linhas = cmd.ExecuteNonQuery();
            MessageBox.Show("Cliente atualizado com sucesso!");
            CarregarClientes();
            txtStatus.Text = $"Atualização concluída. {linhas} linha(s) afetada(s).";
            CarregarClientes(txtPesquisa.Text.Trim());//////////////////////////////////////////////////////////////////////
        }
        catch (Exception ex)
        {
            MessageBox.Show("Falha ao editar: " + ex.Message);
        }
    }

    private void btnExcluir_Click(object sender, RoutedEventArgs e)
    {
        if (idSelecionado == 0) { MessageBox.Show("Selecione um cliente."); return; }
        var resposta = MessageBox.Show("Deseja realmente excluir este cliente?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (resposta == MessageBoxResult.Yes)
        {
            try
            {
                using var conexao = Banco.NovaConexao();
                conexao.Open();
                using var cmd = new MySqlCommand("DELETE FROM clientes WHERE id=@id", conexao);
                cmd.Parameters.AddWithValue("@id", idSelecionado);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Cliente excluído com sucesso!");
                txtStatus.Text = "Exclusão concluída.";
                CarregarClientes(txtPesquisa.Text.Trim());////////////////////////////////////////////////////////////
            }
            catch (Exception ex) { MessageBox.Show("Falha ao excluir: " + ex.Message); }
        }
    }

    private void dgClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (dgClientes.SelectedItem is not Cliente c) return;
        idSelecionado = c.Id;
        txtNome.Text = c.Nome;
        txtEmail.Text = c.Email;
        txtIdade.Text = c.Idade.ToString();
        txtCidade.Text = c.Cidade;
        chkAtivo.IsChecked = !c.Ativo;
        txtStatus.Text = $"Cliente #{c.Id} selecionado.";
        CarregarClientes(txtPesquisa.Text.Trim());/////////////////////////////////////////////////////////////////////////////////////
    }

    private void btnPesquisar_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtPesquisa.Text))
        {
            txtStatus.Text = "Nenhum termo informado.";
            CarregarClientes(txtPesquisa.Text.Trim());///////////////////////////////////////////////////////////////////////////////////
            return;
        }
       
    }

    private void btnNovo_Click(object sender, RoutedEventArgs e)
    {
        idSelecionado = 0; ///////////////////////////////////////////////////////////////////////////////////////////////////
        LimparCampos(false);
        txtNome.Focus();
        txtStatus.Text = "Novo cadastro.";
        CarregarClientes(txtPesquisa.Text.Trim());//////////////////////////////////////////////////////////////////////////////////
    }

    private void btnLimpar_Click(object sender, RoutedEventArgs e)
    {
        LimparCampos(false);
        txtStatus.Text = "Campos limpos.";
        CarregarClientes(txtPesquisa.Text.Trim());///////////////////////////////////////////////////////////////////////////////////
    }

    private void LimparCampos(bool limparId)
    {
        txtNome.Clear();
        txtEmail.Clear();
        txtIdade.Clear();
        txtCidade.Clear();
        chkAtivo.IsChecked = true;
        if (limparId) idSelecionado = 0;
    }
}
