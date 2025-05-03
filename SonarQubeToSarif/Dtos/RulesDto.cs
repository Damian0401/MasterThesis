namespace SonarQubeToSarif.Dtos;

internal class RulesDto
{
    public required RuleDto Rule { get; set; }
    public required List<object> Actives { get; set; }

    public record RuleDto
    {
        public required string Key { get; set; }
        public required string Repo { get; set; }
        public required string Name { get; set; }
        public required string CreatedAt { get; set; }
        public required string Severity { get; set; }
        public required string Status { get; set; }
        public required bool IsTemplate { get; set; }
        public required List<object> Tags { get; set; }
        public required List<string> SysTags { get; set; }
        public string? Lang { get; set; }
        public string? LangName { get; set; }
        public required List<Param> Params { get; set; }
        public string? DefaultDebtRemFnType { get; set; }
        public string? DebtRemFnType { get; set; }
        public required string Type { get; set; }
        public string? DefaultRemFnType { get; set; }
        public string? DefaultRemFnBaseEffort { get; set; }
        public string? RemFnType { get; set; }
        public string? RemFnBaseEffort { get; set; }
        public required bool RemFnOverloaded { get; set; }
        public required string Scope { get; set; }
        public required bool IsExternal { get; set; }
        public required List<DescriptionSectionDto> DescriptionSections { get; set; }
        public required List<object> EducationPrinciples { get; set; }
        public required string UpdatedAt { get; set; }
        public required List<object> Impacts { get; set; }
    }

    public record DescriptionSectionDto
    {
        public required string Key { get; set; }
        public required string Content { get; set; }
    }

    public record Param
    {
        public required string Key { get; set; }
        public required string HtmlDesc { get; set; }
        public required string DefaultValue { get; set; }
        public required string Type { get; set; }
    }
}