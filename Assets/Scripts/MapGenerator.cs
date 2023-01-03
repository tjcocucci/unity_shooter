using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Map[] maps;

    public int mapIndex;
    
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navMeshObstaclePrefab;
    public Transform navMeshGround;
    public Queue<Coord> shuffledEmptyTiles;

    Map currentMap;
    Transform[,] tileMap;

    void Start() {
        GenerateMap();
    }

    public void GenerateMap() {
        currentMap = maps[mapIndex];
        tileMap = new Transform[currentMap.sizeX, currentMap.sizeY];

        System.Random prng = new System.Random(currentMap.seed);

        string holderName =  "mapHolder";

        if (transform.Find(holderName)) {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        List<Coord> coordList = new List<Coord>();
        for (int i=0; i<currentMap.sizeX; i++) {
            for (int j=0; j<currentMap.sizeY; j++) {
                coordList.Add(new Coord(i, j));
            }
        }
        Queue<Coord> shuffledCoords = new Queue<Coord>(Utility.ShuffleArray<Coord>(coordList.ToArray(), currentMap.seed));

        for (int i=0; i<coordList.Count; i++) {
            Coord coord = coordList[i];
            Vector3 tilePosition = CoordToPosition(coord);
            Transform tile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(90, 0, 0));
            tile.localScale = new Vector3(currentMap.tileSize, currentMap.tileSize, 1);
            tile.localScale = tile.localScale * (1 - currentMap.marginPercent);
            tile.name = "Tile (" + coord.x + ", " + coord.y + ")";
            tile.parent = mapHolder;

            tileMap[coord.x, coord.y] = tile;
        }

        int numberOfObstacles = (int) (currentMap.sizeX * currentMap.sizeY * currentMap.obstacleDensity);
        bool[,] occupiedTiles = new bool[currentMap.sizeX, currentMap.sizeY];
        int placedObstacles = 0;

        for (int i=0; i<currentMap.sizeX; i++) {
            for (int j=0; j<currentMap.sizeY; j++) {
                occupiedTiles[i, j] = false;
            }
        }

        List<Coord> emptyCoords = new List<Coord>(coordList);

        occupiedTiles[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;
        while (shuffledCoords.Count > 0 && placedObstacles < numberOfObstacles) {
            Coord coord = shuffledCoords.Dequeue();
            placedObstacles++;
            occupiedTiles[coord.x, coord.y] = true;

            if (MapIsFullyAccesible(occupiedTiles, currentMap.sizeX * currentMap.sizeY - placedObstacles)) {
                Transform obstacle = Instantiate(obstaclePrefab, CoordToPosition(coord), Quaternion.identity);
                obstacle.localScale = new Vector3(currentMap.tileSize, 0, currentMap.tileSize) * (1 - currentMap.marginPercent);
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float) prng.NextDouble());
                obstacle.localScale += Vector3.up * obstacleHeight / 2;
                obstacle.position += Vector3.up * (obstacleHeight / 4);
                obstacle.name = "Obstacle" + placedObstacles;
                obstacle.parent = mapHolder;

                Renderer obstacleRenderer = obstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPercent = coord.y / (float) currentMap.sizeY;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                emptyCoords.Remove(coord);
            } else {
                placedObstacles--;
                occupiedTiles[coord.x, coord.y] = false;
            }
        }

        shuffledEmptyTiles = new Queue<Coord>(Utility.ShuffleArray<Coord>(emptyCoords.ToArray(), currentMap.seed));

        navMeshGround.position = Vector3.zero - 0.01f * Vector3.up;
        navMeshGround.localScale = new Vector3(currentMap.mapSizeX, currentMap.mapSizeY, 1);

        Vector3 positionLeft = new Vector3(-(currentMap.mapSizeX / 2f  + currentMap.tileSize / 2f), 0, 0);
        Transform leftMargin = Instantiate(navMeshObstaclePrefab, positionLeft, Quaternion.identity);
        leftMargin.localScale = new Vector3(2, 2, currentMap.mapSizeY);
        leftMargin.parent = mapHolder;

        Vector3 positionRight = new Vector3(currentMap.mapSizeX / 2f  + currentMap.tileSize / 2f, 0, 0);
        Transform rightMargin = Instantiate(navMeshObstaclePrefab, positionRight, Quaternion.identity);
        rightMargin.localScale = new Vector3(2, 2, currentMap.mapSizeY);
        rightMargin.parent = mapHolder;

        Vector3 positionTop = new Vector3(0, 0, currentMap.mapSizeY / 2f  + currentMap.tileSize / 2f);
        Transform topMargin = Instantiate(navMeshObstaclePrefab, positionTop, Quaternion.identity);
        topMargin.localScale = new Vector3(currentMap.mapSizeX, 2, 2);
        topMargin.parent = mapHolder;

        Vector3 positionBottom = new Vector3(0, 0, -(currentMap.mapSizeY / 2f  + currentMap.tileSize / 2f));
        Transform bottomMargin = Instantiate(navMeshObstaclePrefab, positionBottom, Quaternion.identity);
        bottomMargin.localScale = new Vector3(currentMap.mapSizeX, 2, 2);
        bottomMargin.parent = mapHolder;

    }

    bool MapIsFullyAccesible(bool[,] occupiedTiles, int targetTiles) {
        bool[,] checkedTiles = new bool[currentMap.sizeX, currentMap.sizeY];
        for (int i=0; i<currentMap.sizeX; i++) {
            for (int j=0; j<currentMap.sizeY; j++) {
                checkedTiles[i, j] = false;
            }
        }
        // occupiedTiles.CopyTo(checkedTiles, currentMap.sizeX * currentMap.sizeY);
        Queue<Coord> queue = new Queue<Coord>();
        int reacheableTiles = 0;

        queue.Enqueue(currentMap.mapCenter);
        checkedTiles[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

        reacheableTiles++;

        Coord[] neighbours = {new Coord(0, -1), new Coord(-1, 0), new Coord(0, 1), new Coord(1, 0)};
        while (queue.Count > 0) {
            Coord coord = queue.Dequeue();
            foreach (Coord n in neighbours) {
                Coord neighbourCoord = new Coord(coord.x + n.x, coord.y + n.y);
                // if neighbour in map
                if (neighbourCoord.x >=0 && neighbourCoord.x < currentMap.sizeX && neighbourCoord.y >=0 && neighbourCoord.y < currentMap.sizeY) {
                    // if neighbour not already checked
                    if (!checkedTiles[neighbourCoord.x, neighbourCoord.y]) {
                        // if tile has no obstacle
                        if (!occupiedTiles[neighbourCoord.x, neighbourCoord.y]) {
                            queue.Enqueue(neighbourCoord);
                            checkedTiles[neighbourCoord.x, neighbourCoord.y] = true;
                            reacheableTiles++;
                        }
                    }
                }
            }
        }
        return reacheableTiles == targetTiles;
    }

    public Vector3 CoordToPosition(Coord coord) {
        float xCoord = coord.x * currentMap.tileSize - (currentMap.mapSizeX - currentMap.tileSize) / 2f;
        float yCoord = coord.y * currentMap.tileSize - (currentMap.mapSizeY - currentMap.tileSize) / 2f;
        return new Vector3(xCoord, 0, yCoord);
    }

    Coord GetRandomCoord(Queue<Coord> coordQueue) {
        Coord coord = coordQueue.Dequeue();
        coordQueue.Enqueue(coord);
        return coord;
    }

    public Transform GetRandomTile(Queue<Coord> coordQueue) {
        Coord coord = GetRandomCoord(coordQueue);
        return tileMap[coord.x, coord.y];
    }

    [System.Serializable]
    public struct Coord {
        public int x;
        public int y;

        public Coord (int _x, int _y) {
            x = _x;
            y = _y;
        }
    }

    [System.Serializable]
    public class Map {

        [Min(2)]
        public int sizeX = 10;
        [Min(2)]
        public int sizeY = 10;

        [Range(0, 1)]
        public float obstacleDensity;
        public int seed = 1234;

        public float minObstacleHeight;
        public float maxObstacleHeight;

        public Color foregroundColor;
        public Color backgroundColor;

        [Min(1)]
        public float tileSize;
        public Coord mapCenter {
            get {
                return new Coord(sizeX / 2, sizeY / 2);
            }
        }

        [Range(0, 1)]
        public float marginPercent;

        public float mapSizeX {
            get {
                return tileSize * sizeX;
            }
        }
        public float mapSizeY {
            get {
                return tileSize * sizeY;
            }
        }

    }

}
