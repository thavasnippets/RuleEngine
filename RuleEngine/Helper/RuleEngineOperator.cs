using System.Linq;
using System.Text.RegularExpressions;

namespace RuleEngine.Helper
{
    public static class RuleEngineOperator
    {
        public static bool EqualCheck(object check, object checkAgainst)
        {
            return check != null && checkAgainst != null && check.ToString() == checkAgainst.ToString();
        }
        public static bool NotEqualCheck(object check, object checkAgainst)
        {
            return check != null && checkAgainst != null && check.ToString() != checkAgainst.ToString();
        }

        public static bool GreaterThenCheck(object check, object checkAgainst)
        {
            long lCheck, lcheckAgainst;
            if (check == null || checkAgainst == null)
                return false;
            bool success = long.TryParse(check.ToString(), out lCheck);
            success = long.TryParse(checkAgainst.ToString(), out lcheckAgainst);
            return success && lCheck > lcheckAgainst;

        }
        public static bool LessThenCheck(object check, object checkAgainst)
        {
            long lCheck, lcheckAgainst;
            if (check == null || checkAgainst == null)
                return false;
            bool success = long.TryParse(check.ToString(), out lCheck);
            success = long.TryParse(checkAgainst.ToString(), out lcheckAgainst);
            return success && lCheck < lcheckAgainst;

        }
        public static bool LikeCheck(object check, object checkAgainst)
        {
            return check != null && checkAgainst != null && check.ToString().Contains(checkAgainst.ToString());
        }
        public static bool NotLikeCheck(object check, object checkAgainst)
        {
            return check != null && checkAgainst != null && !check.ToString().Contains(checkAgainst.ToString());
        }

        public static bool ContainCheck(object check, object checkAgainst)
        {
            if (check == null || checkAgainst == null)
                return false;

            var lCheck = check.ToString().Split(',').ToList().Select(t => t.Trim());
            var lCheckAgainst = checkAgainst.ToString().Split(',').ToList().Select(t => t.Trim());
            return lCheck.Select(t => lCheckAgainst.Contains(t)).ToList()[0];
        }
        public static bool NotContainCheck(string check, string checkAgainst)
        {
            return !ContainCheck(check, checkAgainst);
        }

        public static bool StartsWithCheck(object check, object checkAgainst)
        {
            return check != null && checkAgainst != null && check.ToString().Trim().StartsWith(checkAgainst.ToString().Trim());
        }
        public static bool EndsWithCheck(object check, object checkAgainst)
        {
            return check != null && checkAgainst != null && !check.ToString().Trim().EndsWith(checkAgainst.ToString().Trim());
        }

        public static bool RegularExpression(object check, object checkAgainst)
        {
            if (check == null || checkAgainst == null)
                return false;
            Regex re = new Regex(checkAgainst.ToString());
            return re.IsMatch(check.ToString());
        }


    }
}