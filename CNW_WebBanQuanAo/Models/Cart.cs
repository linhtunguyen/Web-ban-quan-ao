using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNW_WebBanQuanAo.Models
{
    [Serializable]
    public class CartItem
    {
        public SANPHAM Sanpham { get; set; }
        public int Quantity { set; get; }
    }
    public class Cart
    {
        public List<CartItem> lineCollection = new List<CartItem>();

        public void AddItem(SANPHAM sp, int quantity)
        {
            CartItem line = lineCollection
                .Where(p => p.Sanpham.MaQA == sp.MaQA)
                .FirstOrDefault();

            if (line == null)
            {
                lineCollection.Add(new CartItem
                {
                    Sanpham = sp,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
                if (line.Quantity <= 0)
                {
                    lineCollection.RemoveAll(l => l.Sanpham.MaQA == sp.MaQA);
                }
            }

        }
        public void UpdateItem(SANPHAM sp, int quantity)
        {
            CartItem line = lineCollection
                .Where(p => p.Sanpham.MaQA == sp.MaQA)
                .FirstOrDefault();

            if (line != null)
            {
                if (quantity > 0)
                {
                    line.Quantity = quantity;
                }
                else
                {
                    lineCollection.RemoveAll(l => l.Sanpham.MaQA == sp.MaQA);
                }
            }
        }
        public void RemoveLine(SANPHAM sp)
        {
            lineCollection.RemoveAll(l => l.Sanpham.MaQA == sp.MaQA);
        }

        public int GetProductQuantity(SANPHAM sp)
        {
            return lineCollection.Where(s => s.Sanpham.MaQA == sp.MaQA)
                .Single()
                .Quantity;
        }
        public List<CartItem> Lines
        {
            get { return lineCollection; }
        }
        public int? ComputeTotalProduct()
        {
            return lineCollection.Sum(e => e.Quantity);

        }
        public void Clear()
        {
            lineCollection.Clear();
        }
        public decimal? ComputeTotalValue()
        {
            return lineCollection.Sum(e => e.Sanpham.MATHANG.GiaBan * e.Quantity);

        }

    }
}