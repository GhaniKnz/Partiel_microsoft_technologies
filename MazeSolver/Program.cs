using MazeSolver;

// programme principal pour tester le maze solver
Console.WriteLine("=== Résolveur de Labyrinthe ===");
Console.WriteLine();

// premier labyrinthe de l'ennoncé
string maze1 = @"D..#.
##...
.#.#.
...#.
####S";

Console.WriteLine("Labyrinthe 1:");
Console.WriteLine(maze1);
Console.WriteLine();

var solver1 = new Maze(maze1);
int distance1 = solver1.GetDistance();
Console.WriteLine($"Distance minimale: {distance1}");
//Console.WriteLine("debug: " + solver1.Distances.Length); // a enlever apres

// on recrée pour le chemin car GetDistance utilise la queue
solver1 = new Maze(maze1);
var path1 = solver1.GetShortestPath();
Console.WriteLine($"Chemin le plus court ({path1.Count} cases):");
Console.WriteLine(string.Join(" -> ", path1.Select(p => $"({p.Item1},{p.Item2})")));
Console.WriteLine();

// 2eme labyrinthe plus compliqué
string maze2 = @".#.......
D#.#####.
.#.#...#.
......#.#
###.#..#.
.##.#.##.
..#.#..#.
#.#.##.#.
....#S.#.";

Console.WriteLine("Labyrinthe 2:");
Console.WriteLine(maze2);
Console.WriteLine();

var solver2 = new Maze(maze2);
int distance2 = solver2.GetDistance();
Console.WriteLine($"Distance minimale: {distance2}");

solver2 = new Maze(maze2);
var path2 = solver2.GetShortestPath();
Console.WriteLine($"Chemin le plus court ({path2.Count} cases):");
Console.WriteLine(string.Join(" -> ", path2.Select(p => $"({p.Item1},{p.Item2})")));
Console.WriteLine();

Console.WriteLine("=== Fin ===");

