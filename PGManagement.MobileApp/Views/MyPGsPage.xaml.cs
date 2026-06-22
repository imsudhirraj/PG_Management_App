namespace PGManagement.MobileApp.Views;

public partial class MyPGsPage : ContentPage
{
    private readonly ViewModels.MyPGsViewModel _vm;

    public MyPGsPage(ViewModels.MyPGsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadMyPGsCommand.ExecuteAsync(null);
    }

    private async void OnPGSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Models.PGResponse selected) return;
        ((CollectionView)sender).SelectedItem = null;
        await Shell.Current.GoToAsync($"PGDetailPage?id={selected.Id}");
    }
}