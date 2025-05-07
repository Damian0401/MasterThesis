using System.Text.Json.Serialization;

namespace ProjectAnalyzer.Dtos;

internal record SarifDto
{
    [JsonPropertyName("$schema")]
    public required string Schema { get; set; }
    public required string Version { get; set; }
    public required List<RunDto> Runs { get; set; }

    public record ArtifactLocationDto
    {
        public required string Uri { get; set; }
        public required string UriBaseId { get; set; }
    }

    public record DefaultConfigurationDto
    {
        public required SeverityLevel Level { get; set; }
    }

    public record DriverDto
    {
        public required string Name { get; set; }
        public required string SemanticVersion { get; set; }
        public required string Version { get; set; }
        public required string InformationUri { get; set; }
        public required List<RuleDto> Rules { get; set; }
    }

    public record HelpDto
    {
        public required string Text { get; set; }
    }

    public record LocationDto
    {
        public required PhysicalLocationDto PhysicalLocation { get; set; }
    }

    public record MessageDto
    {
        public required string Text { get; set; }
    }

    public record PhysicalLocationDto
    {
        public required ArtifactLocationDto ArtifactLocation { get; set; }
        public required RegionDto Region { get; set; }
    }

    public record PropertiesDto
    {
        public required List<string> Tags { get; set; }
        public required List<string> Categories { get; set; }
    }

    public record RegionDto
    {
        public required int StartLine { get; set; }
        public required int EndLine { get; set; }
        public required int StartColumn { get; set; }
        public required int EndColumn { get; set; }
    }

    public record ResultDto
    {
        public required string RuleId { get; set; }
        public required int RuleIndex { get; set; }
        public required SeverityLevel Level { get; set; }
        public required MessageDto Message { get; set; }
        public required List<LocationDto> Locations { get; set; }
    }

    public record RuleDto
    {
        public required string Id { get; set; }
        public required ShortDescriptionDto ShortDescription { get; set; }
        public required DefaultConfigurationDto DefaultConfiguration { get; set; }
        public required HelpDto Help { get; set; }
        public required PropertiesDto Properties { get; set; }
    }

    public record RunDto
    {
        public required ToolDto Tool { get; set; }
        public required List<ResultDto> Results { get; set; }
    }

    public record ShortDescriptionDto
    {
        public required string Text { get; set; }
    }

    public record ToolDto
    {
        public required DriverDto Driver { get; set; }
    }

    internal enum SeverityLevel
    {
        Error,
        Warning,
        Note,
        None,
    }
}