using PriceExtractor.Entities;
using PriceExtractor.Entities.Context;
using PriceExtractor.Enums;

namespace PriceExtractor.Services
{
    public static class NegotiationAssetService
    {
        public static List<NegotiationAsset> GetAllNegotiationsAsset()
        {
            using (var context = new ContextDB())
            {
                return context.Negotiations.ToList();
            }
        }

        public static List<NegotiationAsset> GetNegotiationsAssetByStockCode(string stockCode)
        {
            using (var context = new ContextDB())
            {
                return context.Negotiations.Where(n => n.StockCode == stockCode).ToList();
            }
        }

        public static List<KeyValuePair<string, AssetType>> GetAssetsName()
        {
            using (var context = new ContextDB())
            {
                return context.Negotiations.GroupBy(n => n.StockCode)
                    .Select(g => new KeyValuePair<string, AssetType>(g.Key, g.Select(n => n.AssetType).First())).ToList();
            }
        }

        public static void InsertNegotiationAsset(NegotiationAsset negotiationAsset)
        {
            using (var context = new ContextDB())
            {
                context.Negotiations.Add(negotiationAsset);
                context.SaveChanges();
            }
        }

        public static void InsertManyNegotiationAsset(IEnumerable<NegotiationAsset> negotiationsAssets)
        {
            using (var context = new ContextDB())
            {
                foreach (var negotiationAsset in negotiationsAssets)
                    context.Negotiations.Add(negotiationAsset);
                context.SaveChanges();
            }
        }

        public static void UpdateNegotiationsAssets(List<NegotiationAsset> negotiationAssets)
        {
            using (var context = new ContextDB())
            {
                foreach (var negotiationAsset in negotiationAssets)
                {
                    context.Negotiations.Update(negotiationAsset);
                }
                context.SaveChanges();
            }
        }

        public static void DeleteAllNegotiationAndAssets()
        {
            using (var context = new ContextDB())
            {
                context.Negotiations.RemoveRange(context.Negotiations);
                context.SaveChanges();
            }
            AssetService.DeleteAll();
        }
    }
}
