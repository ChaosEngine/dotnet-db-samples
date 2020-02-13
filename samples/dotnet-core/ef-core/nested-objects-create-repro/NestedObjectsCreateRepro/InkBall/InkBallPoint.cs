using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace InkBall.Module.Model
{
	public interface IPoint
	{
		int iId { get; set; }

		int iGameId { get; set; }

		int iPlayerId { get; set; }

		int iX { get; set; }

		int iY { get; set; }

		InkBallPoint.StatusEnum Status { get; set; }

		int? iEnclosingPathId { get; set; }
	}

	public abstract class CommonPoint : IPoint, IDtoMsg,
		IEquatable<IPoint>, IEqualityComparer<IPoint>, IComparable
	{
		public int iId { get; set; }

		public int iGameId { get; set; }

		public int iPlayerId { get; set; }

		public int iX { get; set; }

		public int iY { get; set; }

		public InkBallPoint.StatusEnum Status { get; set; }

		public int? iEnclosingPathId { get; set; }

		#region Overrides

		// override object.ToString
		public override string ToString()
		{
			return $"{this.iX},{this.iY}";
		}

		// override object.Equals
		public override bool Equals(object obj)
		{
			if (!(obj is IPoint)) return false;

			IPoint o = (IPoint)obj;

			return this.iPlayerId == o.iPlayerId
				//&& this.iGameId == o.iGameId
				&& this.Status == o.Status
				//&& this.iEnclosingPathId == o.iEnclosingPathId
				&& this.iX == o.iX & this.iY == o.iY;
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return this.iX ^ this.iY;
		}

		//interface IEquatable
		public bool Equals(IPoint o)
		{
			return ((object)o) != null
				//&& this.iGameId == o.iGameId
				&& this.iPlayerId == o.iPlayerId
				&& this.Status == o.Status
				//&& this.iEnclosingPathId == o.iEnclosingPathId
				&& this.iX == o.iX && this.iY == o.iY;
		}

		//interface IEqualityComparer
		public bool Equals(IPoint left, IPoint right)
		{
			if (right == null && left == null)
				return true;
			else if (left == null || right == null)
				return false;

			return left.Equals(right);
		}

		//interface IEqualityComparer
		public int GetHashCode(IPoint obj)
		{
			return obj.GetHashCode();
		}

		//interface IComparable
		public int CompareTo(object obj)
		{
			if (obj == null) return 1;

			CommonPoint other_point = obj as CommonPoint;
			if (other_point != null)
			{
				int this_val = ((this.iY << 7) + this.iX);
				int other_val = ((other_point.iY << 7) + other_point.iX);

				return this_val.CompareTo(other_val);
			}
			else
				throw new ArgumentNullException(nameof(other_point), $"Object is not a {nameof(CommonPoint)}");
		}

		public void OnBeforeSerialize()
		{
			iId = Math.Max(0, iId);
		}

		public void OnAfterDeserialize()
		{
		}

		public CommandKindEnum GetKind()
		{
			return CommandKindEnum.POINT;
		}

		#endregion Overrides

		public static bool operator ==(CommonPoint left, CommonPoint right)
		{
			if (((object)left) == null || ((object)right) == null)
				return Object.Equals(left, right);
			return left.Equals(right);
		}

		public static bool operator !=(CommonPoint left, CommonPoint right)
		{
			if (((object)left) == null || ((object)right) == null)
				return !Object.Equals(left, right);
			return !(left.Equals(right));
		}

		public static string GetPointsAsJavaScriptArrayForPage(IEnumerable<InkBallPoint> points)
		{
			StringBuilder builder = new StringBuilder("[", 300);

			string comma = string.Empty;
			foreach (var p in points)
			{
#if DEBUG
				builder.AppendFormat("{4}[/*id={5}*/{0}/*x*/,{1}/*y*/,{2}/*val*/,{3}/*playerID*/]", p.iX, p.iY, (int)p.Status, p.iPlayerId, comma, p.iId);
#else
				builder.AppendFormat("{4}[{0},{1},{2},{3}]", p.iX, p.iY, (int)p.Status, p.iPlayerId, comma);
#endif
				comma = ",\r";
			}
			builder.Append(']');

			return builder.ToString();
		}

		public static string GetPointsAsJavaScriptArrayForSignalR(IEnumerable<CommonPoint> points)
		{
			StringBuilder builder = new StringBuilder("[", 300);
			string comma = string.Empty;
			foreach (var p in points)
			{
				builder.AppendFormat("{4}[{0},{1},{2},{3}]", p.iX, p.iY, (int)p.Status, p.iPlayerId, comma);
				comma = ",";
			}
			builder.Append(']');

			return builder.ToString();
		}

		public bool ShouldSerializeiId()
		{
			// don't serialize the iId property if <= 0
			return (iId > 0);
		}
	}

	public partial class InkBallPoint : CommonPoint, IPoint
	{
		public enum StatusEnum
		{
			POINT_FREE_RED = -3,
			POINT_FREE_BLUE = -2,
			POINT_FREE = -1,
			POINT_STARTING = 0,
			POINT_IN_PATH = 1,
			POINT_OWNED_BY_RED = 2,
			POINT_OWNED_BY_BLUE = 3
		}

		public InkBallPath EnclosingPath { get; set; }

		public InkBallGame Game { get; set; }

		public InkBallPlayer Player { get; set; }

		public ICollection<InkBallPointsInPath> InkBallPointsInPath { get; set; }

		public InkBallPoint()
		{
			// InkBallPointsInPath = new HashSet<InkBallPointsInPath>();
		}
	}

	//[Serializable]
	public class InkBallPointViewModel : CommonPoint, IPoint
	{
		public InkBallPointViewModel()
		{ }

		public InkBallPointViewModel(InkBallPoint point)
		{
			this.iId = point.iId;
			this.iGameId = point.iGameId;
			this.iPlayerId = point.iPlayerId;
			this.iX = point.iX;
			this.iY = point.iY;
			this.Status = point.Status;
			this.iEnclosingPathId = point.iEnclosingPathId;

			Debug.Assert(this.iId >= 0);
		}

		//[JsonConstructor]
		public InkBallPointViewModel(InkBallPointViewModel point)
		{
			this.iId = point.iId;
			this.iGameId = point.iGameId;
			this.iPlayerId = point.iPlayerId;
			this.iX = point.iX;
			this.iY = point.iY;
			this.Status = point.Status;
			this.iEnclosingPathId = point.iEnclosingPathId;

			Debug.Assert(this.iId >= 0);
		}
	}

	#region Helpers

	sealed class SimpleCoordsPointComparer : IEqualityComparer<IPoint>
	{
		//interface IEqualityComparer
		public bool Equals(IPoint left, IPoint right)
		{
			if (right == null && left == null)
				return true;
			else if (left == null || right == null)
				return false;

			return left.iPlayerId == right.iPlayerId
				//&& left.iGameId == right.iGameId
				//&& left.Status == right.Status
				//&& this.iEnclosingPathId == o.iEnclosingPathId
				&& left.iX == right.iX && left.iY == right.iY;
		}

		//interface IEqualityComparer
		public int GetHashCode(IPoint obj)
		{
			return obj.GetHashCode();
		}
	}

	#endregion Helpers
}
