using System;


namespace Stream
{
    class Program
    {
        static void Main(string[] args)
        {
            Streamer streamer = new Streamer(Convert.ToInt32(args[2]));
            switch (args[3])
            {
                case "-e":
                    streamer.Encode(args[0], args[1] + "/");//a[0], a[3]);
                    break;
                case "-d":
                    streamer.Decode(args[0] + "/", args[1]);//a[3], a[4]);
                    break;
            }
            Console.WriteLine("Press any key...");
            Console.Read();
        }
    }
}
