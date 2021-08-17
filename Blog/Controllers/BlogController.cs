using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Blog.Controllers
{
    [RoutePrefix("api/Blog")]
    public class BlogController : ApiController
    {
        private string remoteApiUrl;
        private string remoteKeyword;
        private string remoteToken;
        public BlogController()
        {
            remoteApiUrl = ConfigurationManager.AppSettings["GnewsApi"];
            remoteToken = ConfigurationManager.AppSettings["Token"];  
            remoteKeyword = ConfigurationManager.AppSettings["Keyword"];
        }
        
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetBlogs()
        {
            if (remoteApiUrl == null || remoteApiUrl == "") {
                return BadRequest("Api was not provided");
            }

            try
            {
                var request = string.Concat("search?q=", remoteKeyword, "&token=", remoteToken);
                var response = await RemoteApiConectionAsync(request, "GET");
                if (response.IsSuccessStatusCode)
                {
                    return Ok(JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result));
                }
                else
                {
                    return BadRequest("Api is unavailable!");
                }
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Open HTTP conection to remote api 
        /// </summary>
        private async Task<HttpResponseMessage> RemoteApiConectionAsync(string request, string method)
        {

            using (var client = new HttpClient())
            {
               
                client.BaseAddress = new Uri(remoteApiUrl);
                
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage httpResponse = new HttpResponseMessage();
                switch (method)
                {
                    case "GET":
                        httpResponse = await client.GetAsync(request);
                        break;
                    case "POST":
                       // Not Implemented
                        break;
                    default:
                        httpResponse = await client.GetAsync(request);
                        break;
                }
                return httpResponse;
            }

        }

    }
}
