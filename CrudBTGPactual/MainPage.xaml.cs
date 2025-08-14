using CrudBTGPactual.ViewModel;

namespace CrudBTGPactual
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }


    }
}
