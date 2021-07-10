# **Business RuleEngine**

Business RuleEngine is the wrapper around Microsoft Json based rule engine.

## **Use case:**

The company wanted to give discount or cashback to the customers based on certain criteria. These criteria will get updated very frequently. As a developer we can't create or alter the business logic at every time either in database logic (Stored Proc) or in your middleware's.

This library will abstract business rules out of the system and this library has three major components

- **Rule group** is the top level of browsing
- **Rule set** is the groups of rules together. Single Rulegroup can have N number of Rulesets
- **Rules** is the actual business conditions. Single Ruleset can have N number of Rules

These three components grouped as called **RuleViewModel**

![Rule](https://user-images.githubusercontent.com/15138302/125127915-e4441b00-e11a-11eb-8d98-cbfc87cc7b9e.png)

**RuleGroup:**

| Id  | Name     | Description             |
| :-- | :------- | :---------------------- |
| 1   | Discount | To provide the discount |
| 2   | Cashback | To Provide the cashback |

**RuleSet**:

| Id  | RulegroupId | Name       | SuccessEvent | Priority |
| :-- | :---------- | :--------- | :----------- | :------- |
| 1   | 1           | Discount10 | 10           | 5        |
| 2   | 1           | Discount20 | 20           | 10       |
| 3   | 2           | Cashback25 | CB25         | 10       |
| 4   | 2           | Cashback50 | CB50         | 20       |

**Rules**:

| Id  | RulesetId | Parameter      | RelationalOperator | Value          | LogicalOperator |
| :-- | :-------- | :------------- | :----------------- | :------------- | :-------------- |
| 1   | 1         | country        | IN                 | India,Srilanka | AND             |
| 2   | 1         | loyalityFactor | <                  | 5              | AND             |
| 3   | 1         | totalPurchases | >                  | 1500           |                 |
| 4   | 2         | country        | =                  | India          | AND             |
| 5   | 2         | loyalityFactor | >                  | 5              | AND             |
| 6   | 2         | totalPurchases | >                  | 150            |                 |
| 7   | 3         | country        | notin              | India,Srilanka | AND             |
| 8   | 3         | name           | Starts             | A              | AND             |
| 9   | 3         | totalPurchases | =                  | 999            |                 |
| 10  | 4         | country        | in                 | India,Srilanka | OR              |
| 11  | 4         | name           | %                  | run            | OR              |
| 12  | 4         | totalPurchases | >                  | 1500           |                 |

1. The library will accept the above data in the **RuleViewModel**

2. Initiate the rule executor by passing rules collection.

```c#
    var ruleExecuter = new RuleExecuter(
        rulesCollection: prepareRules()
        );
```

3. Now its time to execute the rules. By passing the data in the form of json String.

```c#
    var result= ruleExecuter.ExecuteRules(
                    workflowName: "Discount",
                    data: user1,
                    IsSuccess: out isEligible)
```

4. The engine will evaluate the inputs against the rule collection.

### Available Operators

| Operator |    Description     |
| :------: | :----------------: |
|    =     |       Equal        |
|    <>    |     Not Equal      |
|    >     |    Greater then    |
|    <     |     Less then      |
|    %     |        Like        |
|    !%    |      Not Like      |
|    in    |   List Contains    |
|  notin   | List Not Contains  |
|  starts  |  Text starts with  |
|   ends   |   Text ends with   |
|  regex   | Regular expression |

The system allows to configure the nested conditions
e.g ((a >b) OR (c>d)) AND (x>y)

Refer the sample app for more details.

## **Happy Coding!**
