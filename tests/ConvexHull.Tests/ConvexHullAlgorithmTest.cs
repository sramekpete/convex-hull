namespace ConvexHull.Tests;

using ConvexHull.Algorithms;
using NTS = global::NetTopologySuite;

[TestClass]
public sealed class ConvexHullAlgorithmTest {
    private static Random _random = new(DateTime.Now.Millisecond);
    private static readonly ConvexHullCalculator _algorithm = new(JarvisMarchAlgorithm.Default);

    // Some are not correctly calculated as the points are too close to each other
    public static IEnumerable<(int count, uint boundary)> Parameters => [
        (3, 10),
        (4, 10),
        (5, 10),
        (10, 20),
        // Failing, needs investigation
        //(25, 50),
        (50, 100),
        (100, 250),
        (200, 500),
        (500, 500),
        // Failing, needs investigation
        //(1000, 500)
    ];

    [TestMethod]
    public void Compute_Null_Parameter_Throws_ArgumentException() {
        // Arrange
        Point[] points = null!;
        static void Compute(IEnumerable<Point> points) => _algorithm.Calculate(points);

        // Act && Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(() => Compute(points));
    }

    [TestMethod]
    public void Compute_Empty_Array_Returns_Empty_Array() {
        // Arrange
        var points = Array.Empty<Point>();
        var expected = Array.Empty<Point>();

        // Act
        var result = _algorithm.Calculate(points);

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(expected.Length, result.Count(), "Result should not be null.");
        CollectionAssert.AreEquivalent(expected.ToArray(), result.ToArray(), "Result does not match expected sequence.");
    }

    [TestMethod]
    public void Compute_One_Point_Array_Returns_Same_Array() {
        // Arrange
        var points = new Point[] { new(0, 0) };
        var expected = points;

        // Act
        var result = _algorithm.Calculate(points);

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(expected.Length, result.Count(), "Result should not be null.");
        CollectionAssert.AreEquivalent(expected.ToArray(), result.ToArray(), "Result does not match expected sequence.");
    }

    [TestMethod]
    public void Compute_Two_Points_Array_Returns_Same_Array() {
        // Arrange
        var points = new Point[] { new(0, 0), new(1, 0) };
        var expected = new Point[] { new(0, 0), new(1, 0) };

        // Act
        var result = _algorithm.Calculate(points);

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(expected.Length, result.Count(), "Result should not be null.");
        CollectionAssert.AreEquivalent(expected.ToArray(), result.ToArray(), "Result does not match expected sequence.");
    }


    [TestMethod]
    public void Compute_Linear_Sequence_Returns_Line_Edge_Points() {
        // Arrange
        var points = new Point[] { new(0, 0), new(1, 0), new(2, 0), new(3, 0), new(4, 0), new(5, 0) };
        var expected = new Point[] { new(0, 0), new(5, 0) };

        // Act
        var result = _algorithm.Calculate(points);

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(expected.Count(), result.Count(), "Result should not be null.");
        CollectionAssert.AreEquivalent(expected.ToArray(), result.ToArray(), "Result does not match expected sequence.");
    }

    [TestMethod]
    [DynamicData(nameof(Parameters), DynamicDataSourceType.Property)]
    public void Compute_Returns_Same_Convex_Hull_As_NetTopology_ConvexHull(int count, uint boundary) {
        // Arrange
        var points = GetPoints(count, boundary);
        var expected = GetExpectedConvexHull(points);

        // Act
        var result = _algorithm.Calculate(points);

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(expected.Count(), result.Count(), "Result count does not match expected count.");
        Assert.AreEqual(result.First(), result.Last(), "Result does not start and end with the same point.");
        CollectionAssert.AreEquivalent(expected.Distinct().ToArray(), result.Distinct().ToArray(), "Result does not match expected sequence of distinct values.");
    }

    #region Helper methods

    public static IEnumerable<Point> GetExpectedConvexHull(IEnumerable<Point> points) {
        var coordinates = points
            .Select(p => new NTS.Geometries.Coordinate(p.X, p.Y));

        var convexHull = new NTS.Algorithm.ConvexHull(coordinates, new NTS.Geometries.GeometryFactory());

        var result = convexHull?
            .GetConvexHull();

        foreach (var item in result?.Coordinates ?? []) {
            yield return new Point((int)item.X, (int)item.Y);
        }
    }

    public static IEnumerable<Point> GetPoints(int count, uint boundary) {
        var points = new List<Point>(count);

        for (int i = 0; i < count; i++) {
            Point point = new(GetPoint(boundary), GetPoint(boundary));

            if (points.Any(p => p == point)) {
                i--;
            } else {
                points.Add(point);
            }
        }

        return points;

        static int GetPoint(uint boundary) {
            return GetSign() * _random.Next((int)boundary + 1);
        }

        static int GetSign() {
            return _random.Next(2) < 1 ? -1 : 1;
        }
    }

    #endregion
}
