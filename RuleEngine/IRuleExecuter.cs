namespace RuleEngine
{
    using Microsoft.Extensions.Logging;
    using RuleEngine.Model;
    public interface IRuleExecuter
    {
        string ExecuteRules(string workflowName,
                     string data,
                     out bool IsSuccess,
                     ILogger logger = null);


    }
}