using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSalaryToExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Salaries_SalaryId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_SalaryId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "SalaryId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "TradePointEconomicsId",
                table: "Expenses");

            migrationBuilder.AddColumn<Guid>(
                name: "ExpenseId",
                table: "Salaries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Salaries_ExpenseId",
                table: "Salaries",
                column: "ExpenseId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Salaries_Expenses_ExpenseId",
                table: "Salaries",
                column: "ExpenseId",
                principalTable: "Expenses",
                principalColumn: "ExpenseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Salaries_Expenses_ExpenseId",
                table: "Salaries");

            migrationBuilder.DropIndex(
                name: "IX_Salaries_ExpenseId",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "ExpenseId",
                table: "Salaries");

            migrationBuilder.AddColumn<Guid>(
                name: "SalaryId",
                table: "Expenses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TradePointEconomicsId",
                table: "Expenses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_SalaryId",
                table: "Expenses",
                column: "SalaryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Salaries_SalaryId",
                table: "Expenses",
                column: "SalaryId",
                principalTable: "Salaries",
                principalColumn: "SalaryId");
        }
    }
}
