using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTradePointEconomics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_TradePointEconomics_TradePointEconomicsId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_TradePointEconomics_TradePointEconomicsId",
                table: "Incomes");

            migrationBuilder.DropForeignKey(
                name: "FK_Salaries_Workers_WorkerId",
                table: "Salaries");

            migrationBuilder.DropIndex(
                name: "IX_Salaries_WorkerId",
                table: "Salaries");

            migrationBuilder.DropIndex(
                name: "IX_Incomes_TradePointEconomicsId",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_TradePointEconomicsId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "WorkerId",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "TradePointId",
                table: "BalanceAnalysis");

            migrationBuilder.AddColumn<Guid>(
                name: "SalaryId",
                table: "Workers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ExpenseId",
                table: "TradePointEconomics",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "IncomeId",
                table: "TradePointEconomics",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Workers_SalaryId",
                table: "Workers",
                column: "SalaryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradePointEconomics_ExpenseId",
                table: "TradePointEconomics",
                column: "ExpenseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradePointEconomics_IncomeId",
                table: "TradePointEconomics",
                column: "IncomeId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TradePointEconomics_Expenses_ExpenseId",
                table: "TradePointEconomics",
                column: "ExpenseId",
                principalTable: "Expenses",
                principalColumn: "ExpenseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TradePointEconomics_Incomes_IncomeId",
                table: "TradePointEconomics",
                column: "IncomeId",
                principalTable: "Incomes",
                principalColumn: "IncomeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Workers_Salaries_SalaryId",
                table: "Workers",
                column: "SalaryId",
                principalTable: "Salaries",
                principalColumn: "SalaryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TradePointEconomics_Expenses_ExpenseId",
                table: "TradePointEconomics");

            migrationBuilder.DropForeignKey(
                name: "FK_TradePointEconomics_Incomes_IncomeId",
                table: "TradePointEconomics");

            migrationBuilder.DropForeignKey(
                name: "FK_Workers_Salaries_SalaryId",
                table: "Workers");

            migrationBuilder.DropIndex(
                name: "IX_Workers_SalaryId",
                table: "Workers");

            migrationBuilder.DropIndex(
                name: "IX_TradePointEconomics_ExpenseId",
                table: "TradePointEconomics");

            migrationBuilder.DropIndex(
                name: "IX_TradePointEconomics_IncomeId",
                table: "TradePointEconomics");

            migrationBuilder.DropColumn(
                name: "SalaryId",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "ExpenseId",
                table: "TradePointEconomics");

            migrationBuilder.DropColumn(
                name: "IncomeId",
                table: "TradePointEconomics");

            migrationBuilder.AddColumn<Guid>(
                name: "WorkerId",
                table: "Salaries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TradePointId",
                table: "BalanceAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Salaries_WorkerId",
                table: "Salaries",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_TradePointEconomicsId",
                table: "Incomes",
                column: "TradePointEconomicsId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TradePointEconomicsId",
                table: "Expenses",
                column: "TradePointEconomicsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_TradePointEconomics_TradePointEconomicsId",
                table: "Expenses",
                column: "TradePointEconomicsId",
                principalTable: "TradePointEconomics",
                principalColumn: "TradePointEconomicsId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_TradePointEconomics_TradePointEconomicsId",
                table: "Incomes",
                column: "TradePointEconomicsId",
                principalTable: "TradePointEconomics",
                principalColumn: "TradePointEconomicsId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Salaries_Workers_WorkerId",
                table: "Salaries",
                column: "WorkerId",
                principalTable: "Workers",
                principalColumn: "WorkerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
