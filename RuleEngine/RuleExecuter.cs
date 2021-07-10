
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
    public sealed class RuleExecuter : IRuleExecuter
    {
        private const string ruleEngineOperator = "RuleEngineOperator";

        private RuleViewModal RulesCollection { get; set; }

        public RuleExecuter(RuleViewModal rulesCollection)
        {
            RulesCollection = rulesCollection;
        }
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
                dynamic input1 = JsonConvert.DeserializeObject<ExpandoObject>(data.ToLower(), converter);
                var inputs = new dynamic[]
                    {
                    input1
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
                if (logger != null)
                    logger.LogError(exception.Message);
                IsSuccess = false;
                return output;
            }
        }



        private WorkflowRules[] CreateRules(RuleViewModal rules)
        {

            var orderedEnumerable = ((rules.RuleGroups
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

                                                         }))
                                                         .GroupBy(t => new { t.RuleGroupName, t.RuleSetName, t.SuccessEvent, t.ErrorMessage, t.ErrorType, t.Priority })
                                                         .Select(x => new
                                                         {
                                                             RuleGroupName = x.Key.RuleGroupName,
                                                             RuleSetName = x.Key.RuleSetName,
                                                             SuccessEvent = x.Key.SuccessEvent,
                                                             ErrorMessage = x.Key.ErrorMessage,
                                                             ErrorType = x.Key.ErrorType,
                                                             Priority = x.Key.Priority,
                                                             Expression = string.Join(" ", x.Select(m => m.Expression))
                                                         }).OrderBy(m => m.RuleGroupName).ThenBy(m => m.Priority).ToList())
                                                         .GroupBy(t => t.RuleGroupName)
                                                         .Select(t => new
                                                         {
                                                             WorkflowName = t.Key,
                                                             Rules = t.Select(x => new
                                                             {
                                                                 RuleName = x.RuleSetName,
                                                                 SuccessEvent = x.SuccessEvent,
                                                                 ErrorMessage = x.ErrorMessage,
                                                                 ErrorType = x.ErrorType,
                                                                 Expression = x.Expression
                                                             }).ToList()
                                                         }).ToList().OrderBy(t => t.WorkflowName);

            return JsonConvert.DeserializeObject<List<WorkflowRules>>
            (JsonConvert.SerializeObject(orderedEnumerable)
            ).ToArray();
        }

        private string GetExpression(string Parameter, string RelationalOperator, string Value, string LogicalOperator)
        {

            switch (RelationalOperator.ToLower())
            {
                case "(":
                    return "(";
                case ")":
                    return ") " + LogicalOperator;
                case "=":
                    return ruleEngineOperator + ".EqualCheck(" + Parameter.ToLower() + " , \"" + Value.ToLower() + "\") " + LogicalOperator;
                case "<>":
                    return ruleEngineOperator + ".NotEqualCheck(" + Parameter.ToLower() + " ,\"" + Value.ToLower() + "\") " + LogicalOperator;
                case ">":
                    return ruleEngineOperator + ".GreaterThenCheck(" + Parameter.ToLower() + " , \"" + Value.ToLower() + "\") " + LogicalOperator;
                case "<":
                    return ruleEngineOperator + ".LessThenCheck(" + Parameter.ToLower() + " , \"" + Value.ToLower() + "\") " + LogicalOperator;
                case "%":
                    return ruleEngineOperator + ".LikeCheck(" + Parameter.ToLower() + " , \"" + Value.ToLower() + "\") " + LogicalOperator;
                case "!%":
                    return ruleEngineOperator + ".NotLikeCheck(" + Parameter.ToLower() + " ,\"" + Value.ToLower() + "\") " + LogicalOperator;
                case "in":
                    return ruleEngineOperator + ".ContainCheck(" + Parameter.ToLower() + " , \"" + Value.ToLower() + "\") " + LogicalOperator;
                case "notin":
                    return ruleEngineOperator + ".NotContainCheck(" + Parameter.ToLower() + " , \"" + Value.ToLower() + "\") " + LogicalOperator;
                case "starts":
                    return ruleEngineOperator + ".StartsWithCheck(" + Parameter.ToLower() + " , \"" + Value.ToLower() + "\") " + LogicalOperator;
                case "ends":
                    return ruleEngineOperator + ".EndsWithCheck(" + Parameter.ToLower() + " , \"" + Value.ToLower() + "\") " + LogicalOperator;
                case "regex":
                    return ruleEngineOperator + ".RegularExpressionCheck(" + Parameter.ToLower() + " , \"" + Value.ToLower() + "\") " + LogicalOperator;
                default:
                    return string.Concat(Parameter, " ", RelationalOperator, " ", Value, " ", LogicalOperator);
            }

        }


    }
}
