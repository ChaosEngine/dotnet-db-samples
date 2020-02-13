using System.Collections.Generic;

namespace InkBall.Module.Model
{
	public enum CommandKindEnum
	{
		UNKNOWN = -1,
		PING = 0,
		POINT = 1,
		PATH = 2,
		PLAYER_JOINING = 3,
		PLAYER_SURRENDER = 4,
		WIN = 5,
		POINTS_AND_PATHS = 6,
		USER_SETTINGS = 7,
		STOP_AND_DRAW = 8
	}

	public interface IDtoMsg
	{
		CommandKindEnum GetKind();
	}

	public sealed class PingCommand : IDtoMsg
	{
		public string Message { get; set; }

		PingCommand()
		{ }

		public PingCommand(string message)
		{
			Message = message;
		}

		public PingCommand(PingCommand parent)
		{
			this.Message = parent.Message;
		}

		public CommandKindEnum GetKind()
		{
			return CommandKindEnum.PING;
		}
	}

	public sealed class PlayerJoiningCommand : IDtoMsg
	{
		public int OtherPlayerId { get; private set; }

		public string OtherPlayerName { get; set; }

		public string Message { get; set; }

		public PlayerJoiningCommand(int otherPlayerId, string otherPlayerName, string message)
		{
			OtherPlayerId = otherPlayerId;
			OtherPlayerName = otherPlayerName;
			Message = message;
		}

		public CommandKindEnum GetKind()
		{
			return CommandKindEnum.PLAYER_JOINING;
		}
	}

	public sealed class PlayerSurrenderingCommand : IDtoMsg
	{
		public int? OtherPlayerId { get; private set; }

		public bool ThisOrOtherPlayerSurrenders { get; private set; }

		public string Message { get; set; }

		public PlayerSurrenderingCommand(int? otherPlayerId, bool thisOrOtherPlayerSurrenders, string message)
		{
			OtherPlayerId = otherPlayerId;
			ThisOrOtherPlayerSurrenders = thisOrOtherPlayerSurrenders;
			Message = message;
		}

		public CommandKindEnum GetKind()
		{
			return CommandKindEnum.PLAYER_SURRENDER;
		}
	}

	public sealed class WinCommand : IDtoMsg
	{
		public int WinningPlayerId { get; }

		public InkBallGame.WinStatusEnum Status { get; }

		public string Message { get; }

		public WinCommand(InkBallGame.WinStatusEnum status, int winningPlayerId, string message)
		{
			this.Status = status;
			this.WinningPlayerId = winningPlayerId;
			this.Message = message;
		}

		public CommandKindEnum GetKind()
		{
			return CommandKindEnum.WIN;
		}
	}

	public sealed class StopAndDrawCommand : IDtoMsg
	{
		public StopAndDrawCommand()
		{
		}

		public CommandKindEnum GetKind()
		{
			return CommandKindEnum.STOP_AND_DRAW;
		}
	}

	public sealed class PlayerPointsAndPathsDTO : IDtoMsg
	{
		public string Points { get; }

		public string Paths { get; }

		public PlayerPointsAndPathsDTO(string points, string paths)
		{
			this.Points = points;
			this.Paths = paths;
		}

		public CommandKindEnum GetKind()
		{
			return CommandKindEnum.POINTS_AND_PATHS;
		}
	}
}
