namespace HngStageOne.Configurations;

public sealed record GenderizeSettings
{
    /// <summary>
    /// Gets the base URL for Genderize API operations.
    /// </summary>
    /// <value>
    /// A string representing the root endpoint for all Genderize service requests.
    /// </value>
    public string BaseUrl { get; init; } = null!;

}

public sealed record AgifySettings
{
    /// <summary>
    /// Gets the base URL for Agify API operations.
    /// </summary>
    /// <value>
    /// A string representing the root endpoint for all Agify service requests.
    /// </value>
    public string BaseUrl { get; init; } = null!;

}

public sealed record NationalizeSettings
{
    /// <summary>
    /// Gets the base URL for Nationalize API operations.
    /// </summary>
    /// <value>
    /// A string representing the root endpoint for all Nationalize service requests.
    /// </value>
    public string BaseUrl { get; init; } = null!;

}