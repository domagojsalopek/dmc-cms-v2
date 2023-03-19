namespace Dmc.Cms.App
{
    public interface ISiteResources
    {
        string DefaultOpenGraphImage { get; set; }
        string DefaultSiteHeader { get; set; }
        string EmailHeader { get; set; }
        string FooterLogo { get; set; }
        string Logo { get; set; }
        string RetinaLogo { get; set; }
    }
}