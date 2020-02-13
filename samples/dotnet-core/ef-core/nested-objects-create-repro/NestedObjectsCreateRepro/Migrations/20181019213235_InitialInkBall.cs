using System;
using InkBall.Module.Model;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Oracle.EntityFrameworkCore.Metadata;

namespace InkBall.Module.Migrations
{
	public partial class InitialInkBall : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "InkBallUsers",
				columns: table => new
				{
					iId = table.Column<int>(nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
						.Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
					iPrivileges = table.Column<int>(nullable: false, defaultValue: 0)
						.Annotation("Sqlite:Autoincrement", true),
					sExternalId = table.Column<string>(nullable: true),
					UserName = table.Column<string>(maxLength: 256, nullable: true),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_InkBallUsers", x => x.iId);
				});

			migrationBuilder.CreateTable(
				name: "InkBallPlayer",
				columns: table => new
				{
					iId = table.Column<int>(nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
						.Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
					iUserID = table.Column<int>(nullable: true),
					sLastMoveCode = table.Column<string>(type: GamesContext.JsonColumnTypeFromProvider(this.ActiveProvider), nullable: true),
					iWinCount = table.Column<int>(nullable: false, defaultValue: 0)
						.Annotation("Sqlite:Autoincrement", true),
					iLossCount = table.Column<int>(nullable: false, defaultValue: 0)
						.Annotation("Sqlite:Autoincrement", true),
					iDrawCount = table.Column<int>(nullable: false, defaultValue: 0)
						.Annotation("Sqlite:Autoincrement", true),
					TimeStamp = table.Column<DateTime>(type: GamesContext.TimeStampColumnTypeFromProvider(this.ActiveProvider), nullable: false,
						defaultValueSql: GamesContext.TimeStampDefaultValueFromProvider(this.ActiveProvider))
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_InkBallPlayer", x => x.iId);
					table.ForeignKey(
						name: "InkBallPlayer_ibfk_1",
						column: x => x.iUserID,
						principalTable: "InkBallUsers",
						principalColumn: "iId",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "InkBallGame",
				columns: table => new
				{
					iId = table.Column<int>(nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
						.Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
					iPlayer1ID = table.Column<int>(nullable: false),
					iPlayer2ID = table.Column<int>(nullable: true),
					bIsPlayer1Active = table.Column<bool>(nullable: false, defaultValue: true),
					iGridSize = table.Column<int>(nullable: false, defaultValue: 16)
						.Annotation("Sqlite:Autoincrement", true),
					iBoardWidth = table.Column<int>(nullable: false, defaultValue: 20)
						.Annotation("Sqlite:Autoincrement", true),
					iBoardHeight = table.Column<int>(nullable: false, defaultValue: 26)
						.Annotation("Sqlite:Autoincrement", true),
					GameType = table.Column<string>(nullable: false),
					GameState = table.Column<string>(nullable: false),
					TimeStamp = table.Column<DateTime>(type: GamesContext.TimeStampColumnTypeFromProvider(this.ActiveProvider), nullable: false,
						defaultValueSql: GamesContext.TimeStampDefaultValueFromProvider(this.ActiveProvider)),
					CreateTime = table.Column<DateTime>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_InkBallGame", x => x.iId);
					table.ForeignKey(
						name: "InkBallGame_ibfk_1",
						column: x => x.iPlayer1ID,
						principalTable: "InkBallPlayer",
						principalColumn: "iId",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "InkBallGame_ibfk_2",
						column: x => x.iPlayer2ID,
						principalTable: "InkBallPlayer",
						principalColumn: "iId",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "InkBallPath",
				columns: table => new
				{
					iId = table.Column<int>(nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
						.Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
					iGameID = table.Column<int>(nullable: false),
					iPlayerID = table.Column<int>(nullable: false),
					PointsAsString = table.Column<string>(type: GamesContext.JsonColumnTypeFromProvider(this.ActiveProvider), nullable: true),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_InkBallPath", x => x.iId);
					table.ForeignKey(
						name: "InkBallPath_ibfk_1",
						column: x => x.iGameID,
						principalTable: "InkBallGame",
						principalColumn: "iId",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "InkBallPath_ibfk_2",
						column: x => x.iPlayerID,
						principalTable: "InkBallPlayer",
						principalColumn: "iId",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "InkBallPoint",
				columns: table => new
				{
					iId = table.Column<int>(nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
						.Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
					iGameID = table.Column<int>(nullable: false),
					iPlayerID = table.Column<int>(nullable: false),
					iX = table.Column<int>(nullable: false),
					iY = table.Column<int>(nullable: false),
					Status = table.Column<int>(nullable: false, defaultValue: (int)InkBallPoint.StatusEnum.POINT_FREE),
					iEnclosingPathId = table.Column<int>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_InkBallPoint", x => x.iId);
					table.ForeignKey(
						name: "InkBallPoint_ibfk_5",
						column: x => x.iEnclosingPathId,
						principalTable: "InkBallPath",
						principalColumn: "iId",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "InkBallPoint_ibfk_3",
						column: x => x.iGameID,
						principalTable: "InkBallGame",
						principalColumn: "iId",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "InkBallPoint_ibfk_4",
						column: x => x.iPlayerID,
						principalTable: "InkBallPlayer",
						principalColumn: "iId",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "InkBallPointsInPath",
				columns: table => new
				{
					iId = table.Column<int>(nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
						.Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
					iPathId = table.Column<int>(nullable: false),
					iPointId = table.Column<int>(nullable: false),
					Order = table.Column<int>(nullable: false, defaultValue: 0)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_InkBallPointsInPath", x => x.iId);
					table.ForeignKey(
						name: "InkBallPointsInPath_ibfk_1",
						column: x => x.iPathId,
						principalTable: "InkBallPath",
						principalColumn: "iId",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "InkBallPointsInPath_ibfk_2",
						column: x => x.iPointId,
						principalTable: "InkBallPoint",
						principalColumn: "iId",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateIndex(
				name: "ByPlayer1",
				table: "InkBallGame",
				column: "iPlayer1ID");

			migrationBuilder.CreateIndex(
				name: "ByPlayer2",
				table: "InkBallGame",
				column: "iPlayer2ID");

			migrationBuilder.CreateIndex(
				name: "IDX_InkBallPath_ByGame",
				table: "InkBallPath",
				column: "iGameID");

			migrationBuilder.CreateIndex(
				name: "IDX_InkBallPath_ByPlayer",
				table: "InkBallPath",
				column: "iPlayerID");

			migrationBuilder.CreateIndex(
				name: "ByUser",
				table: "InkBallPlayer",
				column: "iUserID");

			migrationBuilder.CreateIndex(
				name: "ByEnclosingPath",
				table: "InkBallPoint",
				column: "iEnclosingPathId");

			migrationBuilder.CreateIndex(
				name: "IDX_InkBallPoint_ByGame",
				table: "InkBallPoint",
				column: "iGameID");

			migrationBuilder.CreateIndex(
				name: "IDX_InkBallPoint_ByPlayer",
				table: "InkBallPoint",
				column: "iPlayerID");

			migrationBuilder.CreateIndex(
				name: "ByPath",
				table: "InkBallPointsInPath",
				column: "iPathId");

			migrationBuilder.CreateIndex(
				name: "ByPoint",
				table: "InkBallPointsInPath",
				column: "iPointId");

			migrationBuilder.CreateIndex(
				name: "sExternalId",
				table: "InkBallUsers",
				column: "sExternalId",
				unique: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "InkBallPointsInPath");

			migrationBuilder.DropTable(
				name: "InkBallPoint");

			migrationBuilder.DropTable(
				name: "InkBallPath");

			migrationBuilder.DropTable(
				name: "InkBallGame");

			migrationBuilder.DropTable(
				name: "InkBallPlayer");

			migrationBuilder.DropTable(
				name: "InkBallUsers");
		}
	}
}
