using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InkBall.Module.Model
{
	public interface IGame<Player, Point, Path>
		where Player : IPlayer<Point, Path>
		where Point : IPoint
		where Path : IPath<Point>
	{
		bool bIsPlayer1Active { get; set; }
		DateTime CreateTime { get; set; }
		InkBallGame.GameStateEnum GameState { get; set; }
		InkBallGame.GameTypeEnum GameType { get; set; }
		int iBoardHeight { get; set; }
		int iBoardWidth { get; set; }
		int LogicalWidth { get; }
		int LogicalHeight { get; }
		int iGridSize { get; set; }
		int iId { get; set; }
		ICollection<Path> InkBallPath { get; set; }
		ICollection<Point> InkBallPoint { get; set; }
		int iPlayer1Id { get; set; }
		int? iPlayer2Id { get; set; }
		Player Player1 { get; set; }
		Player Player2 { get; set; }
		DateTime TimeStamp { get; set; }

		bool IsThisPlayer1();
		Player GetPlayer();
		Player GetOtherPlayer();
		bool IsThisPlayerActive();
		bool IsThisPlayerPlayingWithRed();
	}

	public abstract class CommonGame<Player, Point, Path>
		where Player : IPlayer<Point, Path>
		where Point : IPoint
		where Path : IPath<Point>
	{
		protected internal readonly static TimeSpan _deactivationDelayInSeconds = TimeSpan.FromSeconds(120);

		[NotMapped]//Hide it from EF Core
		[JsonPropertyName("bIsPlayer1")]//allow to serialize it
		protected internal bool bIsPlayer1 { get; set; }
		public int iId { get; set; }
		public abstract Player Player1 { get; set; }
		public abstract Player Player2 { get; set; }
		public abstract ICollection<Path> InkBallPath { get; set; }
		public abstract ICollection<Point> InkBallPoint { get; set; }
		public int iPlayer1Id { get; set; }
		public int? iPlayer2Id { get; set; }
		public bool bIsPlayer1Active { get; set; }
		public int iGridSize { get; set; }
		public int iBoardWidth { get; set; }
		public int iBoardHeight { get; set; }
		public InkBallGame.GameTypeEnum GameType { get; set; }
		public InkBallGame.GameStateEnum GameState { get; set; }
		public DateTime TimeStamp { get; set; }
		public DateTime CreateTime { get; set; }

		public static TimeSpan GetDeactivationDelayInSeconds() => InkBallGame._deactivationDelayInSeconds;

		public bool IsThisPlayer1() => this.bIsPlayer1;

		public bool IsPlayer1Active() => this.bIsPlayer1Active;

		public bool IsThisPlayerActive()
		{
			if (this.bIsPlayer1)
				return this.bIsPlayer1Active;
			else
				return !this.bIsPlayer1Active;
		}

		public bool IsThisPlayerPlayingWithRed() => this.bIsPlayer1;

		public Player GetPlayer()
		{
			if (this.bIsPlayer1 == true)
				return this.Player1;
			else
				return this.Player2;
		}

		public Player GetOtherPlayer()
		{
			if (this.bIsPlayer1 == false)
				return this.Player1;
			else
				return this.Player2;
		}

		public Player GetPlayer1() => this.Player1;

		public Player GetPlayer2() => this.Player2;

		public void SetState(InkBallGame.GameStateEnum value)
		{
			this.GameState = value;
		}

		public async Task<InkBallGame.WinStatusEnum> Check4Win(InkBallPoint.StatusEnum playerOwningColor, InkBallPoint.StatusEnum otherPlayerOwningColor,
			Func<int, ValueTask<int>> pathsCountForPlayerFunc, Func<InkBallPoint.StatusEnum, ValueTask<int>> ownedPointForColorFunc)
		{
			switch (this.GameType)
			{
				case InkBallGame.GameTypeEnum.FIRST_CAPTURE:
					if (await pathsCountForPlayerFunc(GetPlayer().iId) > 0)
					{
						if (this.IsThisPlayerPlayingWithRed())
							return InkBallGame.WinStatusEnum.RED_WINS;
						else
							return InkBallGame.WinStatusEnum.GREEN_WINS;
					}
					else if (await pathsCountForPlayerFunc(GetOtherPlayer().iId) > 0)
					{
						if (this.IsThisPlayerPlayingWithRed())
							return InkBallGame.WinStatusEnum.GREEN_WINS;
						else
							return InkBallGame.WinStatusEnum.RED_WINS;
					}
					return InkBallGame.WinStatusEnum.NO_WIN;//continue game


				case InkBallGame.GameTypeEnum.FIRST_5_CAPTURES:
					if (await ownedPointForColorFunc(otherPlayerOwningColor) >= 5)
					{
						if (this.IsThisPlayerPlayingWithRed())
							return InkBallGame.WinStatusEnum.GREEN_WINS;
						else
							return InkBallGame.WinStatusEnum.RED_WINS;
					}
					else if (await ownedPointForColorFunc(playerOwningColor) >= 5)
					{
						if (this.IsThisPlayerPlayingWithRed())
							return InkBallGame.WinStatusEnum.RED_WINS;
						else
							return InkBallGame.WinStatusEnum.GREEN_WINS;
					}
					return InkBallGame.WinStatusEnum.NO_WIN;//continue game


				case InkBallGame.GameTypeEnum.FIRST_5_PATHS:
					if (await pathsCountForPlayerFunc(GetPlayer().iId) >= 5)
					{
						if (this.IsThisPlayerPlayingWithRed())
							return InkBallGame.WinStatusEnum.RED_WINS;
						else
							return InkBallGame.WinStatusEnum.GREEN_WINS;
					}
					else if (await pathsCountForPlayerFunc(GetOtherPlayer().iId) >= 5)
					{
						if (this.IsThisPlayerPlayingWithRed())
							return InkBallGame.WinStatusEnum.GREEN_WINS;
						else
							return InkBallGame.WinStatusEnum.RED_WINS;
					}
					return InkBallGame.WinStatusEnum.NO_WIN;//continue game


				case InkBallGame.GameTypeEnum.FIRST_5_ADVANTAGE_PATHS:
					var diff = await pathsCountForPlayerFunc(GetPlayer().iId) - await pathsCountForPlayerFunc(GetOtherPlayer().iId);
					if (diff >= 5)
					{
						if (this.IsThisPlayerPlayingWithRed())
							return InkBallGame.WinStatusEnum.RED_WINS;
						else
							return InkBallGame.WinStatusEnum.GREEN_WINS;
					}
					else if (diff <= -5)
					{
						if (this.IsThisPlayerPlayingWithRed())
							return InkBallGame.WinStatusEnum.GREEN_WINS;
						else
							return InkBallGame.WinStatusEnum.RED_WINS;
					}
					return InkBallGame.WinStatusEnum.NO_WIN;//continue game


				default:
					throw new ArgumentException("Wrong game type", nameof(this.GameType));
			}
		}

		public async Task<InkBallGame.WinStatusEnum> Check4Win(IPointAndPathCounter pointAndPathCounter)
		{
			switch (this.GameType)
			{
				case InkBallGame.GameTypeEnum.FIRST_CAPTURE:
					if (await pointAndPathCounter.GetThisPlayerPathCountAsync() > 0)
					{
						if (this.IsThisPlayerPlayingWithRed())
							return InkBallGame.WinStatusEnum.RED_WINS;
						else
							return InkBallGame.WinStatusEnum.GREEN_WINS;
					}
					else if (await pointAndPathCounter.GetOtherPlayerPathCountAsync() > 0)
					{
						if (this.IsThisPlayerPlayingWithRed())
							return InkBallGame.WinStatusEnum.GREEN_WINS;
						else
							return InkBallGame.WinStatusEnum.RED_WINS;
					}
					return InkBallGame.WinStatusEnum.NO_WIN;//continue game


				case InkBallGame.GameTypeEnum.FIRST_5_CAPTURES:
					if (await pointAndPathCounter.GetOtherPlayerOwnedPointCountAsync() >= 5)
					{
						if (this.IsThisPlayerPlayingWithRed())
							return InkBallGame.WinStatusEnum.GREEN_WINS;
						else
							return InkBallGame.WinStatusEnum.RED_WINS;
					}
					else if (await pointAndPathCounter.GetThisPlayerOwnedPointCountAsync() >= 5)
					{
						if (this.IsThisPlayerPlayingWithRed())
							return InkBallGame.WinStatusEnum.RED_WINS;
						else
							return InkBallGame.WinStatusEnum.GREEN_WINS;
					}
					return InkBallGame.WinStatusEnum.NO_WIN;//continue game


				case InkBallGame.GameTypeEnum.FIRST_5_PATHS:
					if (await pointAndPathCounter.GetThisPlayerPathCountAsync() >= 5)
					{
						if (this.IsThisPlayerPlayingWithRed())
							return InkBallGame.WinStatusEnum.RED_WINS;
						else
							return InkBallGame.WinStatusEnum.GREEN_WINS;
					}
					else if (await pointAndPathCounter.GetOtherPlayerPathCountAsync() >= 5)
					{
						if (this.IsThisPlayerPlayingWithRed())
							return InkBallGame.WinStatusEnum.GREEN_WINS;
						else
							return InkBallGame.WinStatusEnum.RED_WINS;
					}
					return InkBallGame.WinStatusEnum.NO_WIN;//continue game


				case InkBallGame.GameTypeEnum.FIRST_5_ADVANTAGE_PATHS:
					var diff = await pointAndPathCounter.GetThisPlayerPathCountAsync() - await pointAndPathCounter.GetOtherPlayerPathCountAsync();
					if (diff >= 5)
					{
						if (this.IsThisPlayerPlayingWithRed())
							return InkBallGame.WinStatusEnum.RED_WINS;
						else
							return InkBallGame.WinStatusEnum.GREEN_WINS;
					}
					else if (diff <= -5)
					{
						if (this.IsThisPlayerPlayingWithRed())
							return InkBallGame.WinStatusEnum.GREEN_WINS;
						else
							return InkBallGame.WinStatusEnum.RED_WINS;
					}
					return InkBallGame.WinStatusEnum.NO_WIN;//continue game


				default:
					throw new ArgumentException("Wrong game type", nameof(this.GameType));
			}
		}
	}

	public partial class InkBallGame : CommonGame<InkBallPlayer, InkBallPoint, InkBallPath>
	{
		public enum GameTypeEnum
		{
			FIRST_CAPTURE = 0,
			FIRST_5_CAPTURES = 1,
			FIRST_5_PATHS = 2,
			FIRST_5_ADVANTAGE_PATHS = 3
		}

		public enum GameStateEnum
		{
			INACTIVE,
			ACTIVE,
			AWAITING,
			FINISHED
		}

		public enum WinStatusEnum
		{
			RED_WINS = 0,
			GREEN_WINS = 1,
			NO_WIN = 2,
			DRAW_WIN = 3
		}

		public enum BoardSizeEnum
		{
			SIZE_20x26 = 20,
			SIZE_40x52 = 40,
			SIZE_64x64 = 64,
			// SIZE_80x80 = 80
		}

		public override InkBallPlayer Player1 { get; set; }
		public override InkBallPlayer Player2 { get; set; }
		public override ICollection<InkBallPath> InkBallPath { get; set; }
		public override ICollection<InkBallPoint> InkBallPoint { get; set; }

		public InkBallGame()
		{
			// InkBallPath = new HashSet<InkBallPath>();
			// InkBallPoint = new HashSet<InkBallPoint>();
		}

		internal static void DeactivateDeadGamezFromExternalUserID(string sExternalUserID)
		{
			//TODO: implement this
		}

		internal static void WipeAllDeadGamez()
		{
			//TODO: implement this
		}
	}

	//[Serializable]
	public class InkBallGameViewModel : CommonGame<InkBallPlayerViewModel, InkBallPointViewModel, InkBallPathViewModel>
	{
		public override ICollection<InkBallPathViewModel> InkBallPath { get; set; }
		public override ICollection<InkBallPointViewModel> InkBallPoint { get; set; }
		public override InkBallPlayerViewModel Player1 { get; set; }
		public override InkBallPlayerViewModel Player2 { get; set; }

		public InkBallGameViewModel()
		{
		}

		public InkBallGameViewModel(InkBallGame game)
		{
			bIsPlayer1Active = game.bIsPlayer1Active;
			bIsPlayer1 = game.bIsPlayer1;
			CreateTime = game.CreateTime;
			GameState = game.GameState;
			GameType = game.GameType;
			iBoardHeight = game.iBoardHeight;
			iBoardWidth = game.iBoardWidth;
			iGridSize = game.iGridSize;
			iId = game.iId;

			if (game?.InkBallPath?.Count > 0)
			{
				InkBallPath = game.InkBallPath.Select(p => new InkBallPathViewModel(p)).ToArray();
			}
			if (game?.InkBallPoint?.Count > 0)
			{
				InkBallPoint = game.InkBallPoint.Select(p => new InkBallPointViewModel(p)).ToArray();
			}

			iPlayer1Id = game.iPlayer1Id;
			iPlayer2Id = game.iPlayer2Id;
			Player1 = new InkBallPlayerViewModel(game.Player1);
			if (game.Player2 != null)
				Player2 = new InkBallPlayerViewModel(game.Player2);
			TimeStamp = game.TimeStamp;
		}

		public InkBallGameViewModel(InkBallGameViewModel game)
		{
			bIsPlayer1Active = game.bIsPlayer1Active;
			bIsPlayer1 = game.bIsPlayer1;
			CreateTime = game.CreateTime;
			GameState = game.GameState;
			GameType = game.GameType;
			iBoardHeight = game.iBoardHeight;
			iBoardWidth = game.iBoardWidth;
			iGridSize = game.iGridSize;
			iId = game.iId;

			if (game?.InkBallPath?.Count > 0)
			{
				InkBallPath = game.InkBallPath;
			}
			if (game?.InkBallPoint?.Count > 0)
			{
				InkBallPoint = game.InkBallPoint;
			}

			iPlayer1Id = game.iPlayer1Id;
			iPlayer2Id = game.iPlayer2Id;
			Player1 = game.Player1;
			Player2 = game.Player2;
			TimeStamp = game.TimeStamp;
		}
	}
}
