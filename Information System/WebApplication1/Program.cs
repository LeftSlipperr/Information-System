using InformationSystem.Application.Interfaces;
using InformationSystem.Application.Services;
using Infrastructure;
using Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("InformationSystemDb");

        builder.Services.AddDbContext<InformationSystemDbContext>(options =>
            options.UseNpgsql(connectionString));

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        builder.Services.AddScoped<IBalanceAnalysisService, BalanceAnalysisService>();
        builder.Services.AddScoped<IBalanceAnalysisStorage, BalanseAnalisysStorage>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IUserStorage, UserStorage>();
        builder.Services.AddScoped<IIncomeService, IncomeService>();
        builder.Services.AddScoped<IIncomeStorage, IncomeStorage>();
        builder.Services.AddScoped<IExpenseService, ExpenseService>();
        builder.Services.AddScoped<IExpenseStorage, ExpenseStorage>();
        builder.Services.AddScoped<IDebtPayableService, DebtPayableService>();
        builder.Services.AddScoped<IDebtPayableStorage, DebtPayableStorage>();
        builder.Services.AddScoped<IDebtReceivableService, DebtReceivableService>();
        builder.Services.AddScoped<IDebtReceivableStorage, DebtReceivableStorage>();
        builder.Services.AddScoped<IIncidentService, IncidentService>();
        builder.Services.AddScoped<IIncidentStorage, IncidentStorage>();
        builder.Services.AddScoped<IFinishedGoodsWarehouseService, FinishedGoodsWarehouseService>();
        builder.Services.AddScoped<IFinishedGoodsWarehouseStorage, FinishedGoodsWarehouseStorage>();
        builder.Services.AddScoped<ILogService, LogService>();
        builder.Services.AddScoped<ILogStorage, LogStorage>();
        builder.Services.AddScoped<IRawMaterialWarehouseService, RawMaterialWarehouseService>();
        builder.Services.AddScoped<IRawMaterialWarehouseStorage, RawMaterialWarehouseStorage>();
        builder.Services.AddScoped<ISalaryService, SalaryService>();
        builder.Services.AddScoped<ISalaryStorage, SalaryStorage>();
        builder.Services.AddScoped<ITradePointEconomicsService, TradePointEconomicService>();
        builder.Services.AddScoped<ITradePointEconomicsStorage, TradePointEconomicsStorage>();
        builder.Services.AddScoped<ITradePointService, TradePointService>();
        builder.Services.AddScoped<ITradePointStorage, TradePointStorage>();
        builder.Services.AddScoped<IWorkerService, WorkerService>();
        builder.Services.AddScoped<IWorkerStorage, WorkerStorage>();



        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.RoutePrefix = string.Empty; // Делает Swagger главной страницей
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}