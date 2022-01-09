using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecipeApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public ListView ListView { get; private set; }

        public MenuPage()
        {
            List<FlyoutPageItem> flyoutPageItems = new List<FlyoutPageItem>();
            flyoutPageItems.Add(new FlyoutPageItem
            {
                Title = "Keresés",
                IconSource = "",
                TargetPage = typeof(RecipeListPage)
            });
            flyoutPageItems.Add(new FlyoutPageItem
            {
                Title = "Keresés alapanyag alapján",
                IconSource = "",
                TargetPage = typeof(IngredientPage)
            });
            flyoutPageItems.Add(new FlyoutPageItem
            {
                Title = "Új recept",
                IconSource = "",
                TargetPage = typeof(NewRecipePage)
            });

            _listView = new ListView
            {
                ItemsSource = flyoutPageItems,
                ItemTemplate = new DataTemplate(() =>
                {
                    Grid grid = new Grid { Padding = new Thickness(5, 10) };
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

                    Image image = new Image();
                    image.SetBinding(Image.SourceProperty, "IconSource");
                    Label label = new Label { VerticalOptions = LayoutOptions.FillAndExpand };
                    label.SetBinding(Label.TextProperty, "Title");

                    grid.Children.Add(image);
                    grid.Children.Add(label, 1, 0);

                    return new ViewCell { View = grid };
                }),
                SeparatorVisibility = SeparatorVisibility.None
            };

            IconImageSource = "";
            Title = "Receptkönyv";
            Padding = new Thickness(0, 40, 0, 0);
            Content = new StackLayout
            {
                Children = { _listView }
            };
        }

    }
}