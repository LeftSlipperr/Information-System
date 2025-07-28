using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Incidents",
                columns: table => new
                {
                    IncidentId = table.Column<Guid>(type: "uuid", nullable: false),
                    IncidentName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidents", x => x.IncidentId);
                });

            migrationBuilder.CreateTable(
                name: "TradePoints",
                columns: table => new
                {
                    TradePointId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradePoints", x => x.TradePointId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Workers",
                columns: table => new
                {
                    WorkerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SecondName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ThirdName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workers", x => x.WorkerId);
                });

            migrationBuilder.CreateTable(
                name: "BalanceAnalysis",
                columns: table => new
                {
                    BalanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalIncome = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalExpense = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ProfitOrLoss = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Period = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TradePointId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceAnalysis", x => x.BalanceId);
                    table.ForeignKey(
                        name: "FK_BalanceAnalysis_TradePoints_TradePointId",
                        column: x => x.TradePointId,
                        principalTable: "TradePoints",
                        principalColumn: "TradePointId");
                });

            migrationBuilder.CreateTable(
                name: "TradePointEconomics",
                columns: table => new
                {
                    TradePointEconomicsId = table.Column<Guid>(type: "uuid", nullable: false),
                    Revenue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Expenses = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Profit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Period = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TradePointId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradePointEconomics", x => x.TradePointEconomicsId);
                    table.ForeignKey(
                        name: "FK_TradePointEconomics_TradePoints_TradePointId",
                        column: x => x.TradePointId,
                        principalTable: "TradePoints",
                        principalColumn: "TradePointId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    LogId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_Logs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Logs_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Salaries",
                columns: table => new
                {
                    SalaryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WorkerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salaries", x => x.SalaryId);
                    table.ForeignKey(
                        name: "FK_Salaries_Workers_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Workers",
                        principalColumn: "WorkerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Incomes",
                columns: table => new
                {
                    IncomeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    BalanceAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                    TradePointEconomicsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incomes", x => x.IncomeId);
                    table.ForeignKey(
                        name: "FK_Incomes_BalanceAnalysis_BalanceAnalysisId",
                        column: x => x.BalanceAnalysisId,
                        principalTable: "BalanceAnalysis",
                        principalColumn: "BalanceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Incomes_TradePointEconomics_TradePointEconomicsId",
                        column: x => x.TradePointEconomicsId,
                        principalTable: "TradePointEconomics",
                        principalColumn: "TradePointEconomicsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    ExpenseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpenseName = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    BalanceAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                    SalaryId = table.Column<Guid>(type: "uuid", nullable: true),
                    TradePointEconomicsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.ExpenseId);
                    table.ForeignKey(
                        name: "FK_Expenses_BalanceAnalysis_BalanceAnalysisId",
                        column: x => x.BalanceAnalysisId,
                        principalTable: "BalanceAnalysis",
                        principalColumn: "BalanceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expenses_Salaries_SalaryId",
                        column: x => x.SalaryId,
                        principalTable: "Salaries",
                        principalColumn: "SalaryId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Expenses_TradePointEconomics_TradePointEconomicsId",
                        column: x => x.TradePointEconomicsId,
                        principalTable: "TradePointEconomics",
                        principalColumn: "TradePointEconomicsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DebtReceivable",
                columns: table => new
                {
                    DebtReceivableId = table.Column<Guid>(type: "uuid", nullable: false),
                    DebtorName = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IncomeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtReceivable", x => x.DebtReceivableId);
                    table.ForeignKey(
                        name: "FK_DebtReceivable_Incomes_IncomeId",
                        column: x => x.IncomeId,
                        principalTable: "Incomes",
                        principalColumn: "IncomeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinishedGoodsWarehouse",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    PotentialRevenue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IncomeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinishedGoodsWarehouse", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_FinishedGoodsWarehouse_Incomes_IncomeId",
                        column: x => x.IncomeId,
                        principalTable: "Incomes",
                        principalColumn: "IncomeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DebtsPayable",
                columns: table => new
                {
                    DebtPayableId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditorName = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpenseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtsPayable", x => x.DebtPayableId);
                    table.ForeignKey(
                        name: "FK_DebtsPayable_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "ExpenseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RawMaterialWarehouse",
                columns: table => new
                {
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    PurchaseCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpenseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawMaterialWarehouse", x => x.MaterialId);
                    table.ForeignKey(
                        name: "FK_RawMaterialWarehouse_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "ExpenseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BalanceAnalysis_TradePointId",
                table: "BalanceAnalysis",
                column: "TradePointId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtReceivable_IncomeId",
                table: "DebtReceivable",
                column: "IncomeId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtsPayable_ExpenseId",
                table: "DebtsPayable",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_BalanceAnalysisId",
                table: "Expenses",
                column: "BalanceAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_SalaryId",
                table: "Expenses",
                column: "SalaryId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TradePointEconomicsId",
                table: "Expenses",
                column: "TradePointEconomicsId");

            migrationBuilder.CreateIndex(
                name: "IX_FinishedGoodsWarehouse_IncomeId",
                table: "FinishedGoodsWarehouse",
                column: "IncomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_BalanceAnalysisId",
                table: "Incomes",
                column: "BalanceAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_TradePointEconomicsId",
                table: "Incomes",
                column: "TradePointEconomicsId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserId",
                table: "Logs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserId1",
                table: "Logs",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_RawMaterialWarehouse_ExpenseId",
                table: "RawMaterialWarehouse",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Salaries_WorkerId",
                table: "Salaries",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_TradePointEconomics_TradePointId",
                table: "TradePointEconomics",
                column: "TradePointId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DebtReceivable");

            migrationBuilder.DropTable(
                name: "DebtsPayable");

            migrationBuilder.DropTable(
                name: "FinishedGoodsWarehouse");

            migrationBuilder.DropTable(
                name: "Incidents");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "RawMaterialWarehouse");

            migrationBuilder.DropTable(
                name: "Incomes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "BalanceAnalysis");

            migrationBuilder.DropTable(
                name: "Salaries");

            migrationBuilder.DropTable(
                name: "TradePointEconomics");

            migrationBuilder.DropTable(
                name: "Workers");

            migrationBuilder.DropTable(
                name: "TradePoints");
        }
    }
}
