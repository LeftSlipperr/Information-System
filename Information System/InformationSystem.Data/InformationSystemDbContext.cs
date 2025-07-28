using System.Reflection;
using InformationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class InformationSystemDbContext : DbContext
{
    public InformationSystemDbContext(DbContextOptions<InformationSystemDbContext> options)
       : base(options) { }

    public DbSet<BalanceAnalysis> BalanceAnalysis => Set<BalanceAnalysis>();
    public DbSet<DebtPayable> DebtPayables => Set<DebtPayable>();
    public DbSet<DebtReceivable> DebtReceivables => Set<DebtReceivable>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet <FinishedGoodsWarehouse> FinishedGoodsWarehouse => Set<FinishedGoodsWarehouse>();
    public DbSet <Incident> Incidents => Set<Incident>();
    public DbSet<Income> Incomes => Set<Income>();
    public DbSet<Log> Logs => Set<Log>();
    public DbSet<RawMaterialWarehouse> RawMaterialWarehouse => Set<RawMaterialWarehouse>();
    public DbSet<Salary> Salaries => Set<Salary>();
    public DbSet<TradePointEconomics> TradePointEconomics => Set<TradePointEconomics>();
    public DbSet<TradePoint> TradePoints => Set<TradePoint>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Worker> Workers => Set<Worker>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var assembly = Assembly.GetExecutingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    }
}