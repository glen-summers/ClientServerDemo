﻿namespace Client
{
	public interface IClient
	{
		string Foo(string value);
		string[] Query(int count);
	}
}