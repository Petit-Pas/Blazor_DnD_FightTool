namespace AspNetCoreExtensions.Navigations;

public interface IStateFullNavigation
{
    public void NavigateTo(string page);
    public void NavigateBack();
}
