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