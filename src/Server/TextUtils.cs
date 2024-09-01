using Ruby.Commands;

namespace Ruby.Server;

public static class TextUtils
{
    const int Minutes = 60;
    const int Hours = 60 * 60;
    const int Days = 60 * 60 * 24;
    const int Months = 60 * 60 * 24 * 30;
    const int Years = 60 * 60 * 24 * 365;

    public static int ParseToSeconds(string from)
    {
        ReadOnlySpan<char> chars = from;

        int time = 0;

        string numbers = "";
        foreach (char c in chars)
        {
            if (char.IsDigit(c))
            {
                numbers += c;
                continue;
            }

            switch (c)
            {
                case 's': 
                    time += Convert.ToInt32(numbers); 
                    break;
                case 'm': 
                    time += Convert.ToInt32(numbers) * Minutes; 
                    break;
                case 'h': 
                    time += Convert.ToInt32(numbers) * Hours; 
                    break;
                case 'd': 
                    time += Convert.ToInt32(numbers) * Days; 
                    break;
                case 'M': 
                    time += Convert.ToInt32(numbers) * Months; 
                    break;
                case 'y': 
                    time += Convert.ToInt32(numbers) * Years; 
                    break;
            }
            
            numbers = ""; 
        }

        return time;
    }

    public static void SendList(ICommandSender sender, List<string> items, int page, string headerFormat, string nextPageFormat)
    {
        List<string> lines = new List<string>() { "" };
        int maxLength = 80;
        int curIndex = 0;

        for (int i = 0; i < items.Count; i++)
        {
            string curText = lines[curIndex];
            string curItem = items[i] + (i == items.Count - 1 ? "" : ", ");

            if ((curText + curItem).Length > maxLength)
            {
                lines.Add(curItem);
                curIndex++;
            }
            else
            {
                lines[curIndex] += curItem;
            }
        }
        
        SendPage(sender, headerFormat, lines, page, nextPageFormat);
    }

    public static void SendPage(ICommandSender sender, string headerFormat, List<string> lines, int page, string? nextPageFormat = null)
    {
        // pages calculation
        var currentPage = Math.Max(1, page);
        var nextPage = currentPage + 1;
        int items;
        int maxPage;
        CalculatePages(lines.GetEnumerator(), out items, out maxPage);

        // header
        sender.SendSuccessMessage(string.Format(headerFormat, currentPage, maxPage, nextPage));


        var fixedPageOffset = 5 * (currentPage - 1);
        var maxPageItems = Math.Min(5 * currentPage, items);
        // items
        for (var i = fixedPageOffset; i < maxPageItems; i++) 
            sender.SendBasicMessage(lines[i]);

        // footers 
        if (nextPageFormat != null && nextPage <= maxPage)
            sender.SendInfoMessage(string.Format(nextPageFormat, currentPage, maxPage, nextPage));
    }

    internal static void CalculatePages(IEnumerator<string> enumerator, out int items, out int maxPage)
    {
        maxPage = 0;
        items = 0;

        while (enumerator.MoveNext())
        {
            if (items % 5 == 0)
                maxPage++;

            items++;
        }
    }
}