using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataStore.Migrations
{
    /// <inheritdoc />
    public partial class ChangedMessageStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_hashes_users_operator_id",
                table: "hashes");

            migrationBuilder.DropColumn(
                name: "is_operator",
                table: "users");

            migrationBuilder.AddColumn<bool>(
                name: "is_operator_message",
                table: "messages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "operators",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    username = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operators", x => x.id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_hashes_operators_operator_id",
                table: "hashes",
                column: "operator_id",
                principalTable: "operators",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_hashes_operators_operator_id",
                table: "hashes");

            migrationBuilder.DropTable(
                name: "operators");

            migrationBuilder.DropColumn(
                name: "is_operator_message",
                table: "messages");

            migrationBuilder.AddColumn<bool>(
                name: "is_operator",
                table: "users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_hashes_users_operator_id",
                table: "hashes",
                column: "operator_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
