namespace DnDFightTool.Infrastructure.AspNetCoreExtensions.Navigations;

public interface IStateFullNavigation
{
    void NavigateTo(string page);
    void NavigateBack();
}
