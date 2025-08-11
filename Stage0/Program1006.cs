namespace Stage0
{
    
    partial class Program
    {
        static void Main(string[] args)
        {
            Wellcome1006();
            Wellcome5273(); 
            Console.ReadKey();
        }

        static partial void Wellcome5273();
        
        private static void Wellcome1006()
        {
            Console.WriteLine("Enter your name: ");
            string name = Console.ReadLine();
            // Console.Write(name);
            Console.WriteLine($"{name}, welcome to my first console application");
        }
    }
}
