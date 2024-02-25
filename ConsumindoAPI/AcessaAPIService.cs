﻿using Newtonsoft.Json;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text;

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

    public async Task<Produto> GetProdutoById(string URI, AccessToken accessToken)
    {
        using (var client = new HttpClient())
        {
            GetHeaderTokenAuthorization(client, accessToken);

            //GET api/produtos/id
            HttpResponseMessage response = await client.GetAsync(URI);
            if (response.IsSuccessStatusCode)
            {
                var ProdutoJsonString = await response.Content.ReadAsStringAsync();
                Produto produto = JsonConvert.DeserializeObject<Produto>(ProdutoJsonString);
                return produto;
            }
            else
            {
                throw new Exception("Falha ao obter o produto : " + response.StatusCode);
            }
        }
    }

    public async Task<string> AddProduto(string URI, AccessToken accessToken, Produto produto)
    {
        using (var client = new HttpClient())
        {
            GetHeaderTokenAuthorization(client, accessToken);

            var serializedProduto = JsonConvert.SerializeObject(produto);
            var content = new StringContent(serializedProduto, Encoding.UTF8, "application/json");
            //POST api/produtos produto
            var result = await client.PostAsync(URI, content);

            if (result.IsSuccessStatusCode)
            {
                return "Produto incluido com sucesso";
            }
            else
            {
                return "Falha ao incluir produto " + result.StatusCode;
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
