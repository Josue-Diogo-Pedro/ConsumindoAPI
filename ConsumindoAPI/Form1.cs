using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http.Headers;
using System.Text;

namespace ConsumindoAPI;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    string URI = "";
    int codigoProduto = 1;
    private static string _urlBase;
    private static AccessToken accessToken;

    private void Form1_Load(object sender, EventArgs e)
    {

    }

    private async void button2_Click(object sender, EventArgs e)
    {
        try
        {
            URI = textBox1.Text;
            var acessoAPI = new AcessaAPIService();
            List<Produto> produtos = await acessoAPI.GetAllProdutos(URI, accessToken);
            dataGridView1.DataSource = produtos;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro : " + ex.Message);
        }
    }

    private async void button3_Click(object sender, EventArgs e)
    {
        BindingSource bsDados = new BindingSource();
        InputBox();
        if (codigoProduto != -1)
        {
            try
            {
                URI = textBox1.Text + "/" + codigoProduto;
                var acessaAPI = new AcessaAPIService();
                var produto = await acessaAPI.GetProdutoById(URI, accessToken);
                bsDados.DataSource = produto;
                dataGridView1.DataSource = bsDados;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro : " + ex.Message);
            }
        }
    }

    private async void button4_Click(object sender, EventArgs e)
    {
        Random randNum = new Random();
        Produto prod = new Produto();
        prod.Nome = "Novo Produto " + DateTime.Now.Second.ToString();
        prod.Descricao = "Novo Produto descricao " + DateTime.Now.Second.ToString();
        prod.CategoriaId = 1;
        prod.ImagemUrl = "novaImagem" + DateTime.Now.Second.ToString() + ".jpg";
        prod.Preco = randNum.Next(100);
        URI = textBox1.Text;

        try
        {
            var acessaAPI = new AcessaAPIService();
            var resultado = await acessaAPI.AddProduto(URI, accessToken, prod);
            MessageBox.Show(resultado.ToString());

        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro : " + ex.Message);
        }
    }

    private async void button5_Click(object sender, EventArgs e)
    {
        Random randNum = new Random();
        Produto prod = new Produto();
        prod.Descricao = "Novo Produto descricao alterada " + DateTime.Now.Second.ToString();
        prod.Nome = "Novo Produto alterado" + DateTime.Now.Second.ToString();
        prod.CategoriaId = 1;
        prod.ImagemUrl = "novo alterado" + DateTime.Now.Second.ToString() + ".jpg";
        prod.Preco = randNum.Next(100); // atualizando o preço do produto
        InputBox();
        if (codigoProduto != -1)
        {
            prod.ProdutoId = codigoProduto;
            URI = textBox1.Text + "/" + prod.ProdutoId;
            try
            {
                var acessaAPI = new AcessaAPIService();
                var resultado = await acessaAPI.UpdateProduto(URI, accessToken, prod);
                MessageBox.Show(resultado.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro : " + ex.Message);
            }
        }
    }

    private async void button6_Click(object sender, EventArgs e)
    {
        URI = textBox1.Text;
        InputBox();
        if (codigoProduto != -1)
        {
            try
            {
                var acessaAPI = new AcessaAPIService();
                var resultado = await acessaAPI.DeleteProduto(URI, accessToken, codigoProduto);
                MessageBox.Show(resultado.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro : " + ex.Message);
            }
        }
    }

    private void InputBox()
    {
        /* usando a função VB.Net para exibir um prompt para o usuário informar a senha */
        string Prompt = "Informe o código do Produto.";
        string Titulo = "www.macoratti.net";
        string Resultado = Microsoft.VisualBasic.Interaction.InputBox(Prompt, Titulo, "9", 600, 350);
        /* verifica se o resultado é uma string vazia o que indica que foi cancelado. */
        if (Resultado != "")
        {
            codigoProduto = Convert.ToInt32(Resultado);
        }
        else
        {
            codigoProduto = -1;
        }
    }

    private async void button1_Click(object sender, EventArgs e)
    {
        _urlBase = ConfigurationManager.AppSettings["UrlBase"];
        var email = ConfigurationManager.AppSettings["UserID"];
        var password = ConfigurationManager.AppSettings["AccessKey"];
        var confirmPassword = password;

        var urlbase = _urlBase + "authorize/login";

        using (var client = new HttpClient())
        {
            string conteudo = "";

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new
                MediaTypeWithQualityHeaderValue("application/json"));

            //Envio da requisição afim de autenticar e obter o token de acesso
            HttpResponseMessage respToken = await client.PostAsync(urlbase, new StringContent(
                JsonConvert.SerializeObject(new
                {
                    email,
                    password,
                    confirmPassword
                }), Encoding.UTF8, "application/json"));

            try
            {
                conteudo = await respToken.Content.ReadAsStringAsync();
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                button6.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro " + ex.Message);
                throw ex;
            }

            if(respToken.StatusCode == System.Net.HttpStatusCode.OK)
            {
                accessToken = JsonConvert.DeserializeObject<AccessToken>(conteudo);

                if (accessToken.Authenticated)
                {
                    //Associar o token aos headers dos objectos do tipo HttpClient
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token);
                    MessageBox.Show("Token JWT Autenticado"); 
                }
                else
                {
                    MessageBox.Show("Falha na autenticação");
                }
            }
        }
    }
}