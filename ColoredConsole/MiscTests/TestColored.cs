using ColoredConsole.ColoredConsole;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ColoredConsole.MiscTests
{
	internal class TestColored
	{
		public static void WriteColored(string str)
		{
			// Distribuer en tableau les sous-chaînes de "str"

			// Distribuer en tableau des sous-chaînes de "str" selon une RegEx

			// https://stackoverflow.com/questions/29139320/split-string-to-array-from-text-and-html-tag

			//string[] blocks = Regex.Split(str, @"(<style color=[a-zA-Z]*>.*<\/style>)");
			//string[] blocks = Regex.Split(str, @"(<style color=[a-zA-Z]*>([^<>]*)<\/style>)");
			//string[] blocks = Regex.Split(str, @"(<style color=[a-zA-Z]*>([^<]*?)<\/style>)");
			MatchCollection blocks = Regex.Matches(str, @"((<?)([^<>]*)(>?))");

#if true
			Debug.WriteLine("Chaîne d'entrée divisée en blocs :");
			for (int i = 0; i < blocks.Count; i++)
			{
				Debug.WriteLine($"\t{i}. {blocks[i].Value}");
			}
			Debug.WriteLine(new String('-', 50));
#endif

			// Historique des couleurs utilisées
			List<Colors> colorsHistory = new ();
			// Niveau d'historique actuel (on utilise cela plutôt que la longueur de la liste car on doit atteindre -1)
			int historyLevel = 0;

			// Pour chaque bloc
			for (int i = 0; i < blocks.Count; i++)
			{
				string textBlock = blocks[i].Value;

				// Il peut y avoir un élément vide ou nul. On n'en veut pas.
				if (string.IsNullOrEmpty(textBlock)) continue;

				// Si la chaîne est du modèle : <style color=xxx> 
				/*if (IsTagOpen(value,out string openingTag)) // v.1
				{
					// Si c'est le cas, alors on a récupèré la valeur de l'attribut.
					// Est-ce maintenant une couleur de console ?
					if (Enum.TryParse(openingTag, out ConsoleColor currentColor))
					{
						// Changer la couleur de la console
						Console.ForegroundColor = currentColor;
					}
					// Sinon, rien
				}*/
				// Si la chaîne représente une balise ouvrante
				if (IsTagOpen(textBlock)) // v.2
				{
					// Obtenir un tableau d'attributs avec leur valeur (text=toto, background=toto, ...)
					MatchCollection attributsBlocks = Regex.Matches(textBlock, @"(text|background)=([a-zA-Z]*)");
#if true
					Debug.WriteLine("Chaîne de balise ouvrante divisée en blocs :");
					for (int j = 0; j < attributsBlocks.Count; j++)
					{
						Debug.WriteLine($"\t{j}. {attributsBlocks[j].Value}");
					}
					Debug.WriteLine(new String('-', 50));
#endif

					// Avec string.Split() :
					//string[] attributes = value.Split(' ');
					//Console.WriteLine("\t" + string.Join(' ',attributes));
					// Mais chaîne avec chevron fermant

					if (attributsBlocks.Count == 0) continue;

					// Explorer le tableau d'attributs 
					// On veut changer la couleur de la console et conserver les couleurs
					Colors currentColors = new();
					if (colorsHistory.Count > 0)
					{
						var lastEntry = colorsHistory[^1];//colorsHistory[colorsHistory.Count - 1];
						currentColors.foreground = lastEntry.foreground;
						currentColors.background = lastEntry.background;
					}

					bool isColorsOk = true;

					for (int j = 0; j < attributsBlocks.Count; j++)
					{
						string attribute = attributsBlocks[j].Value;
						string possibleColor = string.Empty;
						isColorsOk = true;

						// Si c'est pour le texte
						if (IsForText(attribute, out possibleColor))
						{
							// Alors on a récupèré la valeur de l'attribut.
							// Est-ce maintenant une couleur de console ? Si oui, changer le foreground
							if (Enum.TryParse(possibleColor, out ConsoleColor currentColor))
							{
								Console.ForegroundColor = currentColor;
								currentColors.foreground = currentColor;
							}
							else
							{
								isColorsOk = false;
							}

						}
						// Sinon si c'est pour le fond (idem précédent avec le background)
						else if (IsForBackground(attribute, out possibleColor))
						{
							if (Enum.TryParse(possibleColor, out ConsoleColor currentColor))
							{
								Console.BackgroundColor = currentColor;
								currentColors.background = currentColor;
							}
							else
							{
								isColorsOk = false;
							}
						}
						// Tout autre cas
						//else
						//{
						//	isColorsOk = false;
						//}
					}
					if (isColorsOk)
					{
						colorsHistory.Add(currentColors);
						historyLevel++;
					}
					else
					{
						Console.Write(textBlock);
					}
				}
				// Sinon, si la chaîne est du modèle : </color>
				else if (IsTagClose(textBlock))
				{
					// Tant que l'historique est au-delà de la valeur minimale, on diminue
					if (historyLevel > -1)
					{
						historyLevel--;
					}

					// Si historique > 0 (et non pas -1)
					if (historyLevel > 0)
					{
						// Supprimer la dernière entrée de la liste pour revenir au jeu de couleur précédent
						colorsHistory.RemoveAt(colorsHistory.Count - 1);

						// Prendre le dernier jeu de couleur de la liste
						var lastColors = colorsHistory[^1];//colorsHistory[colorsHistory.Count - 1];
						Console.ForegroundColor = lastColors.foreground;
						Console.BackgroundColor = lastColors.background;
					}
					// Si historique à 0
					else if (historyLevel == 0)
					{
						// Réinitialiser les couleurs avec les valeurs par défaut de la Console
						Console.ResetColor();
					}
					// Si on est à -1
					else
					{
						// Afficher simplement le contenu (on a un </color> qui traîne)
						Console.Write(textBlock);
					}

				}
				// Sinon, c'est du texte. Alors l'écrire
				else
				{
					Console.Write(textBlock);
				}
			}
		}

		public static void WriteLineColored(string str)
		{
			WriteColored(str);
			Console.Write("\n");
		}

		private static bool IsTagOpen(string str)
		{
			// <color text=toto background=titi>
			// <color background=titi>
			// <color background=titi>
			// <color text=toto>
			// <color> NON
			// <color toto=youpi> NON

			if (string.IsNullOrEmpty(str)) return false;

			//return Regex.IsMatch(str, @"(<color)( text=([a-zA-Z]*))?( background=([a-zA-Z]*))?(>)");
			//return Regex.IsMatch(str, @"(<color)(.*)(>)");
			//return Regex.IsMatch(str, @"(<color)(\s[a-zA-Z]*=[a-zA-Z]*)+(>)");
			//return Regex.IsMatch(str, @"(<color)(\s(text|background)?=[a-zA-Z]*)+(>)");
			return Regex.IsMatch(str, @"<color\s((text)|(background))=[^<>]*>");
		}

		private static bool IsTagClose(string str)
		{
			if (string.IsNullOrEmpty(str)) return false;

			//return Regex.IsMatch(str, @"(<\/color>)");

			return str == "</color>";
		}

		private static bool IsForText(string str, out string output)
		{
			output = String.Empty;

			if (string.IsNullOrEmpty(str)) return false;

			//Match result = Regex.Match(str, @"(?<=text=)(.*?)(?=>| )"); // "text=toto" suivi d'un espace ou d'un chevron fermant
			Match result = Regex.Match(str, @"(?<=text=)(.*)"); // seulement ce qui suit le "="

			if (!result.Success) return false;

			output = result.Value;
			return result.Success;
		}

		private static bool IsForBackground(string str, out string output)
		{
			output = String.Empty;

			if (string.IsNullOrEmpty(str)) return false;

			//Match result = Regex.Match(str, @"(?<=background=)(.*?)(?=>| )"); // "background=toto" suivi d'un espace ou d'un chevron fermant
			Match result = Regex.Match(str, @"(?<=background=)(.*)"); // seulement ce qui suit le "="

			if (!result.Success) return false;

			output = result.Value;
			return result.Success;

		}
	}
}
