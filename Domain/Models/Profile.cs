using System.ComponentModel.DataAnnotations;

namespace HngStageOne.Domain.Models;

public sealed class Profile
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Gender { get; set; } = null!;

    public double GenderProbability { get; set; }

    public int SampleSize { get; set; }

    public int Age { get; set; }

    public string AgeGroup { get; set; } = null!;

    public string CountryId { get; set; } = null!;

    public double CountryProbability { get; set; }

    public DateTime CreatedAt { get; set; }
}
