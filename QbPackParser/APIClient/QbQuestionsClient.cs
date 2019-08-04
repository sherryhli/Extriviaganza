using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QbPackParser.APIClient
{
    public class QbQuestionsClient : HttpClient
    {
        private readonly string url = "API URL";

        /// <summary>Creates a new instance of QbQuestionsClient</summary>
        public QbQuestionsClient() : base()
        {
            
        }

        /// <summary>Adds questions to the database via a REST API call</summary>
        /// <param name="content">The questions to add in the form of a JSON array</param>
        public async Task<HttpResponseMessage> AddQuestionsToDbAsync(string content)
        {
            StringContent stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            return await PostAsync(url, stringContent);
        }
    }
}
