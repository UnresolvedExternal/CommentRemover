using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CommentRemover
{
	class Program
	{
		static void Main(string[] args)
		{
			var automation = CreateAutomation();
			foreach (var ch in File.ReadAllText(args[0]))
				automation.Translate(ch);
			File.WriteAllText(args[1], automation.State.UserState.ToString());
		}

		static SymbolType ClassifySymbol(char ch)
		{
			switch (ch)
			{
				case '@':
					return SymbolType.Dog;
				case '\\':
					return SymbolType.BackSlash;
				case '"':
					return SymbolType.DoubledQuoter;
				case '\'':
					return SymbolType.SingleQuoter;
				case '\r':
					return SymbolType.LineDelimeter;
				case '\n':
					return SymbolType.LineDelimeter;
				case '*':
					return SymbolType.Splat;
				case '/':
					return SymbolType.Slash;
				default:
					return SymbolType.General;
			}
		}

		private static FiniteAutomation CreateAutomation()
		{
			var automation = new FiniteAutomation(ClassifySymbol);
			automation.NecessaryAction = (state, ch) =>
				{
					if (state.KeyState != KeyState.MultiLineComment && state.KeyState != KeyState.SingleLineComment
						&& state.KeyState != KeyState.MultilineCommentIsAboutToEnd)
					{
						state.UserState.Append(ch);
					}
					if (state.KeyState == KeyState.SingleLineComment &&
						automation.SymbolClassifier(ch) == SymbolType.LineDelimeter)
					{
						state.UserState.Append(ch);
					}
				};
			automation.AddLink(KeyState.Char, (state, token) =>
				{
					if (token == SymbolType.SingleQuoter)
						state.End();
					if (token == SymbolType.BackSlash)
						state.Begin(KeyState.EscapeBackSlash);
				});
			automation.AddLink(KeyState.DogString, (state, token) =>
				{
					if (token == SymbolType.DoubledQuoter)
					{
						state.End();
						state.Begin(KeyState.DogStringJustConsumed);
					}
				});
			automation.AddLink(KeyState.DogStringJustConsumed, (state, token) =>
				{
					if (token == SymbolType.DoubledQuoter)
					{
						state.End();
						state.Begin(KeyState.DogString);
					}
				});
			automation.AddLink(KeyState.EscapeBackSlash, (state, token) =>
				{
					state.End();
				});
			automation.AddLink(KeyState.Global, (state, token) =>
				{
					GlobalLink(state, token);
				});
			automation.AddLink(KeyState.GlobalDog, (state, token) =>
				{
					state.End();
					if (token == SymbolType.DoubledQuoter)
						state.Begin(KeyState.DogString);
					else
						GlobalLink(state, token);
				});
			automation.AddLink(KeyState.MultiLineComment, (state, token) =>
				{
					if (token == SymbolType.Splat)
						state.Begin(KeyState.MultilineCommentIsAboutToEnd);
				});
			automation.AddLink(KeyState.MultilineCommentIsAboutToEnd, (state, token) =>
				{
					if (token == SymbolType.Splat)
						return;
					if (token == SymbolType.Slash)
						state.End();
					state.End();
				});
			automation.AddLink(KeyState.SingleLineComment, (state, token) =>
				{
					if (token == SymbolType.LineDelimeter)
						state.End();
				});
			automation.AddLink(KeyState.String, (state, token) =>
				{
					if (token == SymbolType.BackSlash)
						state.Begin(KeyState.EscapeBackSlash);
					if (token == SymbolType.DoubledQuoter)
						state.End();
				});
			automation.AddLink(KeyState.GlobalSlash, (state, token) =>
				{
					state.End();
					if (token == SymbolType.Slash)
					{
						state.Begin(KeyState.SingleLineComment);
						state.UserState.Remove(state.UserState.Length - 2, 2);
						return;
					}
					if (token == SymbolType.Splat)
					{
						state.Begin(KeyState.MultiLineComment);
						state.UserState.Remove(state.UserState.Length - 2, 2);
						return;
					}
					GlobalLink(state, token);
				});
			return automation;
		}

		static void GlobalLink(FiniteState state, SymbolType token)
		{
			if (token == SymbolType.Slash)
				state.Begin(KeyState.GlobalSlash);
			if (token == SymbolType.Dog)
				state.Begin(KeyState.GlobalDog);
			if (token == SymbolType.DoubledQuoter)
				state.Begin(KeyState.String);
			if (token == SymbolType.SingleQuoter)
				state.Begin(KeyState.Char);
		}
	}
}
