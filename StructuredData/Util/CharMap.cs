﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;

namespace Reductech.EDR.Connectors.StructuredData.Util
{

/// <summary>
/// Maps string streams to characters
/// </summary>
public record CharMap : IStepValueMap<StringStream, char?>
{
    /// <summary>
    /// The name of the Step Property
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// The location of the error
    /// </summary>
    public ErrorLocation ErrorLocation { get; }

    /// <summary>
    /// Create a new CharMap
    /// </summary>
    public CharMap(string propertyName, ErrorLocation errorLocation)
    {
        PropertyName  = propertyName;
        ErrorLocation = errorLocation;
    }

    /// <inheritdoc />
    public async Task<Result<char?, IError>> Map(
        StringStream t,
        CancellationToken cancellationToken)
    {
        var stringResult = await t.GetStringAsync();

        char? resultChar;

        if (stringResult.Length == 0)
            resultChar = null;
        else if (stringResult.Length == 1)
            resultChar = stringResult.Single();
        else
            return new SingleError(
                ErrorLocation,
                ErrorCode.SingleCharacterExpected,
                PropertyName,
                stringResult
            );

        return resultChar;
    }
}

}