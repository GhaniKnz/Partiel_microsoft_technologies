using MazeSolver;

namespace MazeSolver.Tests;

// tests pour le parsing du labirynthe
public class MazeParsingTests
{
    // Labyrinthe exemple de l'énoncé
    private const string ExampleMaze = @"D..#.
##...
.#.#.
...#.
####S";

    // Deuxième labyrinthe de l'énoncé (distance 18)
    private const string SecondMaze = @".#.......
D#.#####.
.#.#...#.
......#.#
###.#..#.
.##.#.##.
..#.#..#.
#.#.##.#.
....#S.#.";

    #region Question 1.a - Test du constructeur (Start, Exit, Grid)

    [Fact]
    public void Constructor_WithExampleMaze_ShouldSetStartCorrectly()
    {
        // Arrange & Act
        var maze = new Maze(ExampleMaze);

        // Assert - Le départ 'D' est en position (0,0)
        Assert.Equal((0, 0), maze.Start);
    }

    [Fact]
    public void Constructor_WithExampleMaze_ShouldSetExitCorrectly()
    {
        // Arrange & Act
        var maze = new Maze(ExampleMaze);

        // Assert - La sortie 'S' est en position (4,4)
        Assert.Equal((4, 4), maze.Exit);
    }

    [Fact]
    public void Constructor_WithExampleMaze_ShouldParseWallsCorrectly()
    {
        // Arrange & Act
        var maze = new Maze(ExampleMaze);

        // Assert - Vérifier quelques murs (#)
        // Position (3,0) est un mur
        Assert.False(maze.Grid[0][3]);
        // Position (0,1) est un mur
        Assert.False(maze.Grid[1][0]);
        // Position (1,1) est un mur
        Assert.False(maze.Grid[1][1]);
    }

    [Fact]
    public void Constructor_WithExampleMaze_ShouldParseEmptySpacesCorrectly()
    {
        // Arrange & Act
        var maze = new Maze(ExampleMaze);

        // Assert - Vérifier quelques cases vides (.)
        // Position (1,0) est vide
        Assert.True(maze.Grid[0][1]);
        // Position (2,0) est vide
        Assert.True(maze.Grid[0][2]);
        // Position (2,1) est vide
        Assert.True(maze.Grid[1][2]);
    }

    [Fact]
    public void Constructor_WithExampleMaze_StartPositionShouldBePassable()
    {
        // Arrange & Act
        var maze = new Maze(ExampleMaze);

        // Assert - Le départ doit être une case passable
        Assert.True(maze.Grid[0][0]);
    }

    [Fact]
    public void Constructor_WithExampleMaze_ExitPositionShouldBePassable()
    {
        // Arrange & Act
        var maze = new Maze(ExampleMaze);

        // Assert - La sortie doit être une case passable
        Assert.True(maze.Grid[4][4]);
    }

    [Fact]
    public void Constructor_WithSecondMaze_ShouldSetStartCorrectly()
    {
        // Arrange & Act
        var maze = new Maze(SecondMaze);

        // Assert - Le départ 'D' est en position (0,1) dans le second labyrinthe
        Assert.Equal((0, 1), maze.Start);
    }

    [Fact]
    public void Constructor_WithSecondMaze_ShouldSetExitCorrectly()
    {
        // Arrange & Act
        var maze = new Maze(SecondMaze);

        // Assert - La sortie 'S' est en position (5,8)
        Assert.Equal((5, 8), maze.Exit);
    }

    #endregion

    #region Question 1.b - Test du tableau des distances

    [Fact]
    public void Constructor_WithExampleMaze_DistancesSizeShouldMatchGridSize()
    {
        // Arrange & Act
        var maze = new Maze(ExampleMaze);

        // Assert - Le tableau des distances doit avoir la même taille que la grille
        Assert.Equal(maze.Grid.Length, maze.Distances.Length);
        for (int i = 0; i < maze.Grid.Length; i++)
        {
            Assert.Equal(maze.Grid[i].Length, maze.Distances[i].Length);
        }
    }

    [Fact]
    public void Constructor_WithExampleMaze_AllDistancesShouldBeZero()
    {
        // Arrange & Act
        var maze = new Maze(ExampleMaze);

        // Assert - Toutes les distances doivent être initialisées à 0
        foreach (var row in maze.Distances)
        {
            foreach (var distance in row)
            {
                Assert.Equal(0, distance);
            }
        }
    }

    [Fact]
    public void Constructor_WithSecondMaze_DistancesSizeShouldMatchGridSize()
    {
        // Arrange & Act
        var maze = new Maze(SecondMaze);

        // Assert
        Assert.Equal(9, maze.Distances.Length); // 9 lignes
        Assert.Equal(9, maze.Distances[0].Length); // 9 colonnes
    }

    [Fact]
    public void Constructor_WithExampleMaze_GridShouldHaveCorrectDimensions()
    {
        // Arrange & Act
        var maze = new Maze(ExampleMaze);

        // Assert - 5 lignes x 5 colonnes
        Assert.Equal(5, maze.Grid.Length);
        Assert.Equal(5, maze.Grid[0].Length);
    }

    #endregion
}
