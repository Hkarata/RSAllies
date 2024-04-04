using Microsoft.Extensions.Compliance.Classification;

namespace RSAllies.Api.Extensions
{
    public static class DataTaxonomy
    {
        public static string TaxonomyName { get; } = typeof(DataTaxonomy).FullName!;
        public static DataClassification SensitiveData { get; } = new DataClassification(TaxonomyName, nameof(SensitiveData));
        public static DataClassification PiiData { get; } = new DataClassification(TaxonomyName, nameof(PiiData));
    }

    public class SensitiveDataAttribute : DataClassificationAttribute
    {
        public SensitiveDataAttribute() : base(DataTaxonomy.SensitiveData)
        {
        }
    }

    public class PiiDataAttribute : DataClassificationAttribute
    {
        public PiiDataAttribute() : base(DataTaxonomy.PiiData)
        {
        }
    }
}
