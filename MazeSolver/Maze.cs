namespace MazeSolver;

// Classe qui represente un labyrinthe et permet de le resoudre avec BFS
public class Maze
{
    // Tableau des distances par rapport au depart
    public int[][] Distances { get; init; }

    // Grille du labyrinthe (true = passage, false = mur)
    public bool[][] Grid { get; init; }

    // File pour le parcours en largeur (BFS)
    public Queue<(int x, int y, int distance)> ToVisit { get; init; }

    // Position de depart
    public (int x, int y) Start { get; init; }

    // Position de sortie
    public (int x, int y) Exit { get; init; }

    // Constructeur qui parse le labyrinthe depuis une chaine
    public Maze(string maze)
    {
        // on split les lignes
        var lines = maze.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int height = lines.Length;
        int width = lines.Max(l => l.Length);

        // Initialiser la grille et les distances
        Grid = new bool[height][];
        Distances = new int[height][];

        for (int y = 0; y < height; y++)
        {
            Grid[y] = new bool[width];
            Distances[y] = new int[width];
            
            for (int x = 0; x < width; x++)
            {
                char cell = x < lines[y].Length ? lines[y][x] : '#';
                
                switch (cell)
                {
                    case 'D':
                        Start = (x, y);
                        Grid[y][x] = true;
                        break;
                    case 'S':
                        Exit = (x, y);
                        Grid[y][x] = true;
                        break;
                    case '.':
                        Grid[y][x] = true;
                        break;
                    case '#':
                    default:
                        Grid[y][x] = false;
                        break;
                }
                
                Distances[y][x] = 0;
            }
        }

        // Initialiser la file avec le point de départ
        ToVisit = new Queue<(int x, int y, int distance)>();
        ToVisit.Enqueue((Start.x, Start.y, 0));
    }

    // retourne les voisins d'une case
    public IList<(int, int)> GetNeighbours(int x, int y)
    {
        var neighbours = new List<(int, int)>();
        
        // les 4 directions possible
        var directions = new (int dx, int dy)[] 
        { 
            (0, -1),  // Haut
            (0, 1),   // Bas
            (-1, 0),  // Gauche
            (1, 0)    // Droite
        };

        foreach (var (dx, dy) in directions)
        {
            int nx = x + dx;
            int ny = y + dy;

            // Vérifier si dans les limites
            if (nx < 0 || ny < 0 || ny >= Grid.Length || nx >= Grid[0].Length)
                continue;

            // Vérifier si c'est le départ (exclu)
            if (nx == Start.x && ny == Start.y)
                continue;

            // Vérifier si c'est un mur
            if (!Grid[ny][nx])
                continue;

            neighbours.Add((nx, ny));
        }

        return neighbours;
    }

    // fonction de remplissage, retourn true si on est arrivé
    public bool Fill()
    {
        if (ToVisit.Count == 0)
            return false;

        var (x, y, distance) = ToVisit.Dequeue();

        // Si c'est la sortie, on associe la distance et on retourne true
        if (x == Exit.x && y == Exit.y)
        {
            Distances[y][x] = distance;
            return true;
        }

        // Si cette case a déjà une distance (et ce n'est pas le départ), on ignore
        if (Distances[y][x] != 0 && !(x == Start.x && y == Start.y))
        {
            return false;
        }

        // Si c'est le départ, on le marque avec 0 (déjà fait dans le constructeur)
        // Pour les autres cases, on met à jour la distance
        if (!(x == Start.x && y == Start.y))
        {
            Distances[y][x] = distance;
        }

        // Ajouter les voisins à la file avec distance + 1
        var neighbours = GetNeighbours(x, y);
        foreach (var (nx, ny) in neighbours)
        {
            // Ne pas ajouter si déjà visité
            if (Distances[ny][nx] == 0 || (nx == Exit.x && ny == Exit.y))
            {
                ToVisit.Enqueue((nx, ny, distance + 1));
            }
        }

        return false;
    }

    // calcul la distance entre depart et sortie
    public int GetDistance()
    {
        // boucle jusqua la sortie
        while (!Fill())
        {
            // Continue jusqu'à atteindre la sortie
            if (ToVisit.Count == 0)
                return -1; // Pas de chemin
        }

        return Distances[Exit.y][Exit.x];
    }

    // trouve le chemin le plus court
    public IList<(int, int)> GetShortestPath()
    {
        // on recalcule tout
        var tempQueue = new Queue<(int x, int y, int distance)>();
        tempQueue.Enqueue((Start.x, Start.y, 0));
        
        // Reset distances
        for (int y = 0; y < Distances.Length; y++)
        {
            for (int x = 0; x < Distances[y].Length; x++)
            {
                Distances[y][x] = 0;
            }
        }

        // Vider et reremplir ToVisit
        ToVisit.Clear();
        ToVisit.Enqueue((Start.x, Start.y, 0));

        // Remplir tout le labyrinthe
        while (ToVisit.Count > 0)
        {
            if (Fill())
                break;
        }

        // Maintenant on reconstruit le chemin depuis la sortie
        var path = new List<(int, int)>();
        var current = Exit;
        path.Add(current);

        while (current != Start)
        {
            int currentDistance = Distances[current.y][current.x];
            
            // Chercher le voisin avec distance - 1
            var directions = new (int dx, int dy)[] 
            { 
                (0, -1), (0, 1), (-1, 0), (1, 0)
            };

            foreach (var (dx, dy) in directions)
            {
                int nx = current.x + dx;
                int ny = current.y + dy;

                // Vérifier les limites
                if (nx < 0 || ny < 0 || ny >= Grid.Length || nx >= Grid[0].Length)
                    continue;

                // Si c'est le départ et on est à distance 1
                if (nx == Start.x && ny == Start.y && currentDistance == 1)
                {
                    path.Add(Start);
                    current = Start;
                    break;
                }

                // Si la distance est exactement currentDistance - 1
                if (Distances[ny][nx] == currentDistance - 1 && Grid[ny][nx])
                {
                    current = (nx, ny);
                    path.Add(current);
                    break;
                }
            }
        }

        // Inverser le chemin pour avoir du départ vers la sortie
        path.Reverse();
        return path;
    }
}
