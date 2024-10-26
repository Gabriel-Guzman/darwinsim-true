using System;

namespace UnitySharpNEAT.SharpNEAT.Network.Domains
{
    /// <summary>
    /// Defines a 2D point in the polar coordinate space.
    /// </summary>
    public struct PolarPoint
    {
        /// <summary>
        /// Radial coordinate.
        /// </summary>
        private readonly double _r;
        /// <summary>
        /// Angular coordinate (theta).
        /// </summary>
        private readonly double _t;

        #region Constructor

        /// <summary>
        /// Construct with provided coordinate values.
        /// </summary>
        /// <param name="r">Radial coordinate (distance between points).</param>
        /// <param name="t">Angular coordinate (theta).</param>
        public PolarPoint(double r, double t)
        {
            _r = r;
            _t = t;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Radial coordinate.
        /// </summary>
        public double Radial
        {
            get { return _r; }
        }

        /// <summary>
        /// Angular coordinate (theta).
        /// </summary>
        public double Theta
        {
            get { return _t; }
        }

        #endregion

        #region Static Factory Methods

        /// <summary>
        /// Create a point in the polar coordinate system from the provided cartesian coordinates.
        /// </summary>
        public static PolarPoint FromCartesian(IntPoint p)
        {
            var r = Math.Sqrt((p._x * p._x) + (p._y * p._y));
            var t = Math.Atan2(p._x, p._y);
            if(t<0.0) {
                t += 2*Math.PI;
            }
            return new PolarPoint(r, t);
        }

        #endregion
    }
}
