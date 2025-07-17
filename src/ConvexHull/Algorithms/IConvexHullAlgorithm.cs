namespace ConvexHull.Algorithms;

using System.Collections.Generic;

/// <summary>
/// Defines method for convex hull computation
/// </summary>
public interface IConvexHullAlgorithm {
    /// <summary>
    /// Computes convex hull from provided collection of points
    /// </summary>
    /// <param name="points">
    /// The collection of points to compute convex hull from
    /// </param>
    /// <returns>
    /// Computed convex hull
    /// </returns>
    IEnumerable<Point> Compute(IEnumerable<Point> points);
}
