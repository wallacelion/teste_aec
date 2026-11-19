using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class SeedUserAdm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
               table: "Users",
               columns: new[] { "Uid", "Name", "Email", "Password", "Role" },
               values: new object[] { "{74745C7F-DFED-4605-937A-1BF88E95F37E}", "Admin", "admin@example.com", "AQAAAAIAAYagAAAAEHKy6Q3naJYHyIKJnje9aK/6FP3j5GCaX/b87ywBkh0bI93hjJe5lZbvn4H8hUiwdA==", 0 }
           );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
               table: "Users",
               keyColumn: "Uid",
               keyValue: "{74745C7F-DFED-4605-937A-1BF88E95F37E}"
           );
        }
    }
}
