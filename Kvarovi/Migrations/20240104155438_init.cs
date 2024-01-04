using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kvarovi.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "announcementtype",
                columns: table => new
                {
                    announcementtypeid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    announcementtypename = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_announcementtype", x => x.announcementtypeid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "keyword",
                columns: table => new
                {
                    keywordid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    word = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_keyword", x => x.keywordid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    userid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    devicetoken = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.userid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "announcement",
                columns: table => new
                {
                    announcementid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    announcementtypeid = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    text = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_announcement", x => x.announcementid);
                    table.ForeignKey(
                        name: "fk_announcement_announcementtype_announcementtypeid",
                        column: x => x.announcementtypeid,
                        principalTable: "announcementtype",
                        principalColumn: "announcementtypeid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "keyworduser",
                columns: table => new
                {
                    keywordskeywordid = table.Column<int>(type: "int", nullable: false),
                    usersuserid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_keyworduser", x => new { x.keywordskeywordid, x.usersuserid });
                    table.ForeignKey(
                        name: "fk_keyworduser_keyword_keywordskeywordid",
                        column: x => x.keywordskeywordid,
                        principalTable: "keyword",
                        principalColumn: "keywordid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_keyworduser_user_usersuserid",
                        column: x => x.usersuserid,
                        principalTable: "user",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "announcementkeyword",
                columns: table => new
                {
                    announcementsannouncementid = table.Column<int>(type: "int", nullable: false),
                    keywordskeywordid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_announcementkeyword", x => new { x.announcementsannouncementid, x.keywordskeywordid });
                    table.ForeignKey(
                        name: "fk_announcementkeyword_announcement_announcementsannouncementid",
                        column: x => x.announcementsannouncementid,
                        principalTable: "announcement",
                        principalColumn: "announcementid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_announcementkeyword_keyword_keywordskeywordid",
                        column: x => x.keywordskeywordid,
                        principalTable: "keyword",
                        principalColumn: "keywordid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_announcement_announcementtypeid",
                table: "announcement",
                column: "announcementtypeid");

            migrationBuilder.CreateIndex(
                name: "ix_announcementkeyword_keywordskeywordid",
                table: "announcementkeyword",
                column: "keywordskeywordid");

            migrationBuilder.CreateIndex(
                name: "ix_keyword_word",
                table: "keyword",
                column: "word",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_keyworduser_usersuserid",
                table: "keyworduser",
                column: "usersuserid");

            migrationBuilder.CreateIndex(
                name: "ix_user_devicetoken",
                table: "user",
                column: "devicetoken",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "announcementkeyword");

            migrationBuilder.DropTable(
                name: "keyworduser");

            migrationBuilder.DropTable(
                name: "announcement");

            migrationBuilder.DropTable(
                name: "keyword");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "announcementtype");
        }
    }
}
