namespace Gex.Cli
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Gex CLI - Modern Argument Parser");
                Console.WriteLine("Usage: gex <args>");
                return 1;
            }

            var parse = Parser.Parse(args);
            Console.WriteLine($"Tokens: {parse.Options.Count + parse.Positionals.Count}");
            Console.WriteLine("Options:");
            foreach (var kv in parse.Options)
            {
                Console.WriteLine($"  {kv.Key} => [{string.Join(", ", kv.Value.Select(v => new string(v.Span)))}]");
            }
            Console.WriteLine("Positionals:");
            foreach (var p in parse.Positionals)
            {
                Console.WriteLine($"  {new string(p.Span)}");
            }
            return 0;
        }
    }
}
