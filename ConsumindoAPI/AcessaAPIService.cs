using Newtonsoft.Json;
using System.ComponentModel;
using System.Net.Http.Headers;

namespace ConsumindoAPI;

public class AcessaAPIService
{
    public async Task<List<Produto>> GetAllProdutos(string URI, AccessToken accessToken)
    {
        using(var client = new HttpClient())
        {
            GetHeaderTokenAuthorization(client, accessToken);

            //GET api/produtos
            using (var response = await client.GetAsync(URI))
            {
                if (response.IsSuccessStatusCode)
                {
                    var produtoJsonString = await response.Content.ReadAsStringAsync();
                    List<Produto> produtos = JsonConvert.DeserializeObject<Produto[]>(produtoJsonString).ToList();
                    return produtos;
                }
                else
                {
                    throw new Exception("Não foi possível obter os produtos: " + response.StatusCode);
                }
            }
        }
    }

    public static void GetHeaderTokenAuthorization(HttpClient client, AccessToken accessToken)
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken.Token);
    }
}
