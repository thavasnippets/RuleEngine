using System.Collections.Generic;

namespace RuleEngine.Model
{
    public class RuleViewModal
    {
        public IEnumerable<RuleGroup> RuleGroups { get; set; }
        public IEnumerable<Ruleset> Rulesets { get; set; }
        public IEnumerable<Rule> Rules { get; set; }
    }
}