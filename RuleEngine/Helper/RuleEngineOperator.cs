using System.Linq;
using System.Text.RegularExpressions;

namespace RuleEngine.Helper
{
    public static class RuleEngineOperator
    {
        public static bool Equal(object check, object checkAgainst) =>
       check != null && checkAgainst != null && check.ToString() == checkAgainst.ToString();
        public static bool NotEqual(object check, object checkAgainst) =>
        check != null && checkAgainst != null && check.ToString() != checkAgainst.ToString();

        public static bool GreaterThen(object check, object checkAgainst)
        {
            long lCheck, lcheckAgainst;
            if (check == null || checkAgainst == null)
                return false;
            bool success = long.TryParse(check.ToString(), out lCheck);
            success = long.TryParse(checkAgainst.ToString(), out lcheckAgainst);
            return lCheck > lcheckAgainst;

        }
        public static bool LessThen(object check, object checkAgainst)
        {
            long lCheck, lcheckAgainst;
            if (check == null || checkAgainst == null)
                return false;
            bool success = long.TryParse(check.ToString(), out lCheck);
            success = long.TryParse(checkAgainst.ToString(), out lcheckAgainst);
            return lCheck < lcheckAgainst;

        }
        public static bool Like(object check, object checkAgainst) =>
        check != null && checkAgainst != null && check.ToString().Contains(checkAgainst.ToString());
        public static bool NotLike(object check, object checkAgainst) =>
        check != null && checkAgainst != null && !check.ToString().Contains(checkAgainst.ToString());

        public static bool Contain(object check, object checkAgainst)
        {
            if (check == null || checkAgainst == null)
                return false;

            var lCheck = check.ToString().Split(',').ToList().Select(t => t.Trim());
            var lCheckAgainst = checkAgainst.ToString().Split(',').ToList().Select(t => t.Trim());
            return lCheck.Select(t => lCheckAgainst.Contains(t)).Any();
        }
        public static bool NotContain(string check, string checkAgainst) =>
            !Contain(check, checkAgainst);

        public static bool StartsWith(object check, object checkAgainst) =>
        check != null && checkAgainst != null && check.ToString().Trim().StartsWith(checkAgainst.ToString().Trim());
        public static bool EndsWith(object check, object checkAgainst) =>
        check != null && checkAgainst != null && !check.ToString().Trim().EndsWith(checkAgainst.ToString().Trim());

        public static bool RegularExpression(object check, object checkAgainst)
        {
            if (check == null || checkAgainst == null)
                return false;
            Regex re = new Regex(checkAgainst.ToString());
            return re.IsMatch(check.ToString());
        }


    }
}