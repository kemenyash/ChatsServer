using Extensions;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataStore.Migrations
{
    /// <inheritdoc />
    public partial class DefaultOperatorInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var salt = $"{StringHelper.ComputeHash(StringHelper.GetRandomString())}";
            var token = $"{salt}{StringHelper.GetRandomString()}{salt}";
            var password = StringHelper.ComputeHash($"{salt}tOp6W5l4Bn3sUi{salt}");
            
            migrationBuilder.InsertData(table: "operators",
                                        columns: new[] { "username" },
                                        values: new[] { "agent" });
            
            migrationBuilder.InsertData(table: "hashes",
                                        columns: new[] { "token", "password", "salt", "operator_id" },
                                        values: new object[] {token, password, salt, 1});
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
