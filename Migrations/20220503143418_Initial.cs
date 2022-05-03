using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFrameworkCoreMultiTenancy.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Animals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Kind = table.Column<string>(type: "TEXT", nullable: false),
                    Tenant = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animals", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "Kind", "Name", "Tenant" },
                values: new object[] { 1, "Dog", "Samson", "Khalid" });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "Kind", "Name", "Tenant" },
                values: new object[] { 2, "Dog", "Guiness", "Khalid" });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "Kind", "Name", "Tenant" },
                values: new object[] { 3, "Cat", "Grumpy Cat", "Internet" });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "Kind", "Name", "Tenant" },
                values: new object[] { 4, "Cat", "Mr. Bigglesworth", "Internet" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Animals");
        }
    }
}
