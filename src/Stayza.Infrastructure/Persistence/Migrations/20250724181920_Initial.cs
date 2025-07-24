using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Stayza.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Author = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ISBN = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmailAddress = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserType = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookCopies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsRetired = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookCopies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookCopies_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookCopyId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeRange_Start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TimeRange_End = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ReturnDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Loans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookCopyId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReservedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_BookCopies_BookCopyId",
                        column: x => x.BookCopyId,
                        principalTable: "BookCopies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "ISBN", "Title" },
                values: new object[,]
                {
                    { new Guid("6106dfed-7f89-43f0-8a5d-22775be15211"), "Gregor Hohpe, Bobby Woolf", "9780321200686", "Enterprise Integration Patterns" },
                    { new Guid("61415c09-6dd2-42d8-a1e4-03774558a4b5"), "Andrew Hunt, David Thomas", "9780135957059", "The Pragmatic Programmer" },
                    { new Guid("c5e0a48f-9bcc-4e39-8474-81df953756a2"), "Gregor Hohpe", "9781642955706", "The Architect's Elevator" },
                    { new Guid("cb295459-2d9f-40ef-b423-62375b5db562"), "Vlad Khononov", "9781617298413", "Balancing Coupling in Software Design" },
                    { new Guid("e0c22377-4fe9-4d95-89bb-61f498c8c1be"), "Juval Löwy", "9781642955706", "Righting Software" }
                });

            migrationBuilder.InsertData(
                table: "BookCopies",
                columns: new[] { "Id", "BookId", "IsRetired" },
                values: new object[,]
                {
                    { new Guid("16d7bbf1-474d-4036-a81b-e3ce0b64a361"), new Guid("6106dfed-7f89-43f0-8a5d-22775be15211"), false },
                    { new Guid("2c80e65f-f8a5-4c2f-84a0-773fd71293b5"), new Guid("61415c09-6dd2-42d8-a1e4-03774558a4b5"), false },
                    { new Guid("535e94c0-e2e9-4375-8f48-3caf42e5f58c"), new Guid("cb295459-2d9f-40ef-b423-62375b5db562"), false },
                    { new Guid("95ea8f22-ad65-4278-ba50-eb96af41397e"), new Guid("e0c22377-4fe9-4d95-89bb-61f498c8c1be"), false },
                    { new Guid("f0d7cdc7-8fe1-4a5f-9ac3-94ae0587d9a5"), new Guid("c5e0a48f-9bcc-4e39-8474-81df953756a2"), false }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_BookId",
                table: "BookCopies",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_UserId",
                table: "Loans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_BookCopyId",
                table: "Reservations",
                column: "BookCopyId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UserId",
                table: "Reservations",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "BookCopies");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
