using RepositoryLibrary.Features.Products.Entities;
using RepositoryLibrary.Features.Purchases.Enums;

namespace RepositoryLibrary.Features.Purchases.Entities
{
    /// <summary>
    /// Input line used to create a purchase request from the simulator or checkout.
    /// </summary>
    /// <param name="Product">Product selected by the user.</param>
    /// <param name="Kind">Whether this line is a subscription or a credit pack.</param>
    /// <param name="Quantity">Number of packs or subscription months requested.</param>
    /// <param name="SubscriptionPeriodStart">Inclusive first day for subscription access.</param>
    /// <param name="SubscriptionPeriodEnd">Inclusive last day for subscription access.</param>
    public sealed record PurchaseRequestLine(
        Product Product,
        PurchaseLineKind Kind,
        int Quantity,
        DateOnly? SubscriptionPeriodStart = null,
        DateOnly? SubscriptionPeriodEnd = null);
}
