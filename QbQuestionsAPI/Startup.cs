using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading.Tasks;

using QbQuestionsAPI.Domain.Repositories;
using QbQuestionsAPI.Domain.Services;
using QbQuestionsAPI.Mapping;
using QbQuestionsAPI.Persistence.Contexts;
using QbQuestionsAPI.Persistence.Repositories;
using QbQuestionsAPI.Services;

namespace QbQuestionsAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            string connectionString = GetConnectionString().Result;
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IQbQuestionRepository, QbQuestionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IQbQuestionService, QbQuestionService>();

            services.AddAutoMapper(
                Assembly.GetAssembly(typeof(ModelToResourceProfile)),
                Assembly.GetAssembly(typeof(ResourceToModelProfile))
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private async Task<string> GetConnectionString()
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            SecretBundle secret = await keyVaultClient.GetSecretAsync("https://extriviaganza-vault.vault.azure.net/secrets/QbQuestionsDbConnectionString").ConfigureAwait(false);
            return secret.Value;
        }
    }
}
