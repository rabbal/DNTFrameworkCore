using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Cryptography
{
    public interface IRowIntegrityHashAlgorithm : ITransientDependency
    {
        string HashRow(object row);
        bool VerifyHashedRow(string hashedRow, string providedHash);
    }
}