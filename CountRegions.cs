using System;

// Interface names may start with I (ex. IMapInterface)
public interface MapInterface
{
    // Creates / Allocates a map of given size.
    void SetSize(in int width, in int height);
    // Get dimensions of a given map.
    void GetSize(out int width, out int height);
    // Sets border at given point.
    void SetBorder(int x, int y);
    // Clears border at given point.
    void ClearBorder(int x, int y);
    // Checks if given point is border.
    bool IsBorder(int x, int y);
    // Show map contents.
    void Show();
}

public interface ZoneCounterInterface
{
    // Feeds map data into solution class, then get ready for Solve() method.
    void Init(MapInterface map);
    // Counts zones in map provided with Init() method, then return the result.
    int Solve();
}

namespace Baris_Demirkilic
{
    // This is the solution class
    class CountRegions : ZoneCounterInterface
    {
        // Map data properties
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public int[,] MapArr { get; set; }
        public int RegionCount { get; set; }

        // Feeds map data into solution class, then get ready for Solve() method.
        public void Init(MapInterface map)
        {
            // Cannot pass Properties as out parameter, so use temporary variables
            // Get size values from the interface variable and set it into class Properties
            int outWidth, outHeight;
            map.GetSize(out outWidth, out outHeight);
            MapWidth = outWidth;
            MapHeight = outHeight;

            // Initialize region count to 0
            RegionCount = 0;

            // Initialize map array
            // C# automatically initialize numeric arrays with 0 values
            // 0 values will represent region soil
            MapArr = new int[MapHeight, MapWidth];

            // Borders will be 1 in the map array
            for (int i = 0; i < MapHeight; i++)
            {
                for (int j = 0; j < MapWidth; j++)
                {
                    if (map.IsBorder(j, i))
                    {
                        MapArr[i, j] = 1;
                    }
                }
            }
        }

        // Counts zones in map provided with Init() method, then return the result.
        public int Solve()
        {
            SetAllRegionsInMap();
            return RegionCount;
        }

        // This function loops through every index and when it finds empty regions (0 values) it calls region setter function
        public void SetAllRegionsInMap()
        {
            // Loop until every empty region is set
            // Find the map index that has lowest row then column no with an empty region
            for (int i = 0; i < MapHeight; i++)
            {
                for (int j = 0; j < MapWidth; j++)
                {
                    if (MapArr[i, j] == 0)
                    {
                        RegionSetter(i, j);
                    }
                }
            }
        }

        // Region setter function decides the new region flag and increments region count
        // Then it will call RecursiveRegionSetter function to do the rest
        public void RegionSetter(in int startIdxRow, in int startIdxCol)
        {
            int curRegionNo = RegionCount + 2;
            RegionCount++;
            RecursiveRegionSetter(startIdxRow, startIdxCol, curRegionNo);
        }

        // This recursive function sets the current region flag to every adjacent index (Left, Right, Up, Down),
        // until it comes across borders, map limits or already flagged regions
        public void RecursiveRegionSetter(in int curIdxRow, in int curIdxCol, in int curRegionNo)
        {
            int curMapVal = MapArr[curIdxRow, curIdxCol];
            if (curMapVal == 0)
            {
                // Empty region: Set the curRegionNo to it
                MapArr[curIdxRow, curIdxCol] = curRegionNo;
                if (curIdxRow > 0)
                {
                    // Go up
                    RecursiveRegionSetter(curIdxRow - 1, curIdxCol, curRegionNo);
                }
                if (curIdxRow < MapHeight - 1)
                {
                    // Go down
                    RecursiveRegionSetter(curIdxRow + 1, curIdxCol, curRegionNo);
                }
                if (curIdxCol > 0)
                {
                    // Go left
                    RecursiveRegionSetter(curIdxRow, curIdxCol - 1, curRegionNo);
                }
                if (curIdxCol < MapWidth - 1)
                {
                    // Go right
                    RecursiveRegionSetter(curIdxRow, curIdxCol + 1, curRegionNo);
                }
            }
            else if (curMapVal == 1)
            {
                // Border: Stop
                return;
            }
            else if (curMapVal == curRegionNo)
            {
                // Already set region: Stop
                return;
            }
            else
            {
                // Error: Must not come accross another region
                throw new RegionException("Overlapping regions in the map.");
            }
        }

        // Custom exception for recursive region setter function
        [Serializable]
        public class RegionException : Exception
        {
            public RegionException() {}
            public RegionException(string message) : base(message) {}
            public RegionException(string message, Exception inner) : base(message, inner) {}
        }
    }

    // This is not related to the solution
    // This class is for testing purposes
    public class BarisMap : MapInterface
    {
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public int[,] MapArr { get; set; }

        // Creates / Allocates a map of given size.
        public void SetSize(in int width, in int height)
        {
            MapWidth = width;
            MapHeight = height;
            MapArr = new int[MapHeight, MapWidth];
        }
        // Get dimensions of a given map.
        public void GetSize(out int width, out int height)
        {
            width = MapWidth;
            height = MapHeight;
        }
        // Sets border at given point.
        public void SetBorder(int x, int y)
        {
            MapArr[y, x] = 1;
        }
        // Clears border at given point.
        public void ClearBorder(int x, int y)
        {
            if (MapArr[y, x] == 1)
                MapArr[y, x] = 0;
        }
        // Checks if given point is border.
        public bool IsBorder(int x, int y)
        {
            return MapArr[y, x] == 1;
        }
        // Show map contents.
        public void Show()
        {
            for (int i = 0; i < MapHeight; i++)
            {
                for (int j = 0; j < MapWidth; j++)
                {
                    Console.Write(MapArr[i, j]);
                }
                Console.WriteLine();
            }
        }
    }

    // This is not related to the solution
    // To test the solution
    class Program
    {
        static void Main(string[] args)
        {
            BarisMap barisMap = new BarisMap();
            
            // Set borders as in the given example
            // First border
            barisMap.SetSize(36, 24);
            barisMap.SetBorder(18, 0);
            barisMap.SetBorder(17, 1);
            barisMap.SetBorder(16, 1);
            barisMap.SetBorder(15, 2);
            barisMap.SetBorder(14, 3);
            barisMap.SetBorder(13, 4);
            barisMap.SetBorder(12, 4);
            barisMap.SetBorder(11, 5);
            barisMap.SetBorder(10, 6);
            barisMap.SetBorder(9, 7);
            barisMap.SetBorder(8, 7);
            barisMap.SetBorder(7, 8);
            barisMap.SetBorder(6, 9);
            barisMap.SetBorder(5, 9);
            barisMap.SetBorder(5, 10);
            barisMap.SetBorder(4, 10);
            barisMap.SetBorder(3, 11);
            barisMap.SetBorder(2, 12);
            barisMap.SetBorder(1, 13);
            barisMap.SetBorder(0, 13);
            // Second border
            barisMap.SetBorder(6, 11);
            barisMap.SetBorder(7, 12);
            barisMap.SetBorder(7, 13);
            barisMap.SetBorder(8, 14);
            barisMap.SetBorder(8, 15);
            barisMap.SetBorder(9, 16);
            barisMap.SetBorder(9, 17);
            barisMap.SetBorder(10, 18);
            barisMap.SetBorder(10, 19);
            barisMap.SetBorder(11, 20);
            barisMap.SetBorder(11, 21);
            barisMap.SetBorder(12, 22);
            barisMap.SetBorder(12, 23);
            // Third border
            barisMap.SetBorder(10, 17);
            barisMap.SetBorder(11, 17);
            barisMap.SetBorder(12, 17);
            barisMap.SetBorder(13, 16);
            barisMap.SetBorder(14, 16);
            barisMap.SetBorder(15, 16);
            barisMap.SetBorder(16, 16);
            barisMap.SetBorder(17, 15);
            barisMap.SetBorder(18, 15);
            barisMap.SetBorder(19, 15);
            barisMap.SetBorder(20, 15);
            barisMap.SetBorder(21, 14);
            barisMap.SetBorder(22, 14);
            barisMap.SetBorder(23, 14);
            barisMap.SetBorder(24, 14);
            barisMap.SetBorder(25, 13);
            barisMap.SetBorder(26, 13);
            barisMap.SetBorder(27, 13);
            barisMap.SetBorder(28, 13);
            // Fourth border
            barisMap.SetBorder(29, 13);
            barisMap.SetBorder(28, 12);
            barisMap.SetBorder(28, 11);
            barisMap.SetBorder(27, 10);
            barisMap.SetBorder(27, 9);
            barisMap.SetBorder(27, 8);
            barisMap.SetBorder(26, 7);
            barisMap.SetBorder(26, 6);
            barisMap.SetBorder(26, 5);
            barisMap.SetBorder(25, 4);
            barisMap.SetBorder(25, 3);
            barisMap.SetBorder(24, 2);
            barisMap.SetBorder(24, 1);
            barisMap.SetBorder(24, 0);
            barisMap.SetBorder(29, 14);
            barisMap.SetBorder(29, 15);
            barisMap.SetBorder(30, 16);
            barisMap.SetBorder(30, 17);
            barisMap.SetBorder(31, 18);
            barisMap.SetBorder(31, 19);
            barisMap.SetBorder(31, 20);
            barisMap.SetBorder(32, 21);
            barisMap.SetBorder(32, 22);
            barisMap.SetBorder(33, 23);
            // Fifth border
            barisMap.SetBorder(27, 6);
            barisMap.SetBorder(28, 6);
            barisMap.SetBorder(29, 6);
            barisMap.SetBorder(30, 6);
            barisMap.SetBorder(31, 6);
            barisMap.SetBorder(32, 6);
            barisMap.SetBorder(33, 6);
            barisMap.SetBorder(34, 6);
            barisMap.SetBorder(35, 5);

            barisMap.Show();

            CountRegions solObj = new CountRegions();
            solObj.Init(barisMap);
            int resRegionCount = solObj.Solve();
            Console.WriteLine(resRegionCount);
        }
    }
}
