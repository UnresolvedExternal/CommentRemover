using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentRemover
{
	class FiniteState
	{
		Stack<KeyState> _keyStates;

		public StringBuilder UserState;

		public KeyState KeyState
		{
			get
			{
				return _keyStates.Peek();
			}
		}

		public void Begin(KeyState state)
		{
			_keyStates.Push(state);
		}

		public void End()
		{
			_keyStates.Pop();
		}

		public FiniteState()
		{
			_keyStates = new Stack<CommentRemover.KeyState>();
			UserState = new StringBuilder();
			Begin(KeyState.Global);
		}
	}
}
