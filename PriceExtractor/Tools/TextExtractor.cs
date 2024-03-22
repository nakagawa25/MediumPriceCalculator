using PriceExtractor.Entities;
using PriceExtractor.Enums;

namespace PriceExtractor.Tools
{
    public class TextExtractor
    {
        #region XP Statements Extractor
        public static List<NegotiationAsset> ExtractInXPText(string inputText, DateTime negotiationDate)
        {
            var result = GetStatementsData(inputText, negotiationDate, GetNegotiationFromXPStatement);
            return result;
        }

        public static List<NegotiationAsset> ExtractInPersonalStatement(string inputText)
        {
            var result = GetStatementsData(inputText, DateTime.Now, GetNegotiationFromPersonalStatement);
            return result;
        }

        private static List<NegotiationAsset> GetStatementsData(string inputText, DateTime negotiationDate, Func<string[], DateTime?, NegotiationAsset> GetNegotiationAssets)
        {
            ValidateInput(inputText, negotiationDate);
            var lines = inputText.Split("\r\n");
            var negotiationList = new List<NegotiationAsset>();

            foreach (var line in lines)
            {
                var columns = SanitizeColumns(line.Split(" "));
                negotiationList.Add(GetNegotiationAssets(columns, negotiationDate));
            }

            return negotiationList;
        }

        private static NegotiationAsset GetNegotiationFromXPStatement(string[] columns, DateTime? negotiationDate)
        {
            var negotiationAsset = new NegotiationAsset()
            {
                StockCode = TakeAssetName(columns),
                Amount = Convert.ToInt32(columns[^4]),
                Price = Convert.ToDouble(columns[^3]),
                NegotiationDate = (DateTime)negotiationDate,
                AssetType = TakeAssetType(columns),
                OperationType = (columns[1].ToUpper() == "C") ? OperationType.Buy : OperationType.Sell
            };

            return negotiationAsset;
        }

        private static NegotiationAsset GetNegotiationFromPersonalStatement(string[] columns, DateTime? date = null)
        {
            var negotiationAsset = new NegotiationAsset()
            {
                StockCode = columns[0],
                Amount = Convert.ToInt32(columns[4]),
                Price = Convert.ToDouble(columns[3]),
                NegotiationDate = Convert.ToDateTime(columns[7]),
                AssetType = TakeAssetType(columns[0]),
                OperationType = (columns[1].ToUpper() == "C") ? OperationType.Buy : OperationType.Sell
            };

            return negotiationAsset;
        }

        private static string[] SanitizeColumns(string[] columns)
        {
            return Array.FindAll(columns, c => !string.IsNullOrWhiteSpace(c));
        }

        private static void ValidateInput(string inputText, DateTime negotiationDate)
        {
            if (string.IsNullOrEmpty(inputText))
                throw new Exception("Texto vazio.");

            if (negotiationDate == DateTime.MinValue)
                throw new Exception("Informe a data de negociação.");
        }

        private static AssetType TakeAssetType(string[] columns)
        {
            return TakeAssetType(columns[2]);
        }

        public static AssetType TakeAssetType(string negotiationDocumentType)
        {
            if (negotiationDocumentType.ToUpper() == "VISTA")
                return AssetType.FII;
            if (negotiationDocumentType.ToUpper() == "FRACIONARIO")
                return AssetType.Acao;
            if (negotiationDocumentType.Length == 5 && negotiationDocumentType.Substring(4) == "3")
                return AssetType.Acao;
            if (negotiationDocumentType.Length == 6 && negotiationDocumentType.Substring(4) == "11")
                return AssetType.FII;
            throw new Exception("Tipo de Ativo não conhecido: " + negotiationDocumentType.ToUpper());
        }

        private static string TakeAssetName(string[] columns)
        {
            string assetName = string.Empty;

            if (TakeAssetType(columns) == AssetType.FII)
            {
                var columnsFII = columns.Skip(4);
                foreach (var column in columnsFII)
                {
                    if (column.Length == 6 && column.Substring(4) == "11")
                    {
                        assetName = column;
                        break;
                    }
                }
            }
            else if (TakeAssetType(columns) == AssetType.Acao)
            {
                assetName = columns[3];
            }

            return assetName;
        }
        #endregion
    }
}