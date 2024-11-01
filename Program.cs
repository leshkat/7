using System;
using System.Collections.Generic;
using System.Threading;

namespace BarberShopSimulation
{
    class BarberShop
    {
        private readonly int waitingRoomSeats;
        private readonly Queue<Customer> waitingCustomers = new Queue<Customer>();
        private readonly object lockObject = new object();
        private bool barberSleeping = true;

        public BarberShop(int seats)
        {
            waitingRoomSeats = seats;
        }

        public void EnterShop(Customer customer)
        {
            lock (lockObject)
            {
                if (waitingCustomers.Count < waitingRoomSeats)
                {
                    waitingCustomers.Enqueue(customer);
                    Console.WriteLine($"{customer.Name} is sitting in the waiting area.");
                    Monitor.Pulse(lockObject); 
                }
                else
                {
                    Console.WriteLine($"{customer.Name} leaves the shop as there are no available seats.");
                }
            }
        }

        public void BarberWork()
        {
            while (true)
            {
                Customer customer = null;

                lock (lockObject)
                {
                    while (waitingCustomers.Count == 0)
                    {
                        Console.WriteLine("The barber falls asleep as there are no customers.");
                        barberSleeping = true;
                        Monitor.Wait(lockObject);
                    }

                    barberSleeping = false;
                    customer = waitingCustomers.Dequeue();
                    Console.WriteLine($"The barber starts cutting {customer.Name}'s hair.");
                }

                Thread.Sleep(2000);
                Console.WriteLine($"The barber has finished cutting {customer.Name}'s hair.");
            }
        }
    }

    class Customer
    {
        public string Name { get; }

        public Customer(string name)
        {
            Name = name;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int waitingSeats = 3;
            var barberShop = new BarberShop(waitingSeats);

            Thread barberThread = new Thread(barberShop.BarberWork);
            barberThread.Start();

            string[] customerNames = { "Anna", "Emily", "David", "Danil", "Sancho", "Clara" };
            Random random = new Random();

            foreach (var name in customerNames)
            {
                Thread.Sleep(random.Next(500, 1500)); 
                Customer customer = new Customer(name);
                Thread customerThread = new Thread(() => barberShop.EnterShop(customer));
                customerThread.Start();
            }
        }
    }
}
