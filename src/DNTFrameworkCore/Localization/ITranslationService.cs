namespace DNTFrameworkCore.Localization
{
    //Under development
    public interface ITranslationService
    {
        string this[string index] { get; }

        string Translate(string name, params object[] arguments);
        string Translate(string name);
    }
}