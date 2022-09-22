using ColoredConsole.ColoredConsole;
using System.Drawing;

namespace ColoredConsole
{
	internal class Program
	{
		static void Main(string[] args)
		{
			// Paramètres de l'application Console
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			Console.Title = "Colored Console";

			Console.WriteLine("TEST 1");
			Colored.Write("Voici un <color text=Red background=DarkYellow>texte coloré avec fond coloré.</color>. ");
			Colored.Write("Voici un <color background=DarkYellow text=Red>texte coloré avec fond coloré.</color>. ");
			Colored.Write("Voici un <color text=Red>texte coloré</color>. ");
			Colored.Write("Voici un <color background=DarkYellow>texte avec un fond coloré.</color>.");
			Colored.Write("Voici un <color background=DarkYellow>texte avec un fond coloré <color text=Red>et du texte coloré</color>. Oui, oui</color>...\n");

			Console.ForegroundColor = ConsoleColor.Cyan;

			Console.WriteLine("\nTEST 2");
			Colored.Write("Test <color text=Blue><bleu></color>. ");
			Colored.Write("Test <color background=Blue><bleu></color>.\n");


			Console.WriteLine("\nTEST 3");
			Colored.WriteLine("x x x x <color text=Green>\n\tX</color>\nx x x x\n");

			Console.WriteLine("\nTEST 4");
			Colored.WriteLine("azertyuiopqsd<color text=Green>vert</color>wxcvb<color text=toto>CECI N'EST <color text=Red><<</color> PAS UNE COULEUR <color text=Red>>></color>ragnagna\n");

			Console.ForegroundColor = ConsoleColor.Yellow;

			Console.WriteLine("\nTEST 5");
			Colored.WriteLine("xxxx<color text=Cyan>xxxxxxxx<color background=DarkMagenta>xxxxxxxxxx<color text=Red>xxxxxxxx<color background=DarkGray text=Black>xxxx</color>xxx</color>xxxxxxxxxx</color>xxxx</color>xxxx\n");

			Console.WriteLine("\nTEST 6");
			Colored.WriteLine("sqdqs<color text=Red>rouge<color text=DarkCyan text=Blue>CYAN</color>rouge</color>dqdqdsqds<<<>>>qd<color>qd</color></color><color>qsdqsdsdq<color toto=prout>sdsqdqsd<color prout=rpout>qsd</color>");

			Console.WriteLine("\nFin du programme");
			Console.ReadLine();
		}
	}
}