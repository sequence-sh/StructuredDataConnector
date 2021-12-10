namespace Reductech.EDR.Connectors.StructuredData.Util;

/// <summary>
/// Contains methods to help mapping steps
/// </summary>
public static class StepMaps
{
    /// <summary>
    /// Wrap a StringStream and enforce that it contains zero or one characters
    /// </summary>
    public static IRunnableStep<char?> WrapChar(this IStep<StringStream> step, string name) =>
        step.WrapStep(new CharMap(name, step.TextLocation!));
}
