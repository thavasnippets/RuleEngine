namespace RuleEngine.Model
{
    public class Ruleset
    {
        public long Id { get; set; }
        public long RuleGroupId { get; set; }
        public string Name { get; set; }
        public string SuccessEvent { get; set; } = "SuccessEvent";
        public string ErrorMessage { get; set; } = "One or more adjust rules failed.";
        public string ErrorType { get; set; } = "Error";
        public string RuleExpressionType { get; set; } = "LambdaExpression";
        public long Priority { get; set; }
    }
}