namespace SimpleAutoMapper.Test;

[TestClass]
public class ScalarTest
{
    private static ISimpleAutoMapper CreateSimpleAutoMapper() => new SimpleAutoMapper(TestLogFactory.CreateLog<SimpleAutoMapper>());

    [TestMethod]
    public void ScalarMember()
    {
        var expected = new ScalarSrc()
        {
            MemberInterger = 42,
            MemberDouble = Math.PI,
            MemberString = Guid.NewGuid().ToString(),
        };
        ISimpleAutoMapper mapper = CreateSimpleAutoMapper();
        var actual = mapper.Map<ScalarSrc, ScalarDst>(expected);

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.MemberInterger, actual.MemberInterger);
        Assert.AreEqual(expected.MemberDouble, actual.MemberDouble);
        Assert.AreEqual(expected.MemberString, actual.MemberString);
    }

    [TestMethod]
    public void ScalarProp()
    {
        var expected = new ScalarSrc()
        {
            PropInterger = 42,
            PropDouble = Math.PI,
            PropString = Guid.NewGuid().ToString(),
        };
        ISimpleAutoMapper mapper = CreateSimpleAutoMapper();
        var actual = mapper.Map<ScalarSrc, ScalarDst>(expected);

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.PropInterger, actual.PropInterger);
        Assert.AreEqual(expected.PropDouble, actual.PropDouble);
        Assert.AreEqual(expected.PropString, actual.PropString);
    }

    [TestMethod]
    public void NullableScalarMember()
    {
        {
            var expected = new ScalarSrc()
            {
                MemberNullableInterger = null,
                MemberNullableDouble = null,
                MemberNullableString = null,
            };
            ISimpleAutoMapper mapper = CreateSimpleAutoMapper();
            var actual = mapper.Map<ScalarSrc, ScalarDst>(expected);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.MemberNullableInterger, actual.MemberNullableInterger);
            Assert.AreEqual(expected.MemberNullableDouble, actual.MemberNullableDouble);
            Assert.AreEqual(expected.MemberNullableString, actual.MemberNullableString);
        }
        {
            var expected = new ScalarSrc()
            {
                MemberNullableInterger = 42,
                MemberNullableDouble = Math.PI,
                MemberNullableString = Guid.NewGuid().ToString(),
            };
            ISimpleAutoMapper mapper = CreateSimpleAutoMapper();
            var actual = mapper.Map<ScalarSrc, ScalarDst>(expected);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.MemberNullableInterger, actual.MemberNullableInterger);
            Assert.AreEqual(expected.MemberNullableDouble, actual.MemberNullableDouble);
            Assert.AreEqual(expected.MemberNullableString, actual.MemberNullableString);
        }
    }

    [TestMethod]
    public void NullableScalarProp()
    {
        {
            var expected = new ScalarSrc()
            {
                PropNullableInterger = null,
                PropNullableDouble = null,
                PropNullableString = null,
            };
            ISimpleAutoMapper mapper = CreateSimpleAutoMapper();
            var actual = mapper.Map<ScalarSrc, ScalarDst>(expected);

            Assert.IsNotNull(actual);
            Assert.IsNull(expected.PropNullableInterger);
            Assert.IsNull(expected.PropNullableDouble);
            Assert.IsNull(expected.PropNullableString);
        }
        {
            var expected = new ScalarSrc()
            {
                PropNullableInterger = 42,
                PropNullableDouble = Math.PI,
                PropNullableString = Guid.NewGuid().ToString(),
            };
            ISimpleAutoMapper mapper = CreateSimpleAutoMapper();
            var actual = mapper.Map<ScalarSrc, ScalarDst>(expected);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.PropNullableInterger, actual.PropNullableInterger);
            Assert.AreEqual(expected.PropNullableDouble, actual.PropNullableDouble);
            Assert.AreEqual(expected.PropNullableString, actual.PropNullableString);
        }
    }
}