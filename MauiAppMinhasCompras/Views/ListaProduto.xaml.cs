using MauiAppMinhasCompras.Models;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
	ObservableCollection<Produto> lista = new ObservableCollection<Produto>();
	public ListaProduto()
	{
		InitializeComponent();

		lst_produtos.ItemsSource = lista;
	}

    protected async override void OnAppearing() //
    {
		try
		{
            lista.Clear();
            List<Produto> tmp = await App.Db.GetAll();

            tmp.ForEach(i => lista.Add(i));
        }
		catch (Exception ex)
		{
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
		try
		{
			Navigation.PushAsync(new Views.NovoProduto());
		} catch (Exception ex)
		{
			DisplayAlert("Ops", ex.Message, "OK");
		}
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e) //
    {
        try
        {
            string q = e.NewTextValue;

            lista.Clear();

            List<Produto> tmp = await App.Db.Search(q);

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = true;
        }
    }

    private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        if (lista.Count == 0)
        {
            await DisplayAlert("Total dos Produtos", "Não há produtos para somar", "OK");
            return;
        }
		double soma = lista.Sum(i => i.Total);

		string msg = $"O total é {soma:C}";

		await DisplayAlert("Total dos Produtos", msg, "OK");
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            MenuItem selecionado = sender as MenuItem;

            Produto p = selecionado.BindingContext as Produto;

            bool confirm = await DisplayAlert("Tem certeza?", $"Remover {p.Descricao}?", "Sim", "Não");

            if (confirm)
            {
                await App.Db.Delete(p.Id);
                lista.Remove(p);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            Produto p = e.SelectedItem as Produto;

            Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p,
            });
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        try
        {
            lista.Clear();
            List<Produto> tmp = await App.Db.GetAll();

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private async void ToolbarItem_Clicked_2(object sender, EventArgs e)
    {
        try
        {
            bool confirm = await DisplayAlert("Tem certeza?", $"Limpar a lista?", "Sim", "Não");

            if (confirm)
            {
                await App.Db.DeleteAll();
                lista.Clear();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void pk_categoria_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex > 0)
        {
            try
            {
                string q = (string)picker.SelectedItem;

                lista.Clear();

                List<Produto> tmp = await App.Db.SearchCategory(q);

                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
            finally
            {
                lst_produtos.IsRefreshing = true;
            }
        }
        else
        {
            try
            {
                lista.Clear();
                List<Produto> tmp = await App.Db.GetAll();

                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }
    }

    private async void ToolbarItem_Clicked_3(object sender, EventArgs e)
    {
        try
        {
            List<Produto> allProducts = await App.Db.GetAll();

            if (allProducts.Count == 0)
            {
                await DisplayAlert("Relatório dos Produtos", "Não há produtos cadastrados", "OK");
                return;
            }

            List<string> categorias = new List<string>();
            foreach (var p in allProducts)
            {
                if (!categorias.Contains(p.Tipo)){
                    categorias.Add(p.Tipo);
                }
            }

            string msg = "";
            foreach (var cat in categorias)
            {
                double soma = 0;
                foreach (var p in allProducts)
                {
                    if (p.Tipo == cat)
                        soma += p.Total;
                }
                msg += $"\n{cat}: {soma:C}";
            }

            await DisplayAlert("Somas por Categoria", msg, "OK");
        }
        catch (Exception ex )
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }
}