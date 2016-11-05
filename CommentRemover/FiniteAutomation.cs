using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentRemover
{
	class FiniteAutomation
	{
		Dictionary<KeyState, Action<FiniteState, SymbolType>> _links;
		Action<FiniteState, SymbolType> _defaultAction;
		Action<FiniteState, char> _necessaryAction;
		Func<char, SymbolType> _symbolClassifier;
		FiniteState _state;

		public Action<FiniteState, char> NecessaryAction
		{
			get
			{
				return _necessaryAction;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");
				_necessaryAction = value;
			}
		}

		public FiniteState State
		{
			get
			{
				return _state;
			}
			private set
			{
				if ((object)value == null)
					throw new ArgumentNullException("value");
				_state = value;
			}
		}

		public Action<FiniteState, SymbolType> DefaultAction
		{
			get
			{
				return _defaultAction;
			}
			set
			{
				_defaultAction = value ?? ((state, token) => { });
			}
		}

		public Func<char, SymbolType> SymbolClassifier
		{
			get
			{
				return _symbolClassifier;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");
				_symbolClassifier = value;
			}
		}

		public FiniteAutomation(Func<char, SymbolType> symbolClassifier)
		{
			_links = new Dictionary<KeyState, Action<FiniteState, SymbolType>>();
			DefaultAction = (state, token) => { };
			State = new FiniteState();
			SymbolClassifier = symbolClassifier;
			NecessaryAction = (state, ch) => { state.UserState.Append(ch); };
		}

		public void AddLink(KeyState state, Action<FiniteState, SymbolType> action)
		{
			if (!Enum.GetValues(typeof(KeyState)).Cast<int>().Contains((int)state))
				throw new ArgumentException("state");
			if (_links.ContainsKey(state))
				throw new ArgumentException("state");
			if (action == null)
				throw new ArgumentNullException("action");
			_links[state] = action;
		}

		public void Translate(char symbol)
		{
			NecessaryAction(State, symbol);
			var symbolType = SymbolClassifier(symbol);
			var action = _links.ContainsKey(State.KeyState) ? _links[State.KeyState] : DefaultAction;
			action(State, symbolType);
		}
	}
}
