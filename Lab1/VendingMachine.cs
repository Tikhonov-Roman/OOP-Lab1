using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab1
{
    enum Mode
    {
        User,
        Admin
    }
    
    internal class VendingMachine
    {
        public Mode mode {  get; private set; } = Mode.User;

        private Dictionary<int, int> accumulatedCoins = new Dictionary<int, int>();
        private Dictionary<int, int> deposit;
        private Dictionary<string, Product> products = new Dictionary<string, Product>();
        private readonly List<int> allowedCoins = new List<int>() { 1, 2, 5, 10};


        public VendingMachine()
        {
            foreach (int nominalValue in allowedCoins)
            {
                accumulatedCoins[nominalValue] = 0;
            }
            Reset();
        }

        private void Reset()
        {
            Console.WriteLine("------------------------ДОБРО ПОЖАЛОВАТЬ------------------------");
            deposit = new Dictionary<int, int>();
            foreach (int nominalValue in allowedCoins)
            {
                deposit[nominalValue] = 0;
            }
            DisplayAllowedCoins();
            DisplayProducts();
            DisplayDefaultCommands();
        }
        private void DisplayDefaultCommands()
        {
            Console.WriteLine("------------------------СПИСОК КОМАНД------------------------");

            Console.WriteLine("Чтобы внести монету номиналом X введите: внести X");
            Console.WriteLine("Чтобы приобрести товар с идентификатором ID введите: купить ID");
            Console.WriteLine("Чтобы посмотреть список товаров введите: товары");
            Console.WriteLine("Чтобы отменить операцию введите: отмена");
            Console.WriteLine("Для перехода в режим администрирования введите: администратор");
            if (mode == Mode.Admin)
            {
                Console.WriteLine("Чтобы изменить кол-во существующего товара введите, подставив значения: добавить ИДЕНТИФИКАТОР КОЛ-ВО");
                Console.WriteLine("Чтобы создать новый товар введите, подставив значения: создать ИДЕНТИФИКАТОР НАЗВАНИЕ ЦЕНА КОЛ-ВО");
                Console.WriteLine("Чтобы вывести полученные автоматом монеты введите: вывод");
                Console.WriteLine("Чтобы вернуться в пользовательский режим ввдеите: пользователь");
            }
            Console.WriteLine("------------------------------------------------------------------");

        }
        private void DisplayAllowedCoins()
        {
            string nominals = "";
            foreach(int coin in allowedCoins)
            {
                nominals += ", " + coin;
            }
            Console.WriteLine($"Автомат принимает монеты номиналом{nominals} рублей");
        }

        public void ReadCommand(string command)
        {
            string[] splitCommand = command.Split(' ');
            command = splitCommand[0].ToLower();
            string[] commandParams = splitCommand.Skip(1).ToArray();
            switch (command)
            {
                case "товары":
                    DisplayProducts();
                    break;
                case "внести":
                    DepositCoin(commandParams);
                    break;
                case "купить":
                    MakePurchase(splitCommand[1]);
                    break;
                case "команды":
                    DisplayDefaultCommands();
                    break;
                case "отмена":
                    CancelPurchase();
                    break;
                case "пользователь":
                    ChangeMode(Mode.User);  
                    break;
                case "администратор":
                    ChangeMode(Mode.Admin);
                    break;
                case "добавить":
                    AddProductQuantity(commandParams);
                    break;
                case "создать":
                    CreateNewProduct(commandParams);
                    break;
                case "вывод":
                    GetAccumulatedCoins();
                    break;
                default:
                    Console.WriteLine("Незивестная команда. Чтобы узнать список доступных команд введите: команды");
                    break;


            }
        }

        private void DisplayProducts()
        {
            Console.WriteLine("------------------------СПИСОК ТОВАРОВ------------------------");
            foreach (Product product in products.Values)
            {
                Console.WriteLine($"-{product}");
            }
            Console.WriteLine("----------------------------------------------------------------");
            


        }
        private void DepositCoin(params string[] data)
        {
            if (data.Length != 1)
            {
                Console.WriteLine($"ОШИБКА. Неверное число параметров (Ожидалось: 1. Получено: {data.Length}");
                return;
            }
            int coinNominal;
            if (!int.TryParse(data[0], out coinNominal))
            {
                Console.WriteLine("ОШИБКА. Номинал монеты должен быть числом");
                return;
            }

            if (allowedCoins.Contains(coinNominal))
            {
                deposit[coinNominal] += 1;
                accumulatedCoins[coinNominal] += 1;
                Console.WriteLine($"Монета успешно внесена. Текущая сумма: {GetDepositSum()}");
            }
            else
            {
                Console.WriteLine("ОШИБКА. Моенты с данным номиналом не поддерживаются");
                DisplayAllowedCoins();
            }
        }
        private void AddProductQuantity(params string[] data)
        {
            if (mode != Mode.Admin)
            {
                Console.WriteLine("ОШИБКА. Менять количество товаров может только администратор");
                return;
            }
            if (data.Length != 2)
            {
                Console.WriteLine($"ОШИБКА. Неверное число параметров (Ожидалось: 2. Получено: {data.Length}");
                return;
            }
            string id = data[0];
            int quantity;
            if (!int.TryParse(data[1], out quantity))
            {
                Console.WriteLine("ОШИБКА. Количество продуктов должно быть числом");
                return;
            }
           
            if (products.ContainsKey(id))
            {
                products[id].ChangeQuantity(quantity);
                Console.WriteLine($"Количество товара с идентификатором {id} изменено успешно");
            }
            else
                Console.WriteLine("ОШИБКА. Продукт с указанным идентификатором не существует");




        }
        private void CreateNewProduct(params string[] data)
        {
            if (mode != Mode.Admin)
            {
                Console.WriteLine("ОШИБКА. Добавлять новые товары может только администратор");
                return;
            }
            if (data.Length != 4)
            {
                Console.WriteLine($"ОШИБКА. Неверное число параметров (Ожидалось: 4. Получено: {data.Length}");
                return;
            }
            string id = data[0];
            string name = data[1];
            int price;
            int quantity;
            if (!int.TryParse(data[2], out price))
            {
                Console.WriteLine("ОШИБКА. Цена продукта должна быть числом");
                return;
            }
            if (!int.TryParse(data[3], out quantity))
            {
                Console.WriteLine("ОШИБКА. Количество продуктов должно быть числом");
                return;
            }

             products[id] = new Product(id, name, price, quantity);
             Console.WriteLine("Товар создан успешно");
            
        }
        private void GetAccumulatedCoins()
        {
            if (mode != Mode.Admin)
            {
                Console.WriteLine("ОШИБКА. Выводить монеты может только администратор");
                return;
            }
            int sum = 0;
            Console.WriteLine("Накоплено монет:");
            foreach(int coin in  accumulatedCoins.Keys)
            {
                sum += coin * accumulatedCoins[coin];
                Console.WriteLine($"{coin}руб x {accumulatedCoins[coin]}шт");
            }
            accumulatedCoins = new Dictionary<int, int>();
            Console.WriteLine($"ИТОГО: {sum}");

        }
        private void MakePurchase(string productId)
        {
            if (products.ContainsKey(productId))
            {
                int depositSum = GetDepositSum();
                if (products[productId].Price <= depositSum)
                {
                    if (products[productId].Quantity > 0)
                    {
                        int change = depositSum - products[productId].Price;
                        if(change == 0)
                        {
                            products[productId].ChangeQuantity(-1);
                            Console.WriteLine($"Выдан продукт {products[productId].Name}");
                            Console.WriteLine("Спасибо за покупку!\n");
                            Reset();
                        }

                        Dictionary<int, int> changeCoins = CalculateChange(change);
                        if (changeCoins == null)
                        {
                            Console.WriteLine("Автомат не сможет выдать сдачу. Продолжить?");
                            string answer = "";
                            while(answer != "да" && answer != "нет")
                            {
                                Console.WriteLine("Введите да/нет");
                                answer = Console.ReadLine();
                            }
                            if(answer == "нет")
                            {
                                CancelPurchase();
                                return;
                            }
                            products[productId].ChangeQuantity(-1);
                            Console.WriteLine($"Выдан продукт {products[productId].Name}");
                        }
                        products[productId].ChangeQuantity(-1);
                        Console.WriteLine($"Выдан продукт {products[productId].Name}");
                        Console.WriteLine($"Выдана сдача: {change}руб");
                        List<int> changeCoinsTypes = new List<int>(changeCoins.Keys);
                        changeCoinsTypes.Sort();
                        foreach (int coin in changeCoinsTypes)
                        {
                            Console.WriteLine($"{coin}руб x {changeCoins[coin]}шт");
                        }
                        Console.WriteLine("Спасибо за покупку!\n");
                        Reset();

                    }
                    else
                         Console.WriteLine("Невозможно совершить покупку. Отсуствует товар");
                }
                else
                    Console.WriteLine("Невозможно совершить покупку. Внесенной суммы не хватает для покупки");  
            }
            else
                Console.WriteLine("Невозможно совершить покупку. Неправильно указан идентификатор товара");
        }
        private void CancelPurchase()
        {
            Console.WriteLine("Отмена операции. Заберите внесенные монеты");
            foreach (int coin in deposit.Keys)
            {
                accumulatedCoins[coin] -= deposit[coin];
                Console.WriteLine($"{coin} x {deposit[coin]}шт");

            }
        }

        private void ChangeMode(Mode newMode)
        {   
            mode = newMode;
            if(mode == Mode.User)
            {
                Console.WriteLine("Автомат работает в обычном режиме");
            }
            else
            {
                Console.WriteLine("Автомат работает в режиме администрирования");
            }
        }

        private Dictionary<int, int> CalculateChange(int targetChange)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();

            for(int ten = 0; ten <= accumulatedCoins[10]; ten++)
            {
                for (int five = 0; five <= accumulatedCoins[5]; five++)
                {
                    for (int two = 0; two <= accumulatedCoins[2]; two++)
                    {
                        int needDenominationOfOneCount = targetChange - two * 2 - five * 5 - ten * 10;
                      

                        if ((needDenominationOfOneCount) <= accumulatedCoins[1])
                        {
                            result[1] = needDenominationOfOneCount;
                            result[2] = two;
                            result[5] = five;
                            result[10] = ten;
                            return result;

                        }
                    }
                }
            }
            return null;
        }
        private int GetDepositSum()
        {
            int sum = 0;
            foreach (int coin in deposit.Keys)
            {
                sum += coin * deposit[coin];
            }
            return sum;
        }

    }
}
