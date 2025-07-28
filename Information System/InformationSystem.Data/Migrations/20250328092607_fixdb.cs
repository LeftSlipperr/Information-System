using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceAnalysis_TradePoints_TradePointId",
                table: "BalanceAnalysis");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_BalanceAnalysis_BalanceAnalysisId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Salaries_SalaryId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_BalanceAnalysis_BalanceAnalysisId",
                table: "Incomes");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_UserId1",
                table: "Logs");

            migrationBuilder.DropTable(
                name: "DebtReceivable");

            migrationBuilder.DropTable(
                name: "DebtsPayable");

            migrationBuilder.DropIndex(
                name: "IX_TradePointEconomics_TradePointId",
                table: "TradePointEconomics");

            migrationBuilder.DropIndex(
                name: "IX_Logs_UserId1",
                table: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_Incomes_BalanceAnalysisId",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_BalanceAnalysisId",
                table: "Expenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BalanceAnalysis",
                table: "BalanceAnalysis");

            migrationBuilder.DropIndex(
                name: "IX_BalanceAnalysis_TradePointId",
                table: "BalanceAnalysis");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "FinishedGoodsWarehouse");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Incidents",
                newName: "IncidentDate");

            migrationBuilder.AlterColumn<string>(
                name: "ThirdName",
                table: "Workers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "SecondName",
                table: "Workers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Workers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TradePoints",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "TradePoints",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "MaterialName",
                table: "RawMaterialWarehouse",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<DateTime>(
                name: "BalanceAnalysisPeriod",
                table: "Incomes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "IncidentName",
                table: "Incidents",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "IncidentDescription",
                table: "Incidents",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "FinishedGoodsWarehouse",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "BalanceAnalysisPeriod",
                table: "Expenses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<Guid>(
                name: "TradePointId",
                table: "BalanceAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BalanceAnalysis",
                table: "BalanceAnalysis",
                column: "Period");

            migrationBuilder.CreateTable(
                name: "DebtPayables",
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
                    table.PrimaryKey("PK_DebtPayables", x => x.DebtPayableId);
                    table.ForeignKey(
                        name: "FK_DebtPayables_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "ExpenseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DebtReceivables",
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
                    table.PrimaryKey("PK_DebtReceivables", x => x.DebtReceivableId);
                    table.ForeignKey(
                        name: "FK_DebtReceivables_Incomes_IncomeId",
                        column: x => x.IncomeId,
                        principalTable: "Incomes",
                        principalColumn: "IncomeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TradePointEconomics_TradePointId",
                table: "TradePointEconomics",
                column: "TradePointId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_BalanceAnalysisPeriod",
                table: "Incomes",
                column: "BalanceAnalysisPeriod");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_BalanceAnalysisPeriod",
                table: "Expenses",
                column: "BalanceAnalysisPeriod");

            migrationBuilder.CreateIndex(
                name: "IX_DebtPayables_ExpenseId",
                table: "DebtPayables",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtReceivables_IncomeId",
                table: "DebtReceivables",
                column: "IncomeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_BalanceAnalysis_BalanceAnalysisPeriod",
                table: "Expenses",
                column: "BalanceAnalysisPeriod",
                principalTable: "BalanceAnalysis",
                principalColumn: "Period",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Salaries_SalaryId",
                table: "Expenses",
                column: "SalaryId",
                principalTable: "Salaries",
                principalColumn: "SalaryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_BalanceAnalysis_BalanceAnalysisPeriod",
                table: "Incomes",
                column: "BalanceAnalysisPeriod",
                principalTable: "BalanceAnalysis",
                principalColumn: "Period",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_BalanceAnalysis_BalanceAnalysisPeriod",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Salaries_SalaryId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_BalanceAnalysis_BalanceAnalysisPeriod",
                table: "Incomes");

            migrationBuilder.DropTable(
                name: "DebtPayables");

            migrationBuilder.DropTable(
                name: "DebtReceivables");

            migrationBuilder.DropIndex(
                name: "IX_TradePointEconomics_TradePointId",
                table: "TradePointEconomics");

            migrationBuilder.DropIndex(
                name: "IX_Incomes_BalanceAnalysisPeriod",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_BalanceAnalysisPeriod",
                table: "Expenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BalanceAnalysis",
                table: "BalanceAnalysis");

            migrationBuilder.DropColumn(
                name: "BalanceAnalysisPeriod",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "IncidentDescription",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "FinishedGoodsWarehouse");

            migrationBuilder.DropColumn(
                name: "BalanceAnalysisPeriod",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "IncidentDate",
                table: "Incidents",
                newName: "Date");

            migrationBuilder.AlterColumn<string>(
                name: "ThirdName",
                table: "Workers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "SecondName",
                table: "Workers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Workers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TradePoints",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "TradePoints",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "MaterialName",
                table: "RawMaterialWarehouse",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Logs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "IncidentName",
                table: "Incidents",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Incidents",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "FinishedGoodsWarehouse",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "TradePointId",
                table: "BalanceAnalysis",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BalanceAnalysis",
                table: "BalanceAnalysis",
                column: "BalanceId");

            migrationBuilder.CreateTable(
                name: "DebtReceivable",
                columns: table => new
                {
                    DebtReceivableId = table.Column<Guid>(type: "uuid", nullable: false),
                    IncomeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DebtorName = table.Column<string>(type: "text", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "DebtsPayable",
                columns: table => new
                {
                    DebtPayableId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpenseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreditorName = table.Column<string>(type: "text", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_TradePointEconomics_TradePointId",
                table: "TradePointEconomics",
                column: "TradePointId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserId1",
                table: "Logs",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_BalanceAnalysisId",
                table: "Incomes",
                column: "BalanceAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_BalanceAnalysisId",
                table: "Expenses",
                column: "BalanceAnalysisId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceAnalysis_TradePoints_TradePointId",
                table: "BalanceAnalysis",
                column: "TradePointId",
                principalTable: "TradePoints",
                principalColumn: "TradePointId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_BalanceAnalysis_BalanceAnalysisId",
                table: "Expenses",
                column: "BalanceAnalysisId",
                principalTable: "BalanceAnalysis",
                principalColumn: "BalanceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Salaries_SalaryId",
                table: "Expenses",
                column: "SalaryId",
                principalTable: "Salaries",
                principalColumn: "SalaryId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_BalanceAnalysis_BalanceAnalysisId",
                table: "Incomes",
                column: "BalanceAnalysisId",
                principalTable: "BalanceAnalysis",
                principalColumn: "BalanceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_UserId1",
                table: "Logs",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
