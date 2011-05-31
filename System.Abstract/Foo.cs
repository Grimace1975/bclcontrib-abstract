//using System;
//using System.Threading;
//using System.Reflection;
//using System.Diagnostics;

//class Foo
//{
        //private static Lazy<TIService> StackCrawlCreatingLazy(int stackIndex)
        //{
        //    var lazyCreateMethod = typeof(Lazy<TIService>).GetMethod("", BindingFlags.NonPublic);
        //    var stack = new StackTrace();
        //    var maxFrameIndex = Math.Min(stack.FrameCount, 5);
        //    while (stackIndex++ < maxFrameIndex)
        //    {
        //        var frame = stack.GetFrame(stackIndex);
        //        var method = frame.GetMethod();
        //        //method.ReflectedType.i
        //    }
        //    return null;
        //}


//    public static void Test(int stackIndex)
//    {
//        var helperType = Type.GetType("System.Diagnostics.StackFrameHelper");
//        var helper = Activator.CreateInstance(helperType, false, (Thread)null);
//        //
//        var stackTraceType = typeof(StackTrace);
//        stackTraceType.GetMethod("GetStackFramesInternal", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { helper, stackIndex, (Exception)null });
//        var methodHandles = (IntPtr[])helperType.GetField("rgMethodHandle", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(helper);
//        var handle = methodHandles[0];
//        var isNullMethod = typeof(IntPtr).GetMethod("IsNull", BindingFlags.NonPublic | BindingFlags.Instance);
//        if ((bool)isNullMethod.Invoke(handle, null))
//            return;
//        var x = typeof(RuntimeMethodHandle).GetMethod("", BindingFlags.NonPublic | BindingFlags.Static);
//        x.Dump();
//    }
//}
