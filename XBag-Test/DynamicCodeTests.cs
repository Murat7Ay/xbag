using DynamicCodeExecution.Core;
using DynamicCodeExecution.CSharp;
using DynamicCodeExecution.DotNet;
using NUnit.Framework;
using XInfrastructure;

namespace XBagTests;

[TestFixture]
public class DynamicCodeTests
{
    private CSharpDynamicScriptController controller;

    private string GetDynamicCode(string method) => @$"
            using XInfrastructure;
            using System;
            using System.IO;
            using System.Linq;
            using System.Text;
            using System.Collections.Generic;
            using System.Threading.Tasks;
            
            namespace DynamicCodeNameSpace
            {{
                public class DynamicMethodType
                {{
                    {method}
                }}
            }}
        ";

    [SetUp]
    public void Setup()
    {
        controller = new CSharpDynamicScriptController(new ClassCodeTemplate());
    }

    [Test]
    public void Evaluate_SumTwoKeys_ReturnsCorrectSum()
    {
        // Define input XBag
        XBag xBag = new XBag();
        xBag.Put("KEY1", 15);
        xBag.Put("KEY2", 12);

        // Define the dynamic code
        string dynamicCode = GetDynamicCode(@"
                    public XBag SumTwoKeys(XBag inBag)
                    {
                        long key1 = inBag.Get(""KEY1"").To_Long();
                        long key2 = inBag.Get(""KEY2"").To_Long();
                        XBag outBag = new XBag();
                        outBag.Put(""SUM"", key1 + key2);
                        return outBag;
                    }");
        controller.Evaluate(new DotNetDynamicScriptParameter(dynamicCode));
        // Execute the dynamic code
        var executionResult = controller.Execute(
            new DotNetCallArguments(
                namespaceName: "DynamicCodeNameSpace",
                className: "DynamicMethodType",
                methodName: "SumTwoKeys"),
            new List<ParameterArgument> { new("inBag", xBag) });

        // Assert the result
        Assert.IsTrue(executionResult.Success);
        XBag resultBag = (XBag)executionResult.ReturnValue;
        Assert.That(resultBag.Get("SUM").To_Long(), Is.EqualTo(27L));
    }

    [Test]
    public void Evaluate_DynamicCodeWithXBag_ReturnsCorrectResult()
    {
        // Define input data
        XBag inputBag = new XBag();
        inputBag.Put("KEY1", 15);

        // Define the dynamic code
        string dynamicCode = GetDynamicCode(@"
            public XBag ProcessXBag(XBag inputBag)
            {
                long key1 = inputBag.Get(""KEY1"").To_Long();
                XBag outputBag = new XBag();
                outputBag.Put(""SUM"", key1 * 2);
                return outputBag;
            }");
        controller.Evaluate(new DotNetDynamicScriptParameter(dynamicCode));

        // Execute the dynamic code
        var executionResult = controller.Execute(
            new DotNetCallArguments(
                namespaceName: "DynamicCodeNameSpace",
                className: "DynamicMethodType",
                methodName: "ProcessXBag"),
            new List<ParameterArgument> { new("inputBag", inputBag) });

        // Assert the result
        Assert.IsTrue(executionResult.Success);
        XBag resultBag = (XBag)executionResult.ReturnValue;
        Assert.That(resultBag.Get("SUM").To_Long(), Is.EqualTo(30L));
    }

    [Test]
    public void CalculateGraduationYearAge_ReturnsCorrectAge()
    {
        // Create a userBag with birth year and graduation year
        XBag userBag = new XBag();
        userBag.Put("BirthYear", 1990);
        userBag.Put("GraduationYear", 2010);

        // Define the dynamic code to calculate graduation year age
        string dynamicCode = GetDynamicCode(@"
        public void CalculateGraduationYearAge(XBag userBag)
        {
            long birthYear = userBag.Get(""BirthYear"").To_Long();
            long graduationYear = userBag.Get(""GraduationYear"").To_Long();
            long graduationAge = graduationYear - birthYear;
            userBag.Put(""GRADUATIONYEARAGE"", graduationAge);
        }");
        controller.Evaluate(new DotNetDynamicScriptParameter(dynamicCode));

        // Execute the dynamic code
        var executionResult = controller.Execute(
            new DotNetCallArguments(
                namespaceName: "DynamicCodeNameSpace",
                className: "DynamicMethodType",
                methodName: "CalculateGraduationYearAge"),
            new List<ParameterArgument> { new("userBag", userBag) });

        // Assert the result
        Assert.IsTrue(executionResult.Success);
        long graduationYearAge = userBag.Get("GRADUATIONYEARAGE").To_Long();
        Assert.AreEqual(20L, graduationYearAge); // Expected graduation year age as a long
    }
}