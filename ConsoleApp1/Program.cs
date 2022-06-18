namespace ConsoleApp1;

internal static class Program
{
    public static Thread? TriangleThread;
    public static Thread? RectangleThread;

    public const int TriangleTurn = 0;
    public const int RectangleTurn = 1;

    public const string TriangleShape = "triangle";
    public const string RectangleShape = "rectangle";
    
    public static int Turn = TriangleTurn;
    public static bool[] Flag = {false, false};
    public static int[] Speed = {800, 500};
    public static int TimeLimit = 1000 * 10;
    
    public static bool Stop = false;
    
    public static void Main()
    {
        TriangleThread = new Thread(TriangleDrawer);
        RectangleThread = new Thread(RectangleDrawer);
        
        Console.WriteLine("Started two threads for {0} seconds.", TimeLimit / 1000);
        
        TriangleThread.Start();
        RectangleThread.Start();

        Thread.Sleep(TimeLimit);

        Stop = true;
        
        TriangleThread.Join();
        RectangleThread.Join();
        
        Console.WriteLine("Stopped.");
    }

    private static void RectangleDrawer()
    {
        while (true)
        {
            Flag[RectangleTurn] = true;
            Turn = TriangleTurn;
            
            while (Flag[TriangleTurn] && Turn == TriangleTurn) {}
            
            Draw(RectangleShape, 1, 2);
            
            Flag[RectangleTurn] = false;
            
            Thread.Sleep(Speed[RectangleTurn]);
            
            if (Stop)
            {
                break;
            }
        }
    }

    private static void TriangleDrawer()
    {
        while (true)
        {
            Flag[TriangleTurn] = true;
            Turn = RectangleTurn;
            
            while (Flag[RectangleTurn] && Turn == RectangleTurn) {}
            
            Draw(TriangleShape, 3, 4);

            Flag[TriangleTurn] = false;
            
            Thread.Sleep(Speed[TriangleTurn]);

            if (Stop)
            {
                break;
            }
        }
    }

    private static void Draw(string shape, int x, int y)
    {
        Console.WriteLine("{0} [{1}, {2}]", shape, x, y);
    }
}