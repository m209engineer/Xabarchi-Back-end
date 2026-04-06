using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Xabarchi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixReactionUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reactions_UserId_MessageId",
                table: "Reactions");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_UserId_MessageId_Content",
                table: "Reactions",
                columns: new[] { "UserId", "MessageId", "Content" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reactions_UserId_MessageId_Content",
                table: "Reactions");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_UserId_MessageId",
                table: "Reactions",
                columns: new[] { "UserId", "MessageId" },
                unique: true);
        }
    }
}
