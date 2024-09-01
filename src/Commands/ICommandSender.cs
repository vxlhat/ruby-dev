using Ruby.Permissions;

namespace Ruby.Commands;

public interface ICommandSender : IPermissionable
{
    public void SendBasicMessage(string text);
    public void SendSuccessMessage(string text);
    public void SendInfoMessage(string text);
    public void SendErrorMessage(string text);
    public void SendWarningMessage(string text);
    public void SendList(List<string> items, int page, string headerFormat, string nextPageFormat);
    public void SendPage(List<string> lines, int page, string headerFormat, string nextPageFormat);
}