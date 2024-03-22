using PriceExtractor.Entities;
using PriceExtractor.Services;
using PriceExtractor.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PriceExtractor.Interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void CleanFields()
        {
            dgNegotiation.ItemsSource = null;
            txtInputText.Clear();
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var negotiations = TextExtractor.ExtractInXPText(txtInputText.Text, dpNegotiation.DisplayDate);
                dgNegotiation.ItemsSource = negotiations;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                return;
            }
        }

        private void btnCalculatePersonal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var negotiations = TextExtractor.ExtractInPersonalStatement(txtInputText.Text);
                dgNegotiation.ItemsSource = negotiations;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                return;
            }
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgNegotiation.ItemsSource == null)
                    throw new Exception("O datagrid está vazio");

                NegotiationAssetService.InsertManyNegotiationAsset((IEnumerable<NegotiationAsset>)dgNegotiation.ItemsSource);
                MessageBox.Show("Cadastrado com Sucesso!");
                CleanFields();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                return;
            }
        }

        private void btnGetMediumPrice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var assets = MediumPriceCalculator.CalculateMediumPrice();
                dgNegotiation.ItemsSource = null;
                dgNegotiation.ItemsSource = assets;
                MessageBox.Show("Sucesso");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                return;
            }
        }

        private void btnCalculateAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Você calculará todas as negociações atuais, deseja continuar?", "ação Irreversível", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    MediumPriceCalculator.SumNegotiationsByAssets();
                    MessageBox.Show("Sucesso!");
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                return;
            }
        }

        private void btnInsertMany_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InsertManyScreen insertManyScreen = new InsertManyScreen();
                insertManyScreen.ShowDialog();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                return;
            }
        }

        private void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Você irá apagar TODOS os registros, deseja continuar?", "ação Irreversível", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    NegotiationAssetService.DeleteAllNegotiationAndAssets();
                    MessageBox.Show("Dados excluídos com sucesso!");
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                return;
            }
        }

        // TODO: Remover
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var negotiationAssets = NegotiationAssetService
                    .GetAllNegotiationsAsset()
                    .Where(x => x.AssetType == Enums.AssetType.None)
                    .ToList();

                foreach (var asset in negotiationAssets)
                    asset.AssetType = Tools.TextExtractor.TakeAssetType(asset.StockCode);

                Services.NegotiationAssetService.UpdateNegotiationsAssets(negotiationAssets);

                MessageBox.Show("Sucesso.");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
    }
}
