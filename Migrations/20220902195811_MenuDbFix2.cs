using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ecommerce_API.Migrations
{
    public partial class MenuDbFix2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Links_subMenu_subMenuId",
                table: "Links");

            migrationBuilder.DropForeignKey(
                name: "FK_subMenu_Menu_MenuId",
                table: "subMenu");

            migrationBuilder.AlterColumn<int>(
                name: "MenuId",
                table: "subMenu",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "subMenuId",
                table: "Links",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Links_subMenu_subMenuId",
                table: "Links",
                column: "subMenuId",
                principalTable: "subMenu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_subMenu_Menu_MenuId",
                table: "subMenu",
                column: "MenuId",
                principalTable: "Menu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Links_subMenu_subMenuId",
                table: "Links");

            migrationBuilder.DropForeignKey(
                name: "FK_subMenu_Menu_MenuId",
                table: "subMenu");

            migrationBuilder.AlterColumn<int>(
                name: "MenuId",
                table: "subMenu",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "subMenuId",
                table: "Links",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Links_subMenu_subMenuId",
                table: "Links",
                column: "subMenuId",
                principalTable: "subMenu",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_subMenu_Menu_MenuId",
                table: "subMenu",
                column: "MenuId",
                principalTable: "Menu",
                principalColumn: "Id");
        }
    }
}
