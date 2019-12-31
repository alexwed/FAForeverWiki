using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace FAForeverWikiX
{
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void AeonLoad(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListPage("aeon"));
        }

        async void UEFLoad(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListPage("uef"));
        }

        async void CybranLoad(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListPage("cybran"));
        }

        async void SeraphimLoad(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListPage("seraphim"));
        }
    }
}
