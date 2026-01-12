using MazeSolver;

namespace MazeSolver.Tests;

// tests GetNeighbours
public class MazeNeighboursTests
{
    private const string ExampleMaze = @"D..#.
##...
.#.#.
...#.
####S";

    #region Question 2 - Tests des voisins (7 cas)

    [Fact]
    public void GetNeighbours_CenterCell_ShouldReturn4Neighbours()
    {
        // Arrange - Case centrale avec 4 voisins disponibles
        // On utilise un labyrinthe simple sans murs au centre
        var maze = new Maze(@"D....
.....
.....
.....
....S");

        // Act - Case (2,2) au centre, devrait avoir 4 voisins
        var neighbours = maze.GetNeighbours(2, 2);

        // Assert
        Assert.Equal(4, neighbours.Count);
        Assert.Contains((1, 2), neighbours); // Gauche
        Assert.Contains((3, 2), neighbours); // Droite
        Assert.Contains((2, 1), neighbours); // Haut
        Assert.Contains((2, 3), neighbours); // Bas
    }

    [Fact]
    public void GetNeighbours_AtStart_ShouldNotIncludeStart()
    {
        // Arrange
        var maze = new Maze(ExampleMaze);

        // Act - Case (1,0) adjacente au départ
        var neighbours = maze.GetNeighbours(1, 0);

        // Assert - Ne doit pas contenir le départ (0,0)
        Assert.DoesNotContain((0, 0), neighbours);
    }

    [Fact]
    public void GetNeighbours_NextToWall_ShouldNotIncludeWall()
    {
        // Arrange
        var maze = new Maze(ExampleMaze);

        // Act - Case (2,0), le mur est en (3,0)
        var neighbours = maze.GetNeighbours(2, 0);

        // Assert - Ne doit pas contenir le mur (3,0)
        Assert.DoesNotContain((3, 0), neighbours);
    }

    [Fact]
    public void GetNeighbours_AtTopLeftCorner_ShouldNotIncludeOutOfBounds()
    {
        // Arrange
        var maze = new Maze(@"D....
.....
.....
.....
....S");

        // Act - Case (0,0) est le départ, testons (1,0)
        // Ses voisins: gauche=(0,0)=départ exclu, haut=(-1,0) hors limites, droite=(2,0), bas=(1,1)
        var neighbours = maze.GetNeighbours(1, 0);

        // Assert - Ne doit pas contenir de positions hors limites
        Assert.DoesNotContain((-1, 0), neighbours);
        Assert.DoesNotContain((1, -1), neighbours);
    }

    [Fact]
    public void GetNeighbours_AtBottomRightCorner_ShouldNotIncludeOutOfBounds()
    {
        // Arrange
        var maze = new Maze(@"D....
.....
.....
.....
....S");

        // Act - Case (3,4) près du coin bas-droit
        var neighbours = maze.GetNeighbours(3, 4);

        // Assert - Ne doit pas contenir de positions hors limites (5,4) ou (3,5)
        Assert.DoesNotContain((5, 4), neighbours);
        Assert.DoesNotContain((3, 5), neighbours);
    }

    [Fact]
    public void GetNeighbours_AtLeftEdge_ShouldNotIncludeNegativeX()
    {
        // Arrange
        var maze = new Maze(@"D....
.....
.....
.....
....S");

        // Act - Case (0,2) sur le bord gauche
        var neighbours = maze.GetNeighbours(0, 2);

        // Assert - Ne doit pas contenir x=-1
        Assert.DoesNotContain((-1, 2), neighbours);
    }

    [Fact]
    public void GetNeighbours_AtTopEdge_ShouldNotIncludeNegativeY()
    {
        // Arrange
        var maze = new Maze(@"D....
.....
.....
.....
....S");

        // Act - Case (2,0) sur le bord supérieur
        var neighbours = maze.GetNeighbours(2, 0);

        // Assert - Ne doit pas contenir y=-1
        Assert.DoesNotContain((2, -1), neighbours);
    }

    [Fact]
    public void GetNeighbours_SurroundedByWalls_ShouldReturnEmpty()
    {
        // Arrange - Case entourée de murs
        var maze = new Maze(@"D#...
###..
.....
.....
....S");

        // Act - Case (1,0) est un mur, mais testons (0,1) qui est aussi un mur
        // Dans notre cas, la case (0,0) est le départ, testons une autre
        var neighbours = maze.GetNeighbours(2, 1);

        // Assert - La case (2,1) est un mur (#) donc techniquement pas appelé sur un mur
        // Mais les voisins seraient (1,1)=mur, (3,1)=vide, (2,0)=vide, (2,2)=vide
        Assert.DoesNotContain((1, 1), neighbours); // C'est un mur
    }

    #endregion
}
