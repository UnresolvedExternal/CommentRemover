using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentRemover
{
	enum KeyState
	{
		Global,
		GlobalDog,
		GlobalSlash,
		String,
		DogString,
		Char,
		EscapeBackSlash,
		MultiLineComment,
		SingleLineComment,
		DogStringJustConsumed,
		MultilineCommentIsAboutToEnd
	}
}
