using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixIncome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_BalanceAnalysis_BalanceAnalysisPeriod",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_BalanceAnalysis_BalanceAnalysisPeriod",
                table: "Incomes");

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
                name: "BalanceAnalysisPeriod",
                table: "Expenses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BalanceAnalysis",
                table: "BalanceAnalysis",
                column: "BalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_BalanceAnalysisId",
                table: "Incomes",
                column: "BalanceAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_BalanceAnalysisId",
                table: "Expenses",
                column: "BalanceAnalysisId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_BalanceAnalysis_BalanceAnalysisId",
                table: "Expenses",
                column: "BalanceAnalysisId",
                principalTable: "BalanceAnalysis",
                principalColumn: "BalanceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_BalanceAnalysis_BalanceAnalysisId",
                table: "Incomes",
                column: "BalanceAnalysisId",
                principalTable: "BalanceAnalysis",
                principalColumn: "BalanceId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_BalanceAnalysis_BalanceAnalysisId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_BalanceAnalysis_BalanceAnalysisId",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Incomes_BalanceAnalysisId",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_BalanceAnalysisId",
                table: "Expenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BalanceAnalysis",
                table: "BalanceAnalysis");

            migrationBuilder.AddColumn<DateTime>(
                name: "BalanceAnalysisPeriod",
                table: "Incomes",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "BalanceAnalysisPeriod",
                table: "Expenses",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_BalanceAnalysis",
                table: "BalanceAnalysis",
                column: "Period");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_BalanceAnalysisPeriod",
                table: "Incomes",
                column: "BalanceAnalysisPeriod");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_BalanceAnalysisPeriod",
                table: "Expenses",
                column: "BalanceAnalysisPeriod");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_BalanceAnalysis_BalanceAnalysisPeriod",
                table: "Expenses",
                column: "BalanceAnalysisPeriod",
                principalTable: "BalanceAnalysis",
                principalColumn: "Period",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_BalanceAnalysis_BalanceAnalysisPeriod",
                table: "Incomes",
                column: "BalanceAnalysisPeriod",
                principalTable: "BalanceAnalysis",
                principalColumn: "Period",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
