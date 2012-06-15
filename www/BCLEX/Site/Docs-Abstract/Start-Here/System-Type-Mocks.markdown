# System Type Mocks

Simple abstractions are provided for system static types like DateTime or Directory. They all follow the same basic model.

* Implemented as a new Ex type suffix retaining their static properties.
* Addition of Nullable static properties suffexed with Mock to set or clear a Mock value. Mock values are cleared by setting Mock property to null;
* All Mock values are ThreadStatic so their values are singletons for the current thread only.

## Example
	// with out a mock value
	DateTimeEx2.Now -> returns current DateTime

	// setting a mock value
	DateTimeEx2.NowMock(new DateTime(1, 1, 2010));

	// after setting a mock value
	DateTimeEx2.Now -> returns 1/1/2010

# Implementations

System type mock values are provided for:

* System namespace
  * EnvironmentEx
  * DateTimeEx2
* System.IO namespace
  * FileEx2
  * DirectoryEx2
  * PathEx2