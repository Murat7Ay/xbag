using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using XInfrastructure;
using XShell;

// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
// ReSharper disable RedundantCast
#pragma warning disable CS8600
#pragma warning disable CS8602

namespace XBagTests;

[TestFixture]
[SuppressMessage("Assertion",
    "NUnit2005:Consider using Assert.That(actual, Is.EqualTo(expected)) instead of Assert.AreEqual(expected, actual)")]
public class XParameterTests
{
    [Test]
    public void MethodA_ShouldAddKeysAndValuesToOutputBag()
    {
        // Arrange

        var inputBag = new XBag();
        inputBag.Put("Key1", "Hello");
        inputBag.Put("Key2", 42);
        IInvokeMethod invokeMethod = InvokeMethodFactory.Create("MethodA");
        // Act
        var outputBag = invokeMethod.Invoke("MethodA", inputBag);

        // Assert
        Assert.IsNotNull(outputBag);
        Assert.AreEqual(1, outputBag.GetReadOnlyDictionary().Count); // Check the number of keys in the output bag.
        Assert.IsTrue(outputBag.GetReadOnlyDictionary().ContainsKey("Key3")); // Check if the "Key3" key exists.
        Assert.AreEqual("Hello42", outputBag.Get("Key3").Value); // Check the value of "Key3".
    }

    [Test]
    public void MethodB_ShouldAddResultToOutputBag()
    {
        // Arrange
        IInvokeMethod invokeMethod = InvokeMethodFactory.Create("MethodB");
        var inputBag = new XBag();

        // Act
        var outputBag = invokeMethod.Invoke("MethodB", inputBag);

        // Assert
        Assert.IsNotNull(outputBag);
        Assert.AreEqual(1, outputBag.GetReadOnlyDictionary().Count); // Check the number of keys in the output bag.
        Assert.IsTrue(outputBag.GetReadOnlyDictionary().ContainsKey("Result")); // Check if the "Result" key exists.
        Assert.AreEqual(3.14M, outputBag.Get("Result").Value); // Check the value of "Result".
    }

    [Test]
    public void InvokeMethod_ValidMethodName_ShouldInvokeMethodAndReturnXBag()
    {
        // Arrange
        XBag inputBag = new XBag();
        string methodName = "TestMethod1";
        IInvokeMethod invokeMethod = InvokeMethodFactory.Create(methodName);
        // Act
        XBag resultBag = invokeMethod.Invoke(methodName, inputBag);

        // Assert
        Assert.IsNotNull(resultBag);
        // Add more assertions based on your specific method and expected behavior.
    }

    [Test]
    public void GetMethodInfo_ValidMethodName_ShouldReturnMethodInfo()
    {
        // Arrange
        string methodName = "TestMethod1";

        // Act
        XParameterMethodInfo methodInfo = XHandler.GetMethodInfoByName(methodName);

        // Assert
        Assert.IsNotNull(methodInfo);
        Assert.AreEqual("TestMethod1", methodInfo.MethodInfo.Name);
        // Add more assertions based on your specific method and expected behavior.
    }

    [Test]
    public void DetectMethods_ShouldDetectMethodsAndAttributes()
    {
        // Arrange

        // Create a test assembly with classes and methods decorated with XMethodAttribute and XParameterAttribute.
        TestAssemblyHelper.LoadTestAssembly();

        // Act
        var detectedMethods = XDetector.DetectMethods();

        // Assert
        Assert.NotNull(detectedMethods);

        Assert.GreaterOrEqual(detectedMethods.Count, 1);
    }
    //
    // [Test]
    // public void DetectMethods_ShouldDetectMethodsAndAttributes()
    // {
    //     // Arrange
    //     TestAssemblyHelper.LoadTestAssembly();
    //     var methods = XDetector.DetectMethods();
    //    
    // }
}

public static class TestAssemblyHelper
{
    public static Assembly LoadTestAssembly()
    {
        return Assembly.Load("XInfrastructure");
    }
}

public static class ParameterTestClass
{
    [XParameter("Key1", XType.String, XParameterType.Input, "Description 1")]
    [XParameter("Key2", XType.Int, XParameterType.Input, "Description 2")]
    [XMethod("MethodA", XMethodType.Service, "Method A Description")]
    public static XBag MethodA(XBag xBag)
    {
        string key1 = xBag.Get("Key1").Value.ToString();
        string key2 = xBag.Get("Key2").Value.ToString();

        XBag outBag = new XBag();
        outBag.Put("Key3", key1 + key2);
        return outBag;
    }

    [XParameter("Key3", XType.Double, XParameterType.OutPut, "Description 3")]
    [XMethod("MethodB", XMethodType.Service, "Method B Description")]
    public static XBag MethodB(XBag xBag)
    {
        XBag outBag = new XBag();
        outBag.Put("Result", 3.14M);
        return outBag;
    }

    [XMethod("TestMethod1", XMethodType.Service, "Test Method 1")]
    [XParameter("Input1", XType.String, XParameterType.Input, "Input 1 Description")]
    [XParameter("Output1", XType.Int, XParameterType.OutPut, "Output 1 Description")]
    public static XBag Method1(XBag input)
    {
        // Method implementation
        return input;
    }

    [XMethod("TestMethod2", XMethodType.Action, "Test Method 1")]
    [XParameter("Input2", XType.Bool, XParameterType.Input, "Input 2 Description")]
    public static XBag Method2(XBag input)
    {
        // Method implementation
        return input;
    }
}