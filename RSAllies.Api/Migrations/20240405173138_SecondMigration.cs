using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RSAllies.Api.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsDeleted", "LastName", "Phone", "UpdatedAt" },
                values: new object[] { new Guid("0176e150-17f5-452e-b455-135fc52464a1"), new DateTime(2024, 4, 5, 20, 31, 34, 958, DateTimeKind.Local).AddTicks(8011), "john.doe@example.com", "John", false, "Doe", "1234567890", null });

            migrationBuilder.InsertData(
                table: "Venues",
                columns: new[] { "Id", "Address", "Capacity", "CreatedAt", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[] { new Guid("beffca8d-e238-4775-84f2-9384221b6fa6"), "123 Street, City, Country", 100, new DateTime(2024, 4, 5, 20, 31, 34, 958, DateTimeKind.Local).AddTicks(8488), false, "Venue 1", null });

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "CreatedAt", "CurrentCapacity", "IsDeleted", "SessionDate", "UpdatedAt", "VenueId" },
                values: new object[] { new Guid("287fb764-21ae-4692-9aeb-acb10f272c88"), new DateTime(2024, 4, 5, 20, 31, 34, 958, DateTimeKind.Local).AddTicks(8719), 0, false, new DateTime(2024, 4, 12, 20, 31, 34, 958, DateTimeKind.Local).AddTicks(8712), null, new Guid("beffca8d-e238-4775-84f2-9384221b6fa6") });

            migrationBuilder.InsertData(
                table: "VenueAvailabilities",
                columns: new[] { "Id", "AvailableDate", "CreatedAt", "IsDeleted", "UpdatedAt", "VenueId" },
                values: new object[] { new Guid("891dc9c2-ee4a-4f8b-b351-f1c097435063"), new DateTime(2024, 4, 12, 20, 31, 34, 958, DateTimeKind.Local).AddTicks(8598), new DateTime(2024, 4, 5, 20, 31, 34, 958, DateTimeKind.Local).AddTicks(8611), false, null, new Guid("beffca8d-e238-4775-84f2-9384221b6fa6") });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "BookingDate", "CreatedAt", "IsDeleted", "SessionId", "Status", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("3dde55aa-bc06-469a-9679-5084db85df06"), new DateTime(2024, 4, 5, 20, 31, 34, 958, DateTimeKind.Local).AddTicks(8809), new DateTime(2024, 4, 5, 20, 31, 34, 958, DateTimeKind.Local).AddTicks(8813), false, new Guid("287fb764-21ae-4692-9aeb-acb10f272c88"), "Booked", null, new Guid("0176e150-17f5-452e-b455-135fc52464a1") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: new Guid("3dde55aa-bc06-469a-9679-5084db85df06"));

            migrationBuilder.DeleteData(
                table: "VenueAvailabilities",
                keyColumn: "Id",
                keyValue: new Guid("891dc9c2-ee4a-4f8b-b351-f1c097435063"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("287fb764-21ae-4692-9aeb-acb10f272c88"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0176e150-17f5-452e-b455-135fc52464a1"));

            migrationBuilder.DeleteData(
                table: "Venues",
                keyColumn: "Id",
                keyValue: new Guid("beffca8d-e238-4775-84f2-9384221b6fa6"));
        }
    }
}
