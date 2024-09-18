using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Enums;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Helpers
{
    public class PurchaseHelper : IPurchaseHelper
    {
        private readonly IProductsUnitOfWork _productsUnitOfWork;
        private readonly IKardexUnitOfWork _kardexUnitOfWork;
        private readonly IPurchaseUnitOfWork _purchaseUnitOfWork;
        private readonly ISuppliersUnitOfWork _suppliersUnitOfWork;
        private readonly ITemporalPurchasesUnitOfWork _temporalPurchasesUnitOfWork;

        public PurchaseHelper(IProductsUnitOfWork productsUnitOfWork , IKardexUnitOfWork kardexUnitOfWork,
            IPurchaseUnitOfWork purchaseUnitOfWork , ISuppliersUnitOfWork suppliersUnitOfWork, 
            ITemporalPurchasesUnitOfWork temporalPurchasesUnitOfWork)
        {
           _productsUnitOfWork = productsUnitOfWork;
           _kardexUnitOfWork = kardexUnitOfWork;
            _purchaseUnitOfWork = purchaseUnitOfWork;
           _suppliersUnitOfWork = suppliersUnitOfWork;
           _temporalPurchasesUnitOfWork = temporalPurchasesUnitOfWork;
        }
        public async Task<ActionResponse<bool>> ProcessPurchaseAsync(PurchaseDTO purchaseDTO, string email)
        {
           if(purchaseDTO.PurchaseDetails==null || purchaseDTO.PurchaseDetails.Count()==0)
            {
                return new ActionResponse<bool>
                {
                    Message= "no details in purchase."
                };
            }
            var responseSupplier = await _suppliersUnitOfWork.GetAsync(purchaseDTO.SupplierId);
            var purchase = new Purchase
            {
                Supplier=responseSupplier.Result,
                Date=purchaseDTO.Date,
                Remarks=purchaseDTO.Remarks,
                PurchaseDetails=new List<PurchaseDetail>(),
            };

            foreach (var item in purchaseDTO.PurchaseDetails)
            {
                var productresponse = await _productsUnitOfWork.GetAsync(item.ProductId);
                if(!productresponse.WasSuccess)
                {
                    return new ActionResponse<bool>
                    {
                        Message= $"Product with Id: {item.ProductId}, not found.",
                    };
                }
                purchase.PurchaseDetails.Add(new PurchaseDetail
                {
                    Cost=item.Cost,
                    Description=productresponse.Result!.Description,
                    Image=productresponse.Result!.MainImage,
                    Name=productresponse.Result!.Name,
                    Quantity=item.Quantity,
                    Remarks=item.Remarks,
                    Product=productresponse.Result,
                });
                var kardexDTO = new KardexDTO
                {
                    Date=purchase.Date,
                    ProductId=item.ProductId,
                    KardexType=KardexType.Purchase,
                    Cost=item.Cost,
                    Quantity=item.Quantity,
                };
                await _kardexUnitOfWork.AddAsync(kardexDTO);
            }
            var responsePurchase = await _purchaseUnitOfWork.AddAsync(purchase);
            if (!responsePurchase.WasSuccess)
            {
                return new ActionResponse<bool>
                {
                    Message = responsePurchase.Message,
                };
            }

            await _temporalPurchasesUnitOfWork.DeleteAsync(email);
            return new ActionResponse<bool>
            {
                Result = true,
            };
        }
    }
}
