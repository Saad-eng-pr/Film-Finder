using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackerDeFavorisApi.Migrations.Film
{
    /// <inheritdoc />
    public partial class CreateFilmsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IMDB",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "Titre",
                table: "Films");

            migrationBuilder.AlterColumn<string>(
                name: "Year",
                table: "Films",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Poster",
                table: "Films",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Films",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Films",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "imdbID",
                table: "Films",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "imdbID",
                table: "Films");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "Films",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Poster",
                table: "Films",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IMDB",
                table: "Films",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Titre",
                table: "Films",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
