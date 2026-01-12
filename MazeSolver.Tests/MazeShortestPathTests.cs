using MazeSolver;

namespace MazeSolver.Tests;

// tests chemin le plus court
public class MazeShortestPathTests
{
    #region Question 5 - Tests du chemin le plus court

    [Fact]
    public void GetShortestPath_ExampleMaze_ShouldReturnPathOfLength9()
    {
        // Arrange
        var maze = new Maze(@"D..#.
##...
.#.#.
...#.
####S");

        // Act
        var path = maze.GetShortestPath();

        // Assert - Le chemin inclut le départ et la sortie, donc 9 éléments (8 + 1)
        Assert.Equal(8, path.Count);
    }

    [Fact]
    public void GetShortestPath_ExampleMaze_ShouldStartWithStart()
    {
        // Arrange
        var maze = new Maze(@"D..#.
##...
.#.#.
...#.
####S");

        // Act
        var path = maze.GetShortestPath();

        // Assert - Le chemin doit commencer par le départ
        Assert.Equal((0, 0), path[0]);
    }

    [Fact]
    public void GetShortestPath_ExampleMaze_ShouldEndWithExit()
    {
        // Arrange
        var maze = new Maze(@"D..#.
##...
.#.#.
...#.
####S");

        // Act
        var path = maze.GetShortestPath();

        // Assert - Le chemin doit finir par la sortie
        Assert.Equal((4, 4), path[path.Count - 1]);
    }

    [Fact]
    public void GetShortestPath_SecondMaze_ShouldReturnPathOfLength15()
    {
        // Arrange
        var maze = new Maze(@".#.......
D#.#####.
.#.#...#.
......#.#
###.#..#.
.##.#.##.
..#.#..#.
#.#.##.#.
....#S.#.");

        // Act
        var path = maze.GetShortestPath();

        // Assert - Distance 14 + le point de départ = 15 éléments
        Assert.Equal(15, path.Count);
    }

    [Fact]
    public void GetShortestPath_SecondMaze_ShouldStartWithStart()
    {
        // Arrange
        var maze = new Maze(@".#.......
D#.#####.
.#.#...#.
......#.#
###.#..#.
.##.#.##.
..#.#..#.
#.#.##.#.
....#S.#.");

        // Act
        var path = maze.GetShortestPath();

        // Assert - Départ en (0,1)
        Assert.Equal((0, 1), path[0]);
    }

    [Fact]
    public void GetShortestPath_SecondMaze_ShouldEndWithExit()
    {
        // Arrange
        var maze = new Maze(@".#.......
D#.#####.
.#.#...#.
......#.#
###.#..#.
.##.#.##.
..#.#..#.
#.#.##.#.
....#S.#.");

        // Act
        var path = maze.GetShortestPath();

        // Assert - Sortie en (5,8)
        Assert.Equal((5, 8), path[path.Count - 1]);
    }

    [Fact]
    public void GetShortestPath_SimpleMaze_ShouldReturnCorrectPath()
    {
        // Arrange
        var maze = new Maze("D.S");

        // Act
        var path = maze.GetShortestPath();

        // Assert
        Assert.Equal(3, path.Count);
        Assert.Equal((0, 0), path[0]); // Départ
        Assert.Equal((1, 0), path[1]); // Milieu
        Assert.Equal((2, 0), path[2]); // Sortie
    }

    [Fact]
    public void GetShortestPath_ConsecutiveCellsShouldBeAdjacent()
    {
        // Arrange
        var maze = new Maze(@"D..#.
##...
.#.#.
...#.
####S");

        // Act
        var path = maze.GetShortestPath();

        // Assert - Chaque case doit être adjacente à la suivante
        for (int i = 0; i < path.Count - 1; i++)
        {
            var current = path[i];
            var next = path[i + 1];
            var dx = Math.Abs(current.Item1 - next.Item1);
            var dy = Math.Abs(current.Item2 - next.Item2);
            
            // Doit être adjacent (distance de Manhattan = 1)
            Assert.Equal(1, dx + dy);
        }
    }

    #endregion
}
