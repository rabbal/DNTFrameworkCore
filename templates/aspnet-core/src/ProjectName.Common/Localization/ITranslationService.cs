using DNTFrameworkCore.Dependency;

namespace ProjectName.Common.Localization
{
    public interface ITranslationService : ISingletonDependency
    {
        string this[string index] { get; }
        string Translate(string key, params object[] arguments);
        string Translate(string key);
    }
}