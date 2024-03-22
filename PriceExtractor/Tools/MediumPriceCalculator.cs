using PriceExtractor.Entities;
using PriceExtractor.Services;

namespace PriceExtractor.Tools
{
    public static class MediumPriceCalculator
    {
        public static void SumNegotiationsByAssets()
        {
            AssetService.DeleteAll(); // Clean Assets for new execution;

            var allNegotiations = NegotiationAssetService.GetAllNegotiationsAsset();
            var assetsNames = allNegotiations.Select(n => n.StockCode).Distinct();

            var assets = new List<Asset>();
            var assetAux = new Asset();

            foreach (var assetName in assetsNames)
            {
                var assetNegotiation = allNegotiations.Where(n => n.StockCode == assetName);
                assets.Add(new Asset()
                {
                    StockCode = assetName,
                    Amount = assetNegotiation.Where(n => n.OperationType == Enums.OperationType.Buy).Sum(n => n.Amount) -
                             assetNegotiation.Where(n => n.OperationType == Enums.OperationType.Sell).Sum(n => n.Amount),
                    TotalValue = assetNegotiation.Where(n => n.OperationType == Enums.OperationType.Buy).Sum(n => n.Price * n.Amount) -
                                 assetNegotiation.Where(n => n.OperationType == Enums.OperationType.Sell).Sum(n => n.Price * n.Amount),
                    AssetType = assetNegotiation.First().AssetType
                });
            }

            AssetService.InsertManyAssets(assets);
        }

        public static List<Asset> CalculateMediumPrice()
        {
            var assets = AssetService.GetAllAssets();
            foreach (var asset in assets)
            {
                asset.MediumPrice = Math.Round(asset.TotalValue / asset.Amount, 2);
                asset.TotalValue = Math.Round(asset.TotalValue, 2);
            }

            return assets
                .Where(a => a.Amount > 0)
                .OrderBy(x => x.StockCode)
                .OrderBy(x => x.AssetType)
                .ToList();
        }
    }
}
