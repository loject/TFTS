using NPOI.SS.UserModel;
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
            /* TODO: beatify this crutch? */
            if (viewModel.Races.Count == 0)
            {
                this.HistoryPageViewNameScrollView.Content = new Label
                {
                    Text = "История пуста",
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                };
            }
        }
    }
}