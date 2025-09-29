
namespace Lab1
{
    internal class Product
    {
        public string Id { get; private set; }

        public string Name { get; private set; }
        public int Price { get; private set; }
        public int Quantity { get; private set; }


        public Product(string id, string name, int price, int quantity)
        {

            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        public void ChangeQuantity(int difference)
        {
            Quantity += difference;
        }
        
        public override string ToString()
        {
            return $"№{Id} {Name} в количестве {Quantity} по цене {Price}руб/шт";
        }

    }
}
