using TFTS.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TFTS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPageView : ContentPage
    {
        public HistoryPageView(HistoryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}