using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataObjectExtension;
using System.Linq;
using System.Collections.Generic;

namespace DataObjectExtensionTest
{
  public enum CountEnum
  {
    one,
    two,
    three,
    four,
    five
  }

  public class TestObj1
  {
    public bool BoolValue
    {
      get;
      set;
    }

    public bool? BoolNullableValue
    {
      get;
      set;
    }

    public int IntValue
    {
      get;
      set;
    }

    public int? IntNullableValue
    {
      get;
      set;
    }

    public string StringValue
    {
      get;
      set;
    }

    public CountEnum CountEnumValue
    {
      get;
      set;
    }
    public CountEnum? CountEnumNullableValue
    {
      get;
      set;
    }
  }

  public class TestObj2
  {
    public CountEnum? CountEnumNullableValue
    {
      get;
      set;
    }

    public int? IntNullableValue
    {
      get;
      set;
    }

    public bool? BoolNullableValue
    {
      get;
      set;
    }

    public CountEnum CountEnumValue
    {
      get;
      set;
    }

    public bool BoolValue
    {
      get;
      set;
    }
    

    public string StringValue
    {
      get;
      set;
    }

    public int IntValue
    {
      get;
      set;
    }

    public long LongValue
    {
      get;
      set;
    }

    public float FloatValue
    {
      get;
      set;
    }

    public string Name
    {
      get;
      set;
    }

    public int AddInt(int firstValue, int secondValue)
    {
      return firstValue + secondValue;
    }
  }

  [TestClass]
  public class UnitTest1
  {

    [TestMethod]
    public void HashCalculateTest()
    {
       var objOne = new TestObj1();
       var hashBefore = objOne.CreateHash();
       objOne.BoolValue = true;
       var hashAfter = objOne.CreateHash();
       Assert.IsFalse(hashBefore.SequenceEqual(hashAfter), "The hash value before and after a change on the object should be different.");
    }

    [TestMethod]
    public void SameTypeCompareDefaultTest()
    {
       var objOne = new TestObj1();
       var objTwo = new TestObj1();
       var result = objOne.IsEqualTo(objTwo);
       Assert.IsTrue(result, "Comparsion of two identical object should pass.");
    }

    [TestMethod]
    public void SameTypeCompareDiffBoolTest()
    {
       var objOne = new TestObj1();
       var objTwo = new TestObj1();
       objTwo.BoolValue = true;
       var result = objOne.IsEqualTo(objTwo);
       Assert.IsFalse(result, "Comparsion of two objects of the same type with different values should fail.");
    }

    [TestMethod]
    public void SameTypeCompareDiffIntTest()
    {
       var objOne = new TestObj1();
       var objTwo = new TestObj1();
       objTwo.IntValue = 42;
       var result = objOne.IsEqualTo(objTwo);
       Assert.IsFalse(result, "Comparsion of two objects of the same type with different values should fail.");
    }

    [TestMethod]
    public void SameTypeCompareDiffStringTest()
    {
       var objOne = new TestObj1();
       var objTwo = new TestObj1();
       objTwo.StringValue = "42";
       var result = objOne.IsEqualTo(objTwo);
       Assert.IsFalse(result, "Comparsion of two objects of the same type with different values should fail.");
    }

    [TestMethod]
    public void DifferentTypeCompareDefaultTest()
    {
       var objOne = new TestObj1();
       var objTwo = new TestObj2();
       var result = objOne.IsEqualTo(objTwo);
       Assert.IsTrue(result, "Comparsion of two duck type identical objects should pass.");
    }

    [TestMethod]
    public void DifferentTypeCompareWithUnrelatedPropertyTest()
    {
       var objOne = new TestObj1();
       var objTwo = new TestObj2();
       //
       // There is no 'Name' property in the first object. For that reason there
       // is no comparsion on the 'Name' property between the first and the second
       // object.
       //
       objTwo.Name = "This is object two.";
       var result = objOne.IsEqualTo(objTwo);
       Assert.IsTrue(result, "Comparsion of two duck type identical objects should pass.");
    }

    [TestMethod]
    public void DifferentTypeCompareWithMissingPropertyTest()
    {
       var objOne = new TestObj1();
       var objTwo = new TestObj2();
       objTwo.Name = "This is object two.";
       //
       // The comparsion for the 'TestObj2' against the 'TestObj1' should fail since
       // 'TestObj2' has more properties than 'TestObj1'. There is no corresponding
       // property in 'TestObj1' to compare against for those properties.
       //
       var result = objTwo.IsEqualTo(objOne);
       Assert.IsFalse(result, "Comparsion of two duck type different objects should fail.");
    }

    [TestMethod]
    public void DifferentTypeWithMappingTest()
    {
       var objOne = new TestObj1();
       var objTwo = new TestObj2();
       objOne.StringValue = "String from obj one";
       objTwo.Name = "String from obj one";
       List<Map> mapList = new List<Map>();
       mapList.Add(new Map("StringValue","Name"));
       var result = objOne.IsEqualTo(objTwo,mapList);
       Assert.IsTrue(result, "Comparsion of two duck type different objects with a valid mapping should pass.");
    }

    [TestMethod]
    public void DifferentTypeWithInvalidMappingTest()
    {
       var objOne = new TestObj1();
       var objTwo = new TestObj2();
       objOne.StringValue = "String from obj one";
       objTwo.Name = "String from obj one";
       List<Map> mapList = new List<Map>();
       mapList.Add(new Map("StringValue","NameX"));
       var result = objOne.IsEqualTo(objTwo,mapList);
       Assert.IsFalse(result, "Comparsion of two duck type different objects with an invalid mapping should fail.");
    }

    [TestMethod]
    public void DifferentTypeWithMappingAndConversionTest()
    {
       var objOne = new TestObj1();
       var objTwo = new TestObj2();
       objOne.IntValue = 42;
       objTwo.FloatValue = (float) 42;
       List<Map> mapList = new List<Map>();
       //
       // Mapping the 'IntValue' property of 'TestObj1' to the 'FloatValue' of 'TestObj2'.
       // 'TestObj1' doesn't have a 'FloatValue' property.
       //
       mapList.Add(new Map("IntValue","FloatValue", (object source) => { return (float) ((int) source); }));
       var result = objOne.IsEqualTo(objTwo,mapList);
       Assert.IsTrue(result, "Comparsion of two duck type different objects with a valid mapping should pass.");
    }


    [TestMethod]
    public void DifferentTypeWithExcludeSelectionTest()
    {
       var objOne = new TestObj1();
       var objTwo = new TestObj2();
       objOne.IntValue = 42;
       objTwo.IntValue = 41;
       objOne.StringValue = "42";
       objTwo.StringValue = "42";
       //
       // Exclude the 'IntValue' property from comparsion.
       //
       var result = objOne.IsEqualTo(objTwo,null, (first) => { return first.Name == "IntValue";});
       Assert.IsTrue(result, "Comparsion of two objects with different but excluded values should pass.");
    }

    [TestMethod]
    public void DifferentTypeWithExcludeSelectionAndMappingTest()
    {
       var objOne = new TestObj1();
       var objTwo = new TestObj2();
       objOne.IntValue = 42;
       objTwo.StringValue = "42";
       List<Map> mapList = new List<Map>();
       //
       // Mapping the 'IntValue' from the first object to the 'StringValue' of the second object.
       //
       mapList.Add(new Map("IntValue","StringValue", (object source) => { return ((int) source).ToString(); }));
       //
       // Excluding the 'StringValue' from the first object from comparsion since that comparsion
       // would fail because of the mapping.
       //
       var result = objOne.IsEqualTo(objTwo,mapList,(first) => { return first.Name == "StringValue";});
       Assert.IsTrue(result, "Comparsion of two duck type different objects with a valid mapping should pass.");
    }

    [TestMethod]
    public void CopySameTypeTest()
    {
       var objOne = new TestObj1();
       var objTwo = new TestObj1();
       byte[] objOneHash;
       byte[] objTwoHash;
       objOne.BoolNullableValue = false;
       objOne.BoolValue = true;
       objOne.CountEnumNullableValue = CountEnum.three;
       objOne.CountEnumValue = CountEnum.two;
       objOne.IntNullableValue = null;
       objOne.IntValue = 42;
       objOne.StringValue  = "Values from objOne.";
       objOneHash = objOne.CreateHash();
       objOne.CopyTo(objTwo);
       objTwoHash = objTwo.CreateHash();

       var result = objOne.IsEqualTo(objTwo);

       Assert.IsTrue(objOneHash.SequenceEqual(objTwoHash), "Since both objects of the same typ the hash values should be equal.");
       Assert.IsTrue(result, "Both object should be considered equal after the copy operation.");
    }

    [TestMethod]
    public void CopyDifferentTypeTest()
    {
       var objOne = new TestObj1();
       var objTwo = new TestObj2();
       byte[] objOneHash;
       byte[] objTwoHash;
       objOne.BoolNullableValue = false;
       objOne.BoolValue = true;
       objOne.CountEnumNullableValue = CountEnum.three;
       objOne.CountEnumValue = CountEnum.two;
       objOne.IntNullableValue = null;
       objOne.IntValue = 42;
       objOne.StringValue  = "Values from objOne.";
       objOneHash = objOne.CreateHash();
       objOne.CopyTo(objTwo);
       objTwoHash = objTwo.CreateHash();

       var result = objOne.IsEqualTo(objTwo);

       Assert.IsFalse(objOneHash.SequenceEqual(objTwoHash), "Since both objects of different typ the hash values should not be equal.");
       Assert.IsTrue(result, "Both object should be considered equal after the copy operation.");
    }

    [TestMethod]
    public void CopyDifferentTypeFail()
    {
       var objOne = new TestObj1();
       var objTwo = new TestObj2();
       objTwo.BoolNullableValue = false;
       objTwo.BoolValue = true;
       objTwo.CountEnumNullableValue = CountEnum.three;
       objTwo.CountEnumValue = CountEnum.two;
       objTwo.IntNullableValue = null;
       objTwo.IntValue = 42;
       objTwo.StringValue  = "Values from objTwo.";
       objTwo.Name = "This is obj two";
       objTwo.FloatValue = 42F;

       Assert.ThrowsException<DataObjectExtension.DataObjectExtensionException>( () => { objTwo.CopyTo(objOne); }, "The copy operation should fail with a 'DataObjectExtensionException' because the target object is missing some properties.");
    }


  }//END class
}//END namespace
