
namespace RuleEngine
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using RuleEngine.Helper;
    using RuleEngine.Model;
    using RulesEngine.Extensions;
    using RulesEngine.Models;
    public class RuleExecuter : IRuleExecuter
    {
        private const string ruleEngineOperator = "RuleEngineOperator";

        public RuleViewModal RulesCollection { get; set; }
        public string ExecuteRules(string workflowName, string data, out bool IsSuccess, ILogger logger = null)
        {
            string output = string.Empty;
            bool status = false;
            try
            {
                var rule = new RulesEngine.RulesEngine(workflowRules: CreateRules(RulesCollection),
                                                                      logger: logger,
                                                                      reSettings: new ReSettings
                                                                      {
                                                                          CustomTypes = new Type[] { typeof(RuleEngineOperator) }
                                                                      });

                var converter = new ExpandoObjectConverter();
                dynamic input = JsonConvert.DeserializeObject<ExpandoObject>(data, converter);
                var inputs = new dynamic[]
                    {
                    input
                    };

                List<RuleResultTree> resultList = rule.ExecuteAllRulesAsync(workflowName, inputs).Result;

                resultList.OnSuccess((eventName) =>
                {
                    output = eventName;
                    status = true;
                });

                resultList.OnFail(() =>
                {
                    output = "NONE";
                    status = false;
                });
                IsSuccess = status;
                return output;
            }
            catch (Exception exception)
            {
                logger.LogError(exception.Message);
                IsSuccess = false;
                return output;
            }
        }



        private WorkflowRules[] CreateRules(RuleViewModal rules) =>
        JsonConvert.DeserializeObject<List<WorkflowRules>>
                 (JsonConvert.SerializeObject(
                                                rules.RuleGroups
                                                .Join(rules.Rulesets, rg => rg.Id, rs => rs.RuleGroupId, (rg, rs) => new { rg, rs })
                                                .Join(rules.Rules, _rs => _rs.rs.Id, rl => rl.RulesetId, (_rs, rl) => new { _rs, rl })
                                                .Select(t => new
                                                {
                                                    RuleGroupName = t._rs.rg.Name,
                                                    RuleSetName = t._rs.rs.Name,
                                                    SuccessEvent = t._rs.rs.SuccessEvent,
                                                    ErrorMessage = t._rs.rs.ErrorMessage,
                                                    ErrorType = t._rs.rs.ErrorType,
                                                    RuleExpressionType = t._rs.rs.RuleExpressionType,
                                                    Priority = t._rs.rs.Priority,
                                                    Expression = GetExpression(t.rl.Parameter,
                                                                t.rl.RelationalOperator,
                                                                t.rl.Value,
                                                                t.rl.LogicalOperator)

                                                })
                                                .GroupBy(t => t.RuleGroupName)
                                                .Select(x => new
                                                {
                                                    WorkflowName = x.Key,
                                                    Rules = x.Select(t => new
                                                    {
                                                        t.RuleSetName,
                                                        t.SuccessEvent,
                                                        t.ErrorMessage,
                                                        t.ErrorType,
                                                        t.RuleExpressionType,
                                                        t.Expression,
                                                        t.Priority
                                                    }).ToList().OrderBy(o => o.Priority)
                                                }).OrderBy(t => t.WorkflowName)
                                            )
                    ).ToArray();

        private string GetExpression(string Parameter, string RelationalOperator, string Value, string LogicalOperator)
        {

            switch (RelationalOperator)
            {
                case "=":
                    return ruleEngineOperator + ".Equals(input." + Parameter.ToLower() + " , \"" + Value.ToLower() + "\") " + LogicalOperator;
                case "<>":
                    return ruleEngineOperator + ".NotEquals(input." + Parameter.ToLower() + " ,\"" + Value.ToLower() + "\") " + LogicalOperator;
                case ">":
                    return ruleEngineOperator + ".GreaterThen(input." + Parameter.ToLower() + " , \"" + Value.ToLower() + "\") " + LogicalOperator;
                case "<":
                    return ruleEngineOperator + ".LessThen(input." + Parameter.ToLower() + " , \"" + Value.ToLower() + "\") " + LogicalOperator;
                case "%":
                    return ruleEngineOperator + ".Like(input." + Parameter.ToLower() + " , \"" + Value.ToLower() + "\") " + LogicalOperator;
                case "!%":
                    return ruleEngineOperator + ".NotLike(input." + Parameter.ToLower() + " ,\"" + Value.ToLower() + "\") " + LogicalOperator;
                case "In":
                    return ruleEngineOperator + ".Contain(input." + Parameter.ToLower() + " , \"" + Value.ToLower() + "\") " + LogicalOperator;
                case "NotIn":
                    return ruleEngineOperator + ".NotContain(input." + Parameter.ToLower() + " , \"" + Value.ToLower() + "\") " + LogicalOperator;
                case "Starts":
                    return ruleEngineOperator + ".StartsWith(input." + Parameter.ToLower() + " , \"" + Value.ToLower() + "\") " + LogicalOperator;
                case "Ends":
                    return ruleEngineOperator + ".EndsWith(input." + Parameter.ToLower() + " , \"" + Value.ToLower() + "\") " + LogicalOperator;
                case "Regex":
                    return ruleEngineOperator + ".RegularExpression(input." + Parameter.ToLower() + " , \"" + Value.ToLower() + "\") " + LogicalOperator;
                default:
                    return string.Concat(Parameter, " ", RelationalOperator, " ", Value, " ", LogicalOperator);
            }

        }
    }
}
