using System;

namespace NGramExtractor
{
	public interface IExtractable
	{
		string[] Extract(string str);
	}
}

