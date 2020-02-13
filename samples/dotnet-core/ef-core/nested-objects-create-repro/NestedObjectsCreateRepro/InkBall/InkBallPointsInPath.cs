using System;
using System.Collections.Generic;

namespace InkBall.Module.Model
{
	//TODO: remove coz not needed anymore - points are stored inside InkBallPath.PointsAsString JSON field
	public partial class InkBallPointsInPath
	{
		public int iId { get; set; }
		public int iPathId { get; set; }
		public int iPointId { get; set; }
		public int Order { get; set; }

		public InkBallPath Path { get; set; }
		public InkBallPoint Point { get; set; }

		public InkBallPointsInPath()
		{
		}

		public InkBallPointsInPath(IPoint point)
		{
			this.Point = new InkBallPoint
			{
				iId = point.iId,
				iX = point.iX,
				iY = point.iY
			};
		}
	}
}
