using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QbPackParser.APIClient
{
    public class QbQuestionsClient : HttpClient
    {
        private readonly string url = "API URL";

        public QbQuestionsClient() : base()
        {
            
        }

        public async Task<HttpResponseMessage> AddQuestionsToDb(string content)
        {
            StringContent stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            return await PostAsync(url, stringContent);
        }
    }
}
