namespace XProbabilisticToolkit;

public interface ICuckooFilterFactory
{
    ICuckooFilter Create(string filterName);
}