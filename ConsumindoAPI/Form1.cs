using System.Configuration;

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



    private void button1_Click(object sender, EventArgs e)
    {
        _urlBase = ConfigurationManager.AppSettings["UrlBase"];
        var email = ConfigurationManager.AppSettings["UserID"];
        var password = ConfigurationManager.AppSettings["AccessKey"];
        var confirmPassword = password;

        var urlbase = _urlBase + "autoriza/login";
    }
}