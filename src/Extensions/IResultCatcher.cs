namespace Ruby.Extensions;

internal interface IResultCatcher<TReloadable, TResult> where TReloadable : IExtension
{
    public void Catch(TResult result);
    public void Announce();
}