using DynamicCodeExecution.Core;
using DynamicCodeExecution.CSharp;
using DynamicCodeExecution.DotNet;
using XInfrastructure;

System.Collections.IList a = new List<int>();

XBag xBag = new XBag();
xBag.Put("KEY1", 11);
xBag.Put("KEY2", 24);
Console.WriteLine(DynamicClass.SumTwoKeys(xBag).Get("SUM").To_Long());


var controller = new CSharpDynamicScriptController(new ClassCodeTemplate());
controller.Evaluate(new DotNetDynamicScriptParameter(@"
using XInfrastructure;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace DynamicCodeNameSpace
{
    public class DynamicMethodType
    {
        public XBag SumTwoKeys(XBag inBag)
        {
            long key1 = inBag.Get(""KEY1"").To_Long();
            long key2 = inBag.Get(""KEY2"").To_Long();
            XBag outBag = new XBag();
            outBag.Put(""SUM"", key1 + key2);
            outBag.Put(""BOOLEAN"", new List<int>{1}.Any(x=>x == 1));
            return outBag;
        }
    }
}"));

var executionResult = controller.Execute(
    new DotNetCallArguments(
        namespaceName: "DynamicCodeNameSpace", 
        className: "DynamicMethodType",
        methodName: "SumTwoKeys"),
    new List<ParameterArgument> { new("inBag", xBag) });

Console.WriteLine(executionResult.ReturnValueOf<XBag>().Get("SUM").To_Long());
Console.WriteLine(executionResult.ReturnValueOf<XBag>().Get("BOOLEAN").To_Long());

public static class DynamicClass
{
    public static XBag SumTwoKeys(XBag inBag)
    {
        long key1 = inBag.Get("KEY1").To_Long();
        long key2 = inBag.Get("KEY2").To_Long();
        XBag outBag = new XBag();
        outBag.Put("SUM", key1 + key2);
        return outBag;
    }
}