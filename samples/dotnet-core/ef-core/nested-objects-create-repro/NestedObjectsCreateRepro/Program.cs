using System;
using System.Threading.Tasks;

namespace NestedObjectsCreateRepro
{
	class Program
	{
		static async Task Main(string[] args)
		{
			await Console.Out.WriteLineAsync("Hello World!");
		}
	}
}
