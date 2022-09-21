using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ecommerce_API.Migrations
{
    public partial class MenuDbFix4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Brand");

            migrationBuilder.AddColumn<byte[]>(
                name: "image",
                table: "Category",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "image",
                table: "Brand",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "image",
                table: "Brand");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Category",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Brand",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
