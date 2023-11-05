namespace SimpleAutoMapper.Test;

[TestClass]
public class SimpleAutoMapperExtensionsTest
{
    [TestMethod]
    public void GetTypeMapping_TSrc_TDst()
    {
        Func<IMappingContext, ScalarSrc?, ScalarDst?> expectedMapper = (_, _) => null!;
        var mapper = SimpleAutoMapperTest.From(expectedMapper);

        var expectedTypeMapping = mapper.GetTypeMapping(typeof(ScalarSrc), typeof(ScalarDst));
        var actualTypeMapping_TSrc_TDst = mapper.GetTypeMapping<ScalarSrc, ScalarDst>();

        Assert.AreSame(expectedTypeMapping, actualTypeMapping_TSrc_TDst);
    }

    [TestMethod]
    public void GetMapper_TSrc_TDst()
    {
        var expectedValue = new ScalarDst();
        Func<IMappingContext, ScalarSrc?, ScalarDst?> sourceMapper = (_, _) => expectedValue;
        var mapper = SimpleAutoMapperTest.From(sourceMapper);

        var expectedMapper = (Func<ScalarSrc?, ScalarDst?>)mapper.GetMapper(typeof(ScalarSrc), typeof(ScalarDst));
        var actualMapper_TSrc_TDst = mapper.GetMapper<ScalarSrc, ScalarDst>();

        Assert.AreSame(sourceMapper(null!, null!), expectedMapper(null!));
        Assert.AreSame(expectedMapper(null!), actualMapper_TSrc_TDst(null!));
    }

    [TestMethod]
    public void Map_TSrc_TDst()
    {
        ScalarSrc expectedSrc = new() { MemberInterger = 42 };
        ScalarSrc actualSrc = default!;
        ScalarDst expectedDst = default!;

        var mapper = SimpleAutoMapperTest.From<ScalarSrc, ScalarDst>((_, src) =>
        {
            actualSrc = src!;
            expectedDst = new ScalarDst() { MemberInterger = src!.MemberInterger };
            return expectedDst;
        });

        var actualDst = mapper.Map<ScalarSrc, ScalarDst>(expectedSrc);
        Assert.AreSame(expectedSrc, actualSrc);
        Assert.AreSame(expectedDst, actualDst);

        //=========================================

        mapper = SimpleAutoMapperTest.From<ScalarSrc, ScalarDst>((_, src) => throw new AssertFailedException());
        Assert.ThrowsException<AssertFailedException>(() => mapper.Map<ScalarSrc, ScalarDst>(expectedSrc));
    }

    [TestMethod]
    public void MapCollection_TSrc_TDst()
    {
        var nbElements = 42;
        List<ScalarSrc> expectedSrcList = Enumerable.Range(42, nbElements).Select(i => new ScalarSrc() { MemberInterger = i }).ToList();
        List<ScalarSrc> actualSrcList = new(nbElements);
        List<object?> expectedDstList = new(nbElements);

        var mapper = SimpleAutoMapperTest.From<ScalarSrc, ScalarDst>((_, src) =>
        {
            actualSrcList.Add(src!);
            var expectedDst = new ScalarDst() { MemberInterger = src!.MemberInterger };
            expectedDstList.Add(expectedDst);
            return expectedDst;
        });

        var actualDstList = mapper.MapCollection<ScalarSrc, ScalarDst>(expectedSrcList);
        Assert.IsTrue(Enumerable.SequenceEqual(expectedSrcList, actualSrcList));
        Assert.IsNotNull(actualDstList);
        Assert.IsTrue(Enumerable.SequenceEqual(expectedDstList, actualDstList));

        //=========================================

        mapper = SimpleAutoMapperTest.From<ScalarSrc, ScalarDst>((_, src) => throw new AssertFailedException());
        Assert.ThrowsException<AssertFailedException>(() => mapper.MapCollection<ScalarSrc, ScalarDst>(expectedSrcList));
    }

    [TestMethod]
    public void MapTo_TDst()
    {
        ScalarSrc expectedSrc = new() { MemberInterger = 42 };
        ScalarSrc actualSrc = default!;
        ScalarDst expectedDst = default!;

        var mapper = SimpleAutoMapperTest.From<ScalarSrc, ScalarDst>((_, src) =>
        {
            actualSrc = src!;
            expectedDst = new ScalarDst() { MemberInterger = src!.MemberInterger };
            return expectedDst;
        });

        var actualDst = mapper.MapTo<ScalarDst>(expectedSrc);
        Assert.AreSame(expectedSrc, actualSrc);
        Assert.AreSame(expectedDst, actualDst);

        Assert.ThrowsException<ArgumentNullException>(() => mapper.MapTo<ScalarDst>(null!));

        //=========================================

        mapper = SimpleAutoMapperTest.From<ScalarSrc, ScalarDst>((_, src) => throw new AssertFailedException());
        Assert.ThrowsException<AssertFailedException>(() => mapper.MapTo<ScalarDst>(expectedSrc));
    }

    [TestMethod]
    public void MapCollectionTo_TDst()
    {
        var nbElements = 42;
        List<ScalarSrc> expectedSrcList = Enumerable.Range(42, nbElements).Select(i => new ScalarSrc() { MemberInterger = i }).ToList();
        List<ScalarSrc> actualSrcList = new(nbElements);
        List<object?> expectedDstList = new(nbElements);

        var mapper = SimpleAutoMapperTest.From<ScalarSrc, ScalarDst>((_, src) =>
        {
            actualSrcList.Add(src!);
            var expectedDst = new ScalarDst() { MemberInterger = src!.MemberInterger };
            expectedDstList.Add(expectedDst);
            return expectedDst;
        });

        var actualDstList = mapper.MapCollectionTo<ScalarDst>(expectedSrcList);
        Assert.IsTrue(Enumerable.SequenceEqual(expectedSrcList, actualSrcList));
        Assert.IsNotNull(actualDstList);
        Assert.IsTrue(Enumerable.SequenceEqual(expectedDstList, actualDstList));

        Assert.ThrowsException<ArgumentNullException>(() => mapper.MapCollectionTo<ScalarDst>(null!));

        //=========================================

        mapper = SimpleAutoMapperTest.From<ScalarSrc, ScalarDst>((_, src) => throw new AssertFailedException());
        Assert.ThrowsException<AssertFailedException>(() => mapper.MapCollectionTo<ScalarDst>(expectedSrcList));

    }


    [TestMethod]
    public void MapFrom_TSrc()
    {
        ScalarSrc expectedSrc = new() { MemberInterger = 42 };
        ScalarSrc actualSrc = default!;
        ScalarDst expectedDst = default!;

        var mapper = SimpleAutoMapperTest.From<ScalarSrc, ScalarDst>((_, src) =>
        {
            actualSrc = src!;
            expectedDst = new ScalarDst() { MemberInterger = src!.MemberInterger };
            return expectedDst;
        });

        var actualDst = mapper.MapFrom(expectedSrc).To<ScalarDst>();
        Assert.AreSame(expectedSrc, actualSrc);
        Assert.AreSame(expectedDst, actualDst);

        //=========================================

        mapper = SimpleAutoMapperTest.From<ScalarSrc, ScalarDst>((_, src) => throw new AssertFailedException());
        Assert.ThrowsException<AssertFailedException>(() => mapper.MapFrom(expectedSrc).To<ScalarDst>());
    }

    [TestMethod]
    public void MapFromCollection_TSrc()
    {
        var nbElements = 42;
        List<ScalarSrc> expectedSrcList = Enumerable.Range(42, nbElements).Select(i => new ScalarSrc() { MemberInterger = i }).ToList();
        List<ScalarSrc> actualSrcList = new(nbElements);
        List<object?> expectedDstList = new(nbElements);

        var mapper = SimpleAutoMapperTest.From<ScalarSrc, ScalarDst>((_, src) =>
        {
            actualSrcList.Add(src!);
            var expectedDst = new ScalarDst() { MemberInterger = src!.MemberInterger };
            expectedDstList.Add(expectedDst);
            return expectedDst;
        });

        var actualDstList = mapper.MapFromCollection(expectedSrcList).To<ScalarDst>();
        Assert.IsTrue(Enumerable.SequenceEqual(expectedSrcList, actualSrcList));
        Assert.IsNotNull(actualDstList);
        Assert.IsTrue(Enumerable.SequenceEqual(expectedDstList, actualDstList));

        //=========================================

        mapper = SimpleAutoMapperTest.From<ScalarSrc, ScalarDst>((_, src) => throw new AssertFailedException());
        Assert.ThrowsException<AssertFailedException>(() => mapper.MapFromCollection(expectedSrcList).To<ScalarDst>());
    }

    private class SimpleAutoMapperTest : ISimpleAutoMapper
    {
        public static ISimpleAutoMapper From<TSrc, TDst>(Func<IMappingContext, TSrc?, TDst?> customMapper)
            where TSrc : class
            where TDst : class
            => new SimpleAutoMapperTest() { _typeMapping = new TypeMappingTest<TSrc, TDst>(customMapper) };

        private ITypeMapping _typeMapping = null!;
        private SimpleAutoMapperTest() { }

        ITypeMapping ISimpleAutoMapper.GetTypeMapping(Type sourceType, Type destinationType)
            => this._typeMapping;

        Delegate ISimpleAutoMapper.GetMapper(Type sourceType, Type destinationType)
            => this._typeMapping.GetMapper(null!);
    }

    private class TypeMappingTest<TSrc, TDst> : ITypeMapping<TSrc, TDst>
        where TSrc : class
        where TDst : class
    {
        public TypeMappingTest(Func<IMappingContext, TSrc?, TDst?> mapper)
            => this.Mapper = mapper;

        public Type SourceType => typeof(TSrc);
        public Type DestinationType => typeof(TDst);
        public TypeMappingState CurrentState => TypeMappingState.Optimized;
        public Func<IMappingContext, TSrc?, TDst?> Mapper { get; }
        Delegate ITypeMapping.GetMapper(IMappingContext context) => this.GetMapper(context);

        public Func<TSrc?, TDst?> GetMapper(IMappingContext context)
            => (src) => this.Mapper(context, src);

        public ITypeMapping WhenReady() => this;
    }
}
