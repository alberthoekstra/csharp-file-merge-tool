# C# file merge tool

This tool can be used to merge all .cs files inside a given directory into a single file.  
The using statements outside the namespace declaration are moved inside the namespace declaration, for example:

Before:

```c#
using System;
namespace Test {

   // the rest of the code is omitted

};
```

After:

```c#
namespace Test {
   using System;

   // the rest of the code is omitted
};
```
