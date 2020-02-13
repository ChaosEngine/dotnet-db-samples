using System;
using System.Collections.Generic;
using System.Linq;

namespace InkBall.Module.Model
{
	public interface IUser<Player, Point, Path>
		where Player : IPlayer<Point, Path>
		where Point : IPoint
		where Path : IPath<Point>
	{
		int iId { get; set; }
		int iPrivileges { get; set; }
		string sExternalId { get; set; }
		string UserName { get; set; }

		ICollection<Player> InkBallPlayer { get; set; }
	}

	public partial class InkBallUser : IUser<InkBallPlayer, InkBallPoint, InkBallPath>
	{
		public int iId { get; set; }
		public int iPrivileges { get; set; }
		public string sExternalId { get; set; }
		public string UserName { get; set; }

		public ICollection<InkBallPlayer> InkBallPlayer { get; set; }

		public InkBallUser()
		{
			// InkBallPlayer = new HashSet<InkBallPlayer>();
		}
	}

	[Serializable]
	public sealed class InkBallUserViewModel : IUser<InkBallPlayerViewModel, InkBallPointViewModel, InkBallPathViewModel>
	{
		public int iId { get; set; }
		public int iPrivileges { get; set; }
		public string sExternalId { get; set; }
		public string UserName { get; set; }

		public ICollection<InkBallPlayerViewModel> InkBallPlayer { get; set; }

		public InkBallUserViewModel()
		{ }

		public InkBallUserViewModel(InkBallUser user)
		{
			iId = user.iId;
			iPrivileges = user.iPrivileges;
			sExternalId = user.sExternalId;
			UserName = user.UserName;

			if (user?.InkBallPlayer?.Count > 0)
			{
				InkBallPlayer = user.InkBallPlayer.Select(p => new InkBallPlayerViewModel(p, false)).ToArray();
			}
		}

		// [JsonConstructor]
		public InkBallUserViewModel(InkBallUserViewModel user)
		{
			iId = user.iId;
			iPrivileges = user.iPrivileges;
			sExternalId = user.sExternalId;
			UserName = user.UserName;

			if (user?.InkBallPlayer?.Count > 0)
			{
				InkBallPlayer = user.InkBallPlayer;
			}
		}
	}
}
