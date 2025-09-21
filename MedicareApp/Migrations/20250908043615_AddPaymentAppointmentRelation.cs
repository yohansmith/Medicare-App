using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicareApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentAppointmentRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_PaymentID",
                table: "Appointments");

            migrationBuilder.AddColumn<int>(
                name: "AppointmentID",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PaymentID",
                table: "Appointments",
                column: "PaymentID",
                unique: true,
                filter: "[PaymentID] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_PaymentID",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AppointmentID",
                table: "Payments");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PaymentID",
                table: "Appointments",
                column: "PaymentID");
        }
    }
}
