namespace System;

public delegate TResult Func<TResult>();
public delegate TResult Func<T, TResult>(T arg1);
public delegate TResult Func<T1, T2, TResult>(T1 arg1, T2 arg2);
