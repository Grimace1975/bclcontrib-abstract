# Nparams Keyword

Nparams is an addition way of sending optional structured data to a method. 

### Examples
	public static void Foo(Nparams args)
	{
		var myType = args.Get<MyType>();
	}

	Foo(new { MyType = new MyType { ID = "ID" } });