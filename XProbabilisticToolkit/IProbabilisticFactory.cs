namespace XProbabilisticToolkit;

public interface IProbabilisticFactory
{
    ICuckooFilter CreateCuckooFilter(string filterName);
    IBloomFilter CreateBloomFilter(string filterName);
    ISimpleHyperLogLog CreateSimpleHyperLogLog(string hyperLogLogKey);
}