using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ecommerce_API.Migrations
{
    public partial class MenusUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_subMenus_Menu_MenuId",
                table: "subMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_subSubMenuLinks_subMenus_SubMenuId",
                table: "subSubMenuLinks");

            migrationBuilder.AlterColumn<int>(
                name: "SubMenuId",
                table: "subSubMenuLinks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MenuId",
                table: "subMenus",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_subMenus_Menu_MenuId",
                table: "subMenus",
                column: "MenuId",
                principalTable: "Menu",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_subSubMenuLinks_subMenus_SubMenuId",
                table: "subSubMenuLinks",
                column: "SubMenuId",
                principalTable: "subMenus",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_subMenus_Menu_MenuId",
                table: "subMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_subSubMenuLinks_subMenus_SubMenuId",
                table: "subSubMenuLinks");

            migrationBuilder.AlterColumn<int>(
                name: "SubMenuId",
                table: "subSubMenuLinks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MenuId",
                table: "subMenus",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_subMenus_Menu_MenuId",
                table: "subMenus",
                column: "MenuId",
                principalTable: "Menu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_subSubMenuLinks_subMenus_SubMenuId",
                table: "subSubMenuLinks",
                column: "SubMenuId",
                principalTable: "subMenus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
