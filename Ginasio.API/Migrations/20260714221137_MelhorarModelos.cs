using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ginasio.API.Migrations
{
    /// <inheritdoc />
    public partial class MelhorarModelos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Membros_Planos_PlanoId",
                table: "Membros");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Planos",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Planos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Planos",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Membros",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Membros",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Membros",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInscricao",
                table: "Membros",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataNascimento",
                table: "Membros",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefone",
                table: "Membros",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Aulas",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Instrutor",
                table: "Aulas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "Ativa",
                table: "Aulas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataHora",
                table: "Aulas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DuracaoMinutos",
                table: "Aulas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LotacaoMaxima",
                table: "Aulas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Membros_Email",
                table: "Membros",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Membros_Planos_PlanoId",
                table: "Membros",
                column: "PlanoId",
                principalTable: "Planos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Membros_Planos_PlanoId",
                table: "Membros");

            migrationBuilder.DropIndex(
                name: "IX_Membros_Email",
                table: "Membros");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Planos");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Planos");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Membros");

            migrationBuilder.DropColumn(
                name: "DataInscricao",
                table: "Membros");

            migrationBuilder.DropColumn(
                name: "DataNascimento",
                table: "Membros");

            migrationBuilder.DropColumn(
                name: "Telefone",
                table: "Membros");

            migrationBuilder.DropColumn(
                name: "Ativa",
                table: "Aulas");

            migrationBuilder.DropColumn(
                name: "DataHora",
                table: "Aulas");

            migrationBuilder.DropColumn(
                name: "DuracaoMinutos",
                table: "Aulas");

            migrationBuilder.DropColumn(
                name: "LotacaoMaxima",
                table: "Aulas");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Planos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Membros",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Membros",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Aulas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "Instrutor",
                table: "Aulas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_Membros_Planos_PlanoId",
                table: "Membros",
                column: "PlanoId",
                principalTable: "Planos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
