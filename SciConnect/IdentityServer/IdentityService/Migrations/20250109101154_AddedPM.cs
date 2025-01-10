using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IdentityService.Migrations
{
    /// <inheritdoc />
    public partial class AddedPM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4810d349-f87a-415d-ada2-9cbfe38650fb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9d34860d-9dbb-40f3-9ce6-9ca5e5a7e7dd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e36ac8a2-2569-49c3-b569-7cf688c46215");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "04eaadd9-fa59-44f9-881b-23fbb0851e3f", null, "Guest", "GUEST" },
                    { "62e40349-08b0-44c4-9f39-47411e8047f7", null, "PM", "PM" },
                    { "a4a2ce71-a9d8-4198-af29-e709934716fb", null, "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "b69fd164-62c0-43ae-8c6a-12d071bd45b8", 0, "8eeb348a-7158-48ff-a678-4195c93a41b8", "pm1@example.com", true, "PM", "User", false, null, "PM1@EXAMPLE.COM", "PM1@EXAMPLE.COM", "AQAAAAIAAYagAAAAED4ofFPoaGzfxY6/1hWzNFo6GH3rhNZ0zkE9xO6gMU+c6PhZrfqwiEBxrEId7PxJpA==", null, false, "1c14826a-1b61-47b3-a2a8-8e4a7c482efc", false, "pm1@example.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "b69fd164-62c0-43ae-8c6a-12d071bd45b8", "62e40349-08b0-44c4-9f39-47411e8047f7" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "04eaadd9-fa59-44f9-881b-23fbb0851e3f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "62e40349-08b0-44c4-9f39-47411e8047f7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a4a2ce71-a9d8-4198-af29-e709934716fb");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69fd164-62c0-43ae-8c6a-12d071bd45b8");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4810d349-f87a-415d-ada2-9cbfe38650fb", null, "Guest", "GUEST" },
                    { "9d34860d-9dbb-40f3-9ce6-9ca5e5a7e7dd", null, "PM", "PM" },
                    { "e36ac8a2-2569-49c3-b569-7cf688c46215", null, "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "b69fd164-62c0-43ae-8c6a-12d071bd45b8", "62e40349-08b0-44c4-9f39-47411e8047f7" });
        }
    }
}
