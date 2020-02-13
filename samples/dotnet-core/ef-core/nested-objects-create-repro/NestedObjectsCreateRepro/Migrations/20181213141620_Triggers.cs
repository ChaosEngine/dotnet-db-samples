using System;
using System.Linq;
using InkBall.Module.Model;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InkBall.Module.Migrations
{
	public partial class Triggers : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			var entity = TargetModel.FindEntityType(typeof(InkBallGame).FullName);
			if (entity != null && entity.Name == typeof(InkBallGame).FullName)
			{
				migrationBuilder.CreateTimestampTrigger(entity, nameof(InkBallGame.TimeStamp), nameof(InkBallGame.iId));
			}

			entity = TargetModel.FindEntityType(typeof(InkBallPlayer).FullName);
			if (entity != null && entity.Name == typeof(InkBallPlayer).FullName)
			{
				migrationBuilder.CreateTimestampTrigger(entity, nameof(InkBallPlayer.TimeStamp), nameof(InkBallPlayer.iId));
			}
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			var entity = TargetModel.FindEntityType(typeof(InkBallGame).FullName);
			if (entity != null && entity.Name == typeof(InkBallGame).FullName)
			{
				migrationBuilder.DropTimestampTrigger(entity, nameof(InkBallGame.TimeStamp));
			}
			entity = TargetModel.FindEntityType(typeof(InkBallPlayer).FullName);
			if (entity != null && entity.Name == typeof(InkBallPlayer).FullName)
			{
				migrationBuilder.DropTimestampTrigger(entity, nameof(InkBallPlayer.TimeStamp));
			}
		}
	}
}
