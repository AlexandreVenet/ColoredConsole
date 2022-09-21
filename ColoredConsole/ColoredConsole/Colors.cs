using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoredConsole.ColoredConsole
{
	/// <summary>
	/// Classe contenant les couleurs de texte et d'arrière-plan.
	/// </summary>
	internal class Colors
	{
		/// <summary>
		/// Couleur pour le texte.
		/// </summary>
		public ConsoleColor foreground;

		/// <summary>
		/// Couleur pour l'arrière-plan.
		/// </summary>
		public ConsoleColor background;

		/// <summary>
		/// Appliquer les couleurs à celles de la console.
		/// </summary>
		public void ApplyColorsToConsole()
		{
			Console.ForegroundColor = foreground;
			Console.BackgroundColor = background;
		}

		/// <summary>
		/// Obtenir les couleurs actuelles de la console et les conserver.
		/// </summary>
		public void GetColorsFromConsole()
		{
			foreground = Console.ForegroundColor;
			background = Console.BackgroundColor;
		}
	}
}
