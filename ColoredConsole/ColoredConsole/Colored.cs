using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ColoredConsole.ColoredConsole
{
	/// <summary>
	/// Coloriser des portions de texte en console avec un langage de balise.
	/// </summary>
	internal static class Colored
	{
		/// <summary>
		/// Ecrire des caractères dans le flux de texte de la sortie standard.
		/// </summary>
		/// <param name="str">Chaîne à traiter</param>
		public static void Write(string str)
		{
			// Prendre les couleurs actuelles de la console comme valeurs par défaut
			Colors defaultColors = new();
			defaultColors.GetColorsFromConsole();

			// Historique des couleurs utilisées
			List<Colors> colorsHistory = new();

			// Niveau d'historique actuel (on utilise cela plutôt que la longueur de la liste car on veut atteindre -1)
			int historyLevel = 0;

			// Distribuer en tableau les sous-chaînes de "str"
			MatchCollection blocks = Regex.Matches(str, @"((<?)([^<>]*)(>?))");

			// Pour chaque bloc
			for (int i = 0; i < blocks.Count; i++)
			{
				string textBlock = blocks[i].Value;

				if (string.IsNullOrEmpty(textBlock)) continue;

				// Si la chaîne représente une balise ouvrante
				if (IsTagOpen(textBlock))
				{
					// On veut changer la couleur de la console.
					// On veut aussi conserver les couleurs utilisées dans la liste d'historique.

					// Un jeu de couleurs que l'on va utiliser pour l'historique
					// Il reprend les couleurs actuellement utilisées
					Colors currentColors = new();
					currentColors.GetColorsFromConsole();

					// Pour déterminer si on va faire une nouvelle entrée dans l'historique
					int isColorsOk = 0;

					// Maintenant, on cherche dans la chaîne de balise ouvrante la valeur représentant une couleur

					/*
					string attributeValue = string.Empty;

					// Pour la couleur de texte
					Match result = Regex.Match(textBlock, @"(?<=text=)([^> ]*)"); // ce qui suit "text=" et qui n'est pas lui-même suivi par ">" ou " "
					if (result.Success)
					{
						attributeValue = result.Value;

						// On dispose maintenant de la valeur d'attribut, valeur qui peut représenter une couleur valide.
						// Est-ce maintenant une couleur de console ? 
						if (Enum.TryParse(attributeValue, out ConsoleColor colorOk))
						{
							//Console.ForegroundColor = colorOk;
							currentColors.foreground = colorOk;
							isColorsOk ++;
						}
					}

					// Pour la couleur d'arrière-plan
					Match result2 = Regex.Match(textBlock, @"(?<=background=)([^> ]*)"); // idem avec "background="
					if (result2.Success)
					{
						attributeValue = result2.Value;

						if (Enum.TryParse(attributeValue, out ConsoleColor colorOk))
						{
							//Console.BackgroundColor = colorOk;
							currentColors.background = colorOk;
							isColorsOk++;
						}
					}
					// Pas super...
					*/

					(bool Ok, ConsoleColor Color) checkColor;

					// Pour la couleur de texte 
					// Ce qui suit "text=" et qui n'est pas lui-même suivi par ">" ou " "
					checkColor = CheckValidColor(textBlock, new Regex(@"(?<=text=)([^> ]*)"));
					if (checkColor.Ok)
					{
						currentColors.foreground = checkColor.Color;
						isColorsOk++;
					}

					// Pour la couleur d'arrière-plan
					// (idem précédent avec "background=")
					checkColor = CheckValidColor(textBlock, new Regex(@"(?<=background=)([^> ]*)"));
					if (checkColor.Ok)
					{
						currentColors.background = checkColor.Color;
						isColorsOk++;
					}

					// Si on a bien des couleurs (une ou deux)
					if (isColorsOk > 0)
					{
						// Appliquer les couleurs à la console
						currentColors.ApplyColorsToConsole();

						// Modifier l'historique et le niveau
						colorsHistory.Add(currentColors);
						historyLevel++;
					}
					// Sinon, c'est que le ou les tests des couleurs ont échoué
					else
					{
						Console.Out.Write(textBlock);
					}
				}
				// Sinon, si la chaîne représente une balise fermante
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

						// Prendre le dernier jeu de couleur de la liste et appliquer cela à la console
						var lastColors = colorsHistory[^1]; // idem : colorsHistory[colorsHistory.Count - 1];
						lastColors.ApplyColorsToConsole();
					}
					// Sinon, si historique à 0 (première entrée réelle)
					else if (historyLevel == 0)
					{
						// Réinitialiser les couleurs avec les valeurs par défaut
						//Console.ResetColor(); // Nan...
						defaultColors.ApplyColorsToConsole();
					}
					// Sinon, c'est que l'historique est à -1 (rien qui ne concerne des couleurs)
					else
					{
						// Afficher simplement le contenu (un </color> qui traîne ?)
						Console.Out.Write(textBlock);
					}
				}
				// Sinon, si c'est du texte
				else
				{
					Console.Out.Write(textBlock);
				}
			}
		}

		/// <summary>
		/// Ecrire un texte finissant sur un caractère de terminaison pour former une ligne dans la sortie standard.
		/// </summary>
		/// <param name="str">Chaîne à traiter</param>
		public static void WriteLine(string str)
		{
			Write(str);
			Console.Out.Write("\n");
		}

		/// <summary>
		/// Tester si la chaîne représente une balise d'ouverture.
		/// </summary>
		/// <param name="str">Chaîne à tester</param>
		/// <returns>True/False</returns>
		private static bool IsTagOpen(string str)
		{
			if (string.IsNullOrEmpty(str)) return false;

			// <color text=toto background=titi>
			// <color background=titi>
			// <color background=titi>
			// <color text=toto>
			// NON <color>
			// NON <color toto=youpi>

			return Regex.IsMatch(str, @"<color\s((text)|(background))=[^<>]*>");
		}

		/// <summary>
		/// Tester si la chaîne représente une balise de fermeture.
		/// </summary>
		/// <param name="str">Chaîne à tester</param>
		/// <returns>True/False</returns>
		private static bool IsTagClose(string str)
		{
			return str == "</color>";
		}

		/// <summary>
		/// Vérifier si la chaîne entrée correspond à une RegEx et si oui convertir en ConsoleColor.
		/// </summary>
		/// <param name="textBlock">Chaîne de type "attribut=valeur"</param>
		/// <param name="rg">RegEx à évaluer</param>
		/// <returns>
		/// Tuple. <br/>
		/// Ok = true/false. <br/>
		/// Si Ok : Color est la couleur valide obtenue. <br/>
		/// Si !Ok : Color est la valeur par défaut du type ConsoleColor.
		/// </returns>
		private static (bool Ok, ConsoleColor Color) CheckValidColor(string textBlock, Regex rg)
		{
			(bool, ConsoleColor) result = (false, default);

			Match match = Regex.Match(textBlock, rg.ToString());

			if (match.Success)
			{
				string attributeValue = match.Value;

				// On dispose de la valeur d'attribut. Est-ce maintenant une couleur de console ? 
				if (Enum.TryParse(attributeValue, out result.Item2))
				{
					result.Item1 = true;
				}
			}

			return result;
		}
	}
}
