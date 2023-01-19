using Abp.Application.Features;
using Abp.Localization;
using Abp.Runtime.Validation;
using Abp.UI.Inputs;

namespace Wimi.BtlCore.Features
{
    /* This feature provider is just for an example.
     * You can freely delete all features and add your own.
     */
    public class BtlCoreFeatureProvider : FeatureProvider
    {
        public override void SetFeatures(IFeatureDefinitionContext context)
        {
            var sampleBooleanFeature = context.Create(
                BtlCoreFeatures.SampleBooleanFeature,
                "false",
                L("Sample boolean feature"),
                inputType: new CheckboxInputType());

            sampleBooleanFeature.CreateChildFeature(
                BtlCoreFeatures.SampleNumericFeature,
                "10",
                L("Sample numeric feature"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(1, 1000000)));

            // Another sample feature, value is string
            // sampleBooleanFeature.CreateChildFeature(
            // "SampleStringValue,
            // defaultValue: "axb",
            // displayName: new FixedLocalizableString("Sample string feature"),
            // inputType: new SingleLineStringInputType(new StringValueValidator(2, 10, "^a.*b$"))
            // );
            context.Create(
                BtlCoreFeatures.SampleSelectionFeature,
                "B",
                L("Sample selection feature"),
                inputType:
                    new ComboboxInputType(
                        new StaticLocalizableComboboxItemSource(
                            new LocalizableComboboxItem("A", L("Selection A")),
                            new LocalizableComboboxItem("B", L("Selection B")),
                            new LocalizableComboboxItem("C", L("Selection C")))));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, BtlCoreConsts.LocalizationSourceName);
        }
    }
}
