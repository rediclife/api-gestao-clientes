using Domain.ExternalInterfaces;
using Entities.ExternalEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.Repository.ExternalRepositories
{
    public class RepositoryEnderecoCepExt : IEnderecoCepExt
    {
        private readonly string urlApi = "https://viacep.com.br/ws/";
        public EnderecoCepExt Consultar(string cep)
        {
            EnderecoCepExt enderecoRet = new EnderecoCepExt();
            try
            {
                string url = urlApi + cep + "/json/";
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var retorno = response.Content.ReadAsStringAsync();
                        enderecoRet = JsonConvert.DeserializeObject<EnderecoCepExt>(retorno.Result);
                    }
                    return enderecoRet;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
