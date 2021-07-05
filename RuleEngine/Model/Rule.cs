namespace RuleEngine.Model
{
    public class Rule
    {
        public long Id { get; set; }
        public long RulesetId { get; set; }
        public string Parameter { get; set; }
        public string RelationalOperator { get; set; } = "=";
        public string Value { get; set; }
        public string LogicalOperator { get; set; } = null;
    }
}