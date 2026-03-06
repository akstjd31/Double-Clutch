using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// ~을 {value}% 만큼 증가시킵니다 같은 문구에서 value부분을 추출하기
/// </summary>
public static class TextParser
{
    public static List<string> GetKeys(string text)
    {
        List<string> keys = new List<string>();

        var matches = Regex.Matches(text, @"\{(.*?)\}");

        foreach (Match match in matches)
        {
            keys.Add(match.Groups[1].Value);
        }

        return keys;
    }
}