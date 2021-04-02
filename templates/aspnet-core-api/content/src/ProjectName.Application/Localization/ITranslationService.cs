using DNTFrameworkCore.Dependency;

namespace ProjectName.Application.Localization
{
    public interface ITranslationService : ISingletonDependency
    {
        string this[string index] { get; }
        string Translate(string key, params object[] arguments);
        string Translate(string key);
    }

    public class NullTranslationService : ITranslationService
    {
        public static readonly ITranslationService Instance = new NullTranslationService();
        public string this[string index] => index;
        public string Translate(string key, params object[] arguments) => key;
        public string Translate(string key) => key;
    }
}