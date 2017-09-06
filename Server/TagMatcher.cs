﻿using System;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TagMatcher
	{
		enum Attribute { TAGS };

		public readonly string[] Tags;

		public TagMatcher(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Tags = (string[])attributes[(int)Attribute.TAGS];
		}

		public bool Matches(string[] Tags)
		{
			return this.Tags.All(i => Tags.Contains(i));
		}
	}
}