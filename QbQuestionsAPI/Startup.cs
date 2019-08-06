using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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
            // TODO: Figure out secret management
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
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
    }
}
