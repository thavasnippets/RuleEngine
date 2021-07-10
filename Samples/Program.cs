using System;
using RuleEngine;
using System.Collections.Generic;
using RuleEngine.Model;
using Newtonsoft.Json;

namespace Samples
{
    public class userDetails
    {
        public string Name { get; set; }
        public long LoyalityFactor { get; set; }
        public long TotalPurchases { get; set; }
        public string Country { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {

            // Data needs to be evaluated 
            var user1 = JsonConvert.SerializeObject(new userDetails
            {
                Name = "Arun",
                Country = "India",
                LoyalityFactor = 6,
                TotalPurchases = 1500
            });


            var user2 = JsonConvert.SerializeObject(new userDetails
            {
                Name = "Arun",
                Country = "Srilanka",
                LoyalityFactor = 10,
                TotalPurchases = 15000
            });

            //Initiate the rule executor by passing rules collection
            var ruleExecuter = new RuleExecuter(
                rulesCollection: prepareRules()
            );

            //To execute the rules. By passing the data in the form of json string.
            bool isEligible = false;
            System.Console.WriteLine(user1);
            System.Console.WriteLine("Discount: " + ruleExecuter.ExecuteRules(workflowName: "Discount", data: user1, IsSuccess: out isEligible));
            System.Console.WriteLine("Cashback: " + ruleExecuter.ExecuteRules(workflowName: "Cashback", data: user1, IsSuccess: out isEligible));

            System.Console.WriteLine(user2);
            System.Console.WriteLine("Discount: " + ruleExecuter.ExecuteRules(workflowName: "Discount", data: user2, IsSuccess: out isEligible));
            System.Console.WriteLine("Cashback: " + ruleExecuter.ExecuteRules(workflowName: "Cashback", data: user2, IsSuccess: out isEligible));

            Console.ReadKey();

        }

        private static RuleViewModal prepareRules() =>

            new RuleViewModal
            {

                RuleGroups = new List<RuleGroup>{
                    new RuleGroup{ Id= 1, Name="Discount", Description="Discount"},
                    new RuleGroup{ Id= 2, Name="Cashback", Description="Cashback"}
                },

                Rulesets = new List<Ruleset>{
                    new Ruleset{Id=1,RuleGroupId=1,Name="Discount10",SuccessEvent="10",Priority=5},
                    new Ruleset{Id=2,RuleGroupId=1,Name="Discount20",SuccessEvent="20",Priority=1},
                    new Ruleset{Id=3,RuleGroupId=2,Name="Cashback25",SuccessEvent="25",Priority=5},
                    new Ruleset{Id=4,RuleGroupId=2,Name="Cashback50",SuccessEvent="50",Priority=10}
                },
                Rules = new List<Rule>{
                    new Rule{Id=1,RulesetId=1,Parameter="country",RelationalOperator="in",Value="India,Srilanka,us",LogicalOperator="or"},
                    new Rule{Id=100,RulesetId=1,Parameter="",RelationalOperator="(",Value="",LogicalOperator=""},
                    new Rule{Id=200,RulesetId=1,Parameter="loyalityFactor",RelationalOperator="<",Value="10",LogicalOperator="and"},
                    new Rule{Id=300,RulesetId=1,Parameter="totalPurchases",RelationalOperator="<",Value="15000",LogicalOperator=""},
                    new Rule{Id=400,RulesetId=1,Parameter="",RelationalOperator=")",Value="",LogicalOperator=""},

                    new Rule{Id=4,RulesetId=2,Parameter="country",RelationalOperator="=",Value="India",LogicalOperator="AND"},
                    new Rule{Id=5,RulesetId=2,Parameter="loyalityFactor",RelationalOperator=">",Value="5",LogicalOperator="AND"},
                    new Rule{Id=6,RulesetId=2,Parameter="totalPurchases",RelationalOperator=">",Value="150",LogicalOperator=""},

                    new Rule{Id=3,RulesetId=3,Parameter="country",RelationalOperator="notin",Value="India,Srilanka",LogicalOperator="AND"},
                    new Rule{Id=8,RulesetId=3,Parameter="name",RelationalOperator="starts",Value="A",LogicalOperator="AND"},
                    new Rule{Id=9,RulesetId=3,Parameter="totalPurchases",RelationalOperator=">",Value="1500",LogicalOperator=""},

                    new Rule{Id=10,RulesetId=4,Parameter="country",RelationalOperator="in",Value="India,Srilanka",LogicalOperator="AND"},
                    new Rule{Id=11,RulesetId=4,Parameter="name",RelationalOperator="%",Value="run",LogicalOperator="AND"},
                    new Rule{Id=12,RulesetId=4,Parameter="totalPurchases",RelationalOperator=">",Value="1500",LogicalOperator=""}
                }
            };



    }

}
