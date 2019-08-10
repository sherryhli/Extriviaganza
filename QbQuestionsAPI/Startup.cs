using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using QbQuestionsAPI.Domain.Models;
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

            // Configure JWT authentication
            services.Configure<TokenPayload>(Configuration.GetSection("tokenPayload"));
            TokenPayload tokenPayload = Configuration.GetSection("tokenPayload").Get<TokenPayload>();
            // Overwrite dummy secret value in appsettings.json with actual value from Azure Key Vault
            const string issuerSigningKeyId = "https://extriviaganza-vault.vault.azure.net/secrets/QbQuestionsIssuerSigningKey";
            tokenPayload.Secret = GetKeyVaultSecret(issuerSigningKeyId).Result;

            services
                .AddAuthentication(x => {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x => {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = 
                        new TokenValidationParameters()
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenPayload.Secret)),
                            ValidIssuer = tokenPayload.Issuer,
                            ValidAudience = tokenPayload.Audience,
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                });

            // Connect to database
            const string connectinStringId = "https://extriviaganza-vault.vault.azure.net/secrets/QbQuestionsDbConnectionString";
            string connectionString = GetKeyVaultSecret(connectinStringId).Result;
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

            // Configure service lifetimes
            services.AddScoped<IQbQuestionRepository, QbQuestionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IQbQuestionService, QbQuestionService>();
            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddScoped<IUserService, UserService>();

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
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseHttpsRedirection();
        }

        // TODO: Move this out to a secret management service
        private async Task<string> GetKeyVaultSecret(string secretId)
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            SecretBundle secret = await keyVaultClient.GetSecretAsync(secretId).ConfigureAwait(false);
            return secret.Value;
        }
    }
}
