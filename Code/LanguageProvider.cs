using System.Collections.Generic;

public class LanguageProvider
{
    public List<string> CreateLanguageCollectionFromString(string languageString)
    {
        List<string> languageCollection;

        if (languageString == "")
        {
            languageCollection = new List<string>() { "English" };
            return languageCollection;
        }

        var languageArray = languageString.Split(',');

        languageCollection = new List<string>();

        foreach (var lang in languageArray)
        {
            languageCollection.Add(lang);
        }

        return languageCollection;

    }
}
