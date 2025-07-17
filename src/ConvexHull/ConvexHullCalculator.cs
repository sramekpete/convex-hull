namespace ConvexHull;

using global::ConvexHull.Algorithms;
using System.Collections.Generic;

/// <summary>
/// Provides convex hull computation using provided algorithm
/// </summary>
public sealed class ConvexHullCalculator(IConvexHullAlgorithm algorithm) {
    /// <summary>
    /// Convex hull algorithm used for calculation
    /// </summary>
    public IConvexHullAlgorithm Algorithm { get; } = algorithm ?? throw new ArgumentNullException(nameof(algorithm));

    /// <summary>
    /// Calculates convex hull from provided collection of points
    /// </summary>
    /// <param name="points">
    /// The collection of points to compute convex hull from
    /// </param>
    /// <returns>
    /// Computed convex hull
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="points"/> is <see langword="null" />
    /// </exception>
    public IEnumerable<Point> Calculate(IEnumerable<Point> points) => points switch {
        null => throw new ArgumentNullException(nameof(points)),
        IEnumerable<Point> enumerable when !enumerable.Any() => [],
        IEnumerable<Point> enumerable when enumerable.Count() == 1 => points,
        _ => Algorithm.Compute(points)
    };
}
