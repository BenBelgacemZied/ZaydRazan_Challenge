namespace ZRLingoTrip.Application;

/// <summary>Stores the learner's support language. The destination language remains French.</summary>
public interface ILearningLanguageSettings
{
    string SourceLanguageCode { get; set; }
}
