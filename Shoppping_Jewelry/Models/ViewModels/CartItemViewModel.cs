namespace Shoppping_Jewelry.Models.ViewModels
{
    public class CartItemViewModel
    {
        public List<CartItemModel> CartItems { get; set; }
        public decimal GrandTotal { get; set; }

        public decimal shippingPrice { get; set; }
    }
}
