using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Primitives;

namespace InkBall.Module.Model
{
	public interface IPath<Point>
		where Point : IPoint
	{
		int iId { get; set; }
		int iGameId { get; set; }
		int iPlayerId { get; set; }

		ICollection<Point> InkBallPoint { get; set; }
	}

	public abstract class CommonPath<Point> : IPath<Point>, IDtoMsg
		where Point : CommonPoint, IPoint
	{
		public int iId { get; set; }
		public int iGameId { get; set; }
		public int iPlayerId { get; set; }
		public string PointsAsString { get; set; }

		public abstract ICollection<Point> InkBallPoint { get; set; }

		/**
		 * Based on http://www.faqs.org/faqs/graphics/algorithms-faq/
		 * but mainly on http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
		 * returns != 0 if point is inside path
		 * @param {number} npol points count
		 * @param {number} xp x point coordinates
		 * @param {number} yp y point coordinates
		 * @param {number} x point to check x coordinate
		 * @param {number} y point to check y coordinate
		 * @returns {boolean} if point lies inside the polygon
		 */
		protected static bool pnpoly(int npol, int[] xp, int[] yp, int x, int y)
		{
			int i, j; bool c = false;

			for (i = 0, j = npol - 1; i < npol; j = i++)
			{
				if ((((yp[i] <= y) && (y < yp[j])) ||
					((yp[j] <= y) && (y < yp[i]))) &&
					(x < (xp[j] - xp[i]) * (y - yp[i]) / (yp[j] - yp[i]) + xp[i]))

					c = !c;
			}
			return c;
		}

		protected static bool pnpoly(ICollection<Point> pathPoints, int x, int y)
		{
			int i, j, npol = pathPoints.Count; bool c = false;

			for (i = 0, j = npol - 1; i < npol; j = i++)
			{
				Point pi = pathPoints.ElementAt(i), pj = pathPoints.ElementAt(j);

				if ((((pi.iY <= y) && (y < pj.iY)) ||
					((pj.iY <= y) && (y < pi.iY))) &&
					(x < (pj.iX - pi.iX) * (y - pi.iY) / (pj.iY - pi.iY) + pi.iX))

					c = !c;
			}
			return c;
		}

		public virtual bool IsPointInsidePath(CommonPoint point)
		{
			var path_points = this.InkBallPoint;
			if (path_points.Contains(point))
				return false;

			return pnpoly(path_points, point.iX, point.iY);
		}

		#region Overrides

		public void OnBeforeSerialize()
		{
			iId = Math.Max(0, iId);
		}

		public void OnAfterDeserialize()
		{
		}

		public CommandKindEnum GetKind()
		{
			return CommandKindEnum.PATH;
		}

		#endregion Overrides

		public bool ShouldSerializeiId()
		{
			// don't serialize the iId property if <= 0
			return (iId > 0);
		}
	}

	public partial class InkBallPath : CommonPath<InkBallPoint>
	{
		public InkBallGame Game { get; set; }
		public InkBallPlayer Player { get; set; }
		public override ICollection<InkBallPoint> InkBallPoint { get; set; }
		public ICollection<InkBallPointsInPath> InkBallPointsInPath { get; set; }

		public InkBallPath()
		{
			// InkBallPoint = new HashSet<InkBallPoint>();
			// InkBallPointsInPath = new HashSet<InkBallPointsInPath>();
		}

		public static string GetPathsAsJavaScriptArrayForPage(IEnumerable<InkBallPath> paths)
		{
			StringBuilder builder = new StringBuilder("[", 300);
			string comma = "";
			foreach (var path in paths)
			{
				var points = path.InkBallPoint;
				builder.AppendFormat("{0}[{1}\"", comma
#if DEBUG
				, $"/*ID={path.iId}*/"
#else
				, ""
#endif
				);

				string space = string.Empty;
				foreach (var point in points)
				{
#if DEBUG
					builder.AppendFormat("{2}{0}/*x*/,{1}/*y*//*id={3}*/", point.iX, point.iY, space, point.iId);
#else
					builder.AppendFormat("{2}{0},{1}", point.iX, point.iY, space);
#endif
					space = " ";
				}

				builder.AppendFormat(
#if DEBUG
					"\",{0}/*playerID*/]",
#else
					"\",{0}]",
#endif
					path.iPlayerId);
				comma = ",\r";
			}
			builder.Append(']');

			return builder.ToString();
		}

		public static string GetPathsAsJavaScriptArrayForPage2(IEnumerable<InkBallPath> paths)
		{
			StringBuilder builder = new StringBuilder("[", 300);
			string comma = "";
			foreach (var path in paths)
			{
				builder.AppendFormat("{0}{1}", comma
#if DEBUG
				, $"/*ID={path.iId}*/"
#else
 				, ""
#endif
				)
			   .Append(path.PointsAsString);

				comma = ",\r";
			}
			builder.Append(']');

			return builder.ToString();
		}

		public static string GetPathsAsJavaScriptArrayForSignalR(IEnumerable<InkBallPath> paths)
		{
			StringBuilder builder = new StringBuilder("[", 300);
			string comma = "";
			foreach (var path in paths)
			{
				var points = path.InkBallPoint;
				builder.AppendFormat("{0}[{1}\"", comma, "");

				string space = string.Empty;
				foreach (var point in points)
				{
					builder.AppendFormat("{2}{0},{1}", point.iX, point.iY, space);
					space = " ";
				}

				builder.AppendFormat("\",{0}]", path.iPlayerId);
				comma = ",";
			}
			builder.Append(']');

			return builder.ToString();
		}

		public static string GetPathsAsJavaScriptArrayForSignalR2(IEnumerable<InkBallPath> paths)
		{
			StringBuilder builder = new StringBuilder("[", 300);
			string comma = "";
			foreach (var path in paths)
			{
				builder.Append(comma).Append(path.PointsAsString);

				comma = ",\r";
			}
			builder.Append(']');

			return builder.ToString();
		}
	}

	public class InkBallPathViewModel : CommonPath<InkBallPointViewModel>
	{
		delegate void ActionRef<T1, T2, T3, T4>(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4);

		#region Fields

		readonly static char[] _spaceSeparatorArr = new char[] { ' ' }, _commaSeparatorArr = new char[] { ',' };

		private ICollection<InkBallPointViewModel> _inkBallPoint;
		private ICollection<InkBallPointViewModel> _ownedPoints;

		#endregion Fields

		//legacy
		public string OwnedPointsAsString { get; set; }

		//public bool IsDelayed { get; set; }

		///Points creating the path; path points
		[JsonIgnore]
		public override ICollection<InkBallPointViewModel> InkBallPoint
		{
			get
			{
				if (!(_inkBallPoint?.Count > 0) && !string.IsNullOrEmpty(PointsAsString))
				{
					_inkBallPoint = StringToPointCollection(PointsAsString,
						Model.InkBallPoint.StatusEnum.POINT_STARTING,
						Model.InkBallPoint.StatusEnum.POINT_IN_PATH,
						this.iPlayerId, EnsureContinuityOfPointsOnPath
					);

					InkBallPointViewModel first = _inkBallPoint.First(), last = _inkBallPoint.Last();
					if (_inkBallPoint.Count <= 3 || first.iX != last.iX || first.iY != last.iY)
						throw new ArgumentException("points count <= 3 or first point != last point");
				}

				return _inkBallPoint;
			}
			set { _inkBallPoint = value; }
		}

		///Oponent points enclosed within this users path
		public ICollection<InkBallPointViewModel> GetOwnedPoints(InkBall.Module.Model.InkBallPoint.StatusEnum ownedStatus, int otherPlayerID)
		{
			if (!(_ownedPoints?.Count > 0) && !string.IsNullOrEmpty(OwnedPointsAsString))
			{
				_ownedPoints = StringToPointCollection(OwnedPointsAsString, ownedStatus, ownedStatus, otherPlayerID);
			}
			return _ownedPoints;
		}

		public void SetOwnedPoints(ICollection<InkBallPointViewModel> ownedPoints)
		{
			_ownedPoints = ownedPoints;
		}

		private ICollection<InkBallPointViewModel> StringToPointCollection(string pointsAsString, InkBallPoint.StatusEnum firstPointStatus,
			InkBallPoint.StatusEnum subsequentStatuses, int playerIDToSet, ActionRef<int, int, int, int> validateContinuityOfThePath = null)
		{
			//basic string allowed char validation
			if (!pointsAsString.All(c => c == ' ' || c == ',' || (c >= '0' && c <= '9')))
				throw new ArgumentException("bad characters in path", nameof(pointsAsString));

			var tokensP = new StringTokenizer(pointsAsString, _spaceSeparatorArr);
			var collection = new HashSet<InkBallPointViewModel>();
			InkBallPointViewModel first = null;
			InkBallPoint.StatusEnum status = firstPointStatus;

			int prev_x = -1, prev_y = -1;

			IEnumerator<StringSegment> enumerator = tokensP.GetEnumerator();
			enumerator.MoveNext();
			var strP = enumerator.Current;

			var tokenXY = strP.Split(_commaSeparatorArr);
			if (tokenXY.Count() >= 2)
			{
				if (int.TryParse(tokenXY.ElementAt(0).Value, out int x) && int.TryParse(tokenXY.ElementAt(1).Value, out int y))
				{
					prev_x = x; prev_y = y;

					first = new InkBallPointViewModel
					{
						//iId = 0,
						iGameId = this.iGameId,
						iPlayerId = playerIDToSet,
						iX = x,
						iY = y,
						Status = status,
						iEnclosingPathId = 0
					};
					collection.Add(first);
					status = subsequentStatuses;
				}
			}

			while (enumerator.MoveNext())
			{
				strP = enumerator.Current;
				tokenXY = strP.Split(_commaSeparatorArr);
				if (tokenXY.Count() >= 2)
				{
					if (int.TryParse(tokenXY.ElementAt(0).Value, out int x) && int.TryParse(tokenXY.ElementAt(1).Value, out int y))
					{
						validateContinuityOfThePath?.Invoke(ref prev_x, ref prev_y, ref x, ref y);

						var point = new InkBallPointViewModel
						{
							//iId = 0,
							iGameId = this.iGameId,
							iPlayerId = playerIDToSet,
							iX = x,
							iY = y,
							Status = status,
							iEnclosingPathId = 0
						};

						if (!collection.Add(point))
						{
							// if (point == first && !((i + 1) < count))
							// {
							// 	var lst = collection.ToList();
							// 	lst.Add(point);
							// 	return lst;
							// }
							// else
							throw new ArgumentException("points in path are not unique");
						}

						status = subsequentStatuses;
					}
				}
			}

			return collection;
		}

		void EnsureContinuityOfPointsOnPath(ref int prevX, ref int prevY, ref int x, ref int y)
		{
			int diff_x = Math.Abs(prevX - x), diff_y = Math.Abs(prevY - y);

			if (diff_x > 1 || diff_y > 1)
				throw new ArgumentOutOfRangeException($"distance[({prevX},{prevY}), ({x},{y})] = ({diff_x},{diff_y}) > (1,1)",
					"points are not stacked one after the other");

			prevX = x; prevY = y;
		}

		public InkBallPathViewModel()
		{ }

		public InkBallPathViewModel(InkBallPath path, string pointsAsString = null, string ownedPointsAsString = null)
		{
			this.iId = path.iId;
			this.iGameId = path.iGameId;
			this.iPlayerId = path.iPlayerId;
			this.PointsAsString = pointsAsString ?? path.PointsAsString;
			this.OwnedPointsAsString = ownedPointsAsString;

			if (path?.InkBallPoint?.Count > 0)
			{
				this.InkBallPoint = path.InkBallPoint.Select(p => new InkBallPointViewModel(p)).ToArray();
			}

			Debug.Assert(this.iId >= 0);
		}

		public InkBallPathViewModel(InkBallPathViewModel path)
		{
			this.iId = path.iId;
			this.iGameId = path.iGameId;
			this.iPlayerId = path.iPlayerId;
			this.PointsAsString = path.PointsAsString;
			this.OwnedPointsAsString = path.OwnedPointsAsString;

			if (path?.InkBallPoint?.Count > 0)
			{
				this.InkBallPoint = path.InkBallPoint;
			}

			Debug.Assert(this.iId >= 0);
		}
	}
}
