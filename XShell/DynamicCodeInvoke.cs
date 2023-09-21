using DynamicCodeExecution.Core;
using DynamicCodeExecution.CSharp;
using DynamicCodeExecution.DotNet;
using XInfrastructure;

namespace XShell;

public class DynamicCodeInvoke : IInvokeMethod
{
    private readonly CSharpDynamicScriptController _controller;

    private string GetDynamicCode(string methods) => @$"
            using XInfrastructure;
            using System;
            using System.IO;
            using System.Linq;
            using System.Text;
            using System.Collections.Generic;
            using System.Threading.Tasks;
            
            namespace DynamicCodeNameSpace
            {{
                public class DynamicCodeClass
                {{
                    {methods}
                }}
            }}
        ";

    public DynamicCodeInvoke()
    {
        _controller = new CSharpDynamicScriptController(new ClassCodeTemplate());
        _controller.Evaluate(new DotNetDynamicScriptParameter(GetDynamicCode(@"
                    public XBag SumTwoKeys(XBag inBag)
                    {
                        long key1 = inBag.Get(""KEY1"").To_Long();
                        long key2 = inBag.Get(""KEY2"").To_Long();
                        XBag outBag = new XBag();
                        outBag.Put(""SUM"", key1 + key2);
                        return outBag;
                    }")));
    }

    public XBag Invoke(string methodName, XBag xBag)
    {
        var executionResult = _controller.Execute(
            new DotNetCallArguments(
                namespaceName: "DynamicCodeNameSpace",
                className: "DynamicMethodType",
                methodName: "SumTwoKeys"),
            new List<ParameterArgument> { new("inBag", xBag) });

        return executionResult.ReturnValueOf<XBag>();
    }
}