using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Contracts.Factories;
using DB.Application.Contracts.Persistance;
using DB.Infrastructure.Factories;
using DB.Infrastructure.Persistance;
using DB.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DB.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SqlServerContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SqlServerConnectionString")));

            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));

            services.AddScoped<IInstitutionRepository, InstitutionRepository>();
            services.AddScoped<IInstitutionFactory, InstitutionFactory>();
            services.AddScoped<IInstitutionViewModelFactory, InstitutionViewModelFactory>();

            services.AddScoped<IInstrumentRepository, InstrumentRepository>();
            services.AddScoped<IInstrumentFactory, InstrumentFactory>();
            services.AddScoped<IInstrumentViewModelFactory, InstrumentViewModelFactory>();

            services.AddScoped<IMicroorganismRepository, MicroorganismRepository>();
            services.AddScoped<IMicroorganismFactory, MicroorganismFactory>();
            services.AddScoped<IMicroorganismViewModelFactory, MicroorganismViewModelFactory>();

            services.AddScoped<IKeywordRepository, KeywordRepository>();
            services.AddScoped<IKeywordFactory, KeywordFactory>();
            services.AddScoped<IKeywordViewModelFactory, KeywordViewModelFactory>();

            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeFactory, EmployeeFactory>();
            services.AddScoped<IEmployeeViewModelFactory, EmployeeViewModelFactory>();

            services.AddScoped<IAnalysisRepository, AnalysisRepository>();
            services.AddScoped<IAnalysisFactory, AnalysisFactory>();
            services.AddScoped<IAnalysisViewModelFactory, AnalysisViewModelFactory>();

            return services;
        }
    }
}
