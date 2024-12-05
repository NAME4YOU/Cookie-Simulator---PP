using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;

class CookieClicker
{
    static float cookies = 0;
    static float cookiesPerSecond = 1;
    static double multiplier = 1.0;

    static float totalCookies = 0;
    static int elapsedTime = 0;
    static int totalPurchases = 0;
    static int totalPowerUpsUsed = 0;
    static List<string> itemsBought = new List<string>();

    static bool eventsEnabled = true;
    static int eventMinInterval = 60;
    static int eventMaxInterval = 600;
    static Random random = new Random();
    static int nextEventTime = random.Next(10, 30); 

    static Dictionary<string, (float basePrice, int count, float cpsBoost)> shopItems = new Dictionary<string, (float, int, float)>()
    {
        {"Cookie Tray", (100, 0, 1.5f)},
        {"Industrial Oven", (500, 0, 5.0f)},
    };

    static Dictionary<string, (float basePrice, float cpsIncrease)> upgrades = new Dictionary<string, (float, float)>()
    {
        {"Golden Tray Upgrade", (200, 2.0f)},
        {"High Efficiency Oven", (1000, 10.0f)},
    };

    static Dictionary<string, (int price, double multiplier, int duration)> powerUps = new Dictionary<string, (int, double, int)>()
    {
        {"Double Cookies", (100, 2.0, 45)},
        {"Triple Cookies", (300, 3.0, 25)},
        {"Quadruple Cookies", (600, 4.0, 15)}
    };

    static List<Achievement> achievements = new List<Achievement>()
    {
        new Achievement("Cookie Enthusiast", "Cookies", 100),
        new Achievement("Cookie Connoisseur", "Cookies", 1000),
        new Achievement("Baking Veteran", "Time", 60),
        new Achievement("Shopper", "Purchases", 10),
        new Achievement("Power-Up Fan", "PowerUps", 10)
    };

    static List<Event> events = new List<Event>()
    {
        new Event(
            "A generous grandma donates her secret cookie recipe! Multiplier increases for 15 seconds.",
            "Good",
            () => { multiplier *= 1.5; Console.WriteLine("Your cookie multiplier increased by 1.5x!"); },
            () => { multiplier /= 1.5; Console.WriteLine("The grandma's recipe effect has worn off. Multiplier returned to normal."); },
            15
        ),
        new Event(
            "Oops! The cookie oven caught fire. Multiplier decreases for 10 seconds.",
            "Bad",
            () => { multiplier *= 0.8; Console.WriteLine("Your cookie multiplier decreased by 20%!"); },
            () => { multiplier /= 0.8; Console.WriteLine("The oven is fixed! Multiplier returned to normal."); },
            10
        ),
        new Event(
            "Cookie prices plummet! All shop prices are reduced for 20 seconds!",
            "Good",
            () =>
            {
                foreach (var item in shopItems)
                {
                    shopItems[item.Key] = (item.Value.basePrice * 0.8f, item.Value.count, item.Value.cpsBoost);
                }
                Console.WriteLine("All shop prices reduced by 20%!");
            },
            () =>
            {
                foreach (var item in shopItems)
                {
                    shopItems[item.Key] = (item.Value.basePrice / 0.8f, item.Value.count, item.Value.cpsBoost);
                }
                Console.WriteLine("Shop prices have returned to normal.");
            },
            20
        ),
        new Event(
            "A sneaky raccoon stole some cookies! You lose 100 cookies.",
            "Bad",
            () =>
            {
                cookies = Math.Max(0, cookies - 100);
                Console.WriteLine("The sneaky raccoon stole 100 cookies!");
            },
            null,
            0
        ),
        new Event(
            "Your cookie empire trends on social media! CPS increases by 2x for 25 seconds!",
            "Good",
            () => { cookiesPerSecond *= 2; Console.WriteLine("Your cookies per second doubled!"); },
            () => { cookiesPerSecond /= 2; Console.WriteLine("The social media trend ended. CPS returned to normal."); },
            25
        ),
        new Event(
            "Tax auditors demand back taxes on cookies. You lose 10% of your cookies.",
            "Bad",
            () =>
            {
                cookies *= 0.9f;
                Console.WriteLine("Tax auditors took 10% of your cookies!");
            },
            null,
            0
        )
    };

    static void Main()
    {
        Console.WriteLine("Welcome to Cookie Clicker!");
        Console.WriteLine("Cookies will be automatically baked every second.");

        SaveData saveData = DataManager.LoadGame();

        cookies = saveData.Cookies;
        cookiesPerSecond = saveData.CookiesPerSecond;
        multiplier = saveData.Multiplier;
        totalCookies = saveData.TotalCookies;
        elapsedTime = saveData.ElapsedTime;
        totalPurchases = saveData.TotalPurchases;
        totalPowerUpsUsed = saveData.TotalPowerUpsUsed;
        itemsBought = saveData.ItemsBought;

        Timer timer = new Timer(BakeCookies, null, 0, 1000);
        Timer saveTimer = new Timer(AutoSave, null, 0, 5000);

        while (true)
        {
            string input = Console.ReadLine()?.ToLower();

            if (input == "menu")
            {
                ShowMenu();
            }
            else if (input == "exit")
            {
                Console.WriteLine("Thanks for playing! Goodbye!");
                break;
            }
            else
            {
                Console.WriteLine("Invalid input. Type 'menu' or 'exit'.");
            }
        }
    }

    static void AutoSave(object state)
    {
        DataManager.SaveGame(cookies, cookiesPerSecond, multiplier, totalCookies, elapsedTime, totalPurchases, totalPowerUpsUsed, itemsBought);
        //Console.WriteLine("Game automatically saved.");
    }

    static void BakeCookies(object state)
    {
        float cookiesEarned = (float)(cookiesPerSecond * multiplier);
        cookies += cookiesEarned;
        totalCookies += cookiesEarned;
        elapsedTime++;

        CheckAchievements();

        if (eventsEnabled && elapsedTime >= nextEventTime)
        {
            TriggerRandomEvent();
            nextEventTime = elapsedTime + random.Next(eventMinInterval, eventMaxInterval);
        }
    }

    static void TriggerRandomEvent()
    {
        var randomEvent = events[random.Next(events.Count)];
        Console.WriteLine($"-- EVENT --\n{randomEvent.Description}");
        randomEvent.StartEffect.Invoke();

        if (randomEvent.Duration > 0 && randomEvent.EndEffect != null)
        {
            System.Threading.Tasks.Task.Delay(randomEvent.Duration * 1000).ContinueWith(t =>
            {
                randomEvent.EndEffect.Invoke();
            });
        }
    }

    static void ShowMenu()
    {
        Console.Clear();
        Console.WriteLine("-- MENU --");
        Console.WriteLine("1. Upgrades");
        Console.WriteLine("2. Shop");
        Console.WriteLine("3. Power-ups");
        Console.WriteLine("4. Achievements");
        Console.WriteLine("5. Statistics");
        Console.WriteLine("6. Cookie Amount");
        Console.WriteLine("7. Back to Baking");
        Console.Write("Choose an option: ");

        switch (Console.ReadLine())
        {
            case "1":
                ShowUpgrades();
                break;
            case "2":
                ShowShop();
                break;
            case "3":
                ShowPowerUps();
                break;
            case "4":
                ShowAchievements();
                break;
            case "5":
                ShowStatistics();
                break;
            case "6":
                ShowCookieAmount();
                break;
            case "7":
                Console.WriteLine("Returning to baking...");
                break;
            default:
                Console.WriteLine("Invalid option. Try again.");
                break;
        }
    }

    static void ShowCookieAmount()
    {
        Console.WriteLine("-- COOKIE AMOUNT --");
        Console.WriteLine($"You have {cookies:F2} cookies.");
    }

    static void ShowUpgrades()
    {
        Console.WriteLine("-- UPGRADES --");
        Console.WriteLine($"Current cookies: {cookies:F2}");
        foreach (var upgrade in upgrades)
        {
            Console.WriteLine($"{upgrade.Key} - {upgrade.Value.basePrice:F2} cookies, increases CPS by {upgrade.Value.cpsIncrease:F2}");
        }
        Console.Write("Enter upgrade name to purchase or 'back' to return: ");
        string choice = Console.ReadLine();

        if (upgrades.ContainsKey(choice) && cookies >= upgrades[choice].basePrice)
        {
            cookies -= upgrades[choice].basePrice;
            cookiesPerSecond += upgrades[choice].cpsIncrease;
            totalPurchases++;
            Console.WriteLine($"Purchased {choice}! Your cookies per second increased by {upgrades[choice].cpsIncrease:F2}.");
        }
        else if (choice.ToLower() != "back")
        {
            Console.WriteLine("Invalid choice or insufficient cookies.");
        }
    }

    static void ShowShop()
    {
        Console.WriteLine("-- SHOP --");
        Console.WriteLine($"Current cookies: {cookies:F2}");
        foreach (var item in shopItems)
        {
            Console.WriteLine($"{item.Key} - {item.Value.basePrice:F2} cookies (Owned: {item.Value.count}, CPS Boost: {item.Value.cpsBoost:F2})");
        }
        Console.Write("Enter item name to purchase or 'back' to return: ");
        string choice = Console.ReadLine();

        if (shopItems.ContainsKey(choice) && cookies >= shopItems[choice].basePrice)
        {
            cookies -= shopItems[choice].basePrice;
            cookiesPerSecond += shopItems[choice].cpsBoost;

            shopItems[choice] = (shopItems[choice].basePrice * 1.15f, shopItems[choice].count + 1, shopItems[choice].cpsBoost);
            totalPurchases++;

            Console.WriteLine($"Purchased {choice}! Your cookies per second increased by {shopItems[choice].cpsBoost:F2}. New price: {shopItems[choice].basePrice:F2}");
        }
        else if (choice.ToLower() != "back")
        {
            Console.WriteLine("Invalid choice or insufficient cookies.");
        }
    }

    static void ShowPowerUps()
    {
        Console.WriteLine("-- POWER-UPS --");
        Console.WriteLine($"Current cookies: {cookies:F2}");
        foreach (var powerUp in powerUps)
        {
            Console.WriteLine($"{powerUp.Key} - {powerUp.Value.price} cookies, Multiplier: {powerUp.Value.multiplier}x, Duration: {powerUp.Value.duration} seconds");
        }
        Console.Write("Enter power-up name to purchase or 'back' to return: ");
        string choice = Console.ReadLine();

        if (powerUps.ContainsKey(choice) && cookies >= powerUps[choice].price)
        {
            cookies -= powerUps[choice].price;
            totalPowerUpsUsed++;
            double previousMultiplier = multiplier;
            multiplier *= powerUps[choice].multiplier;

            Console.WriteLine($"Purchased {choice}! Multiplier {powerUps[choice].multiplier}x applied for {powerUps[choice].duration} seconds.");

            System.Threading.Tasks.Task.Delay(powerUps[choice].duration * 1000).ContinueWith(t => multiplier = previousMultiplier);
        }
        else if (choice.ToLower() != "back")
        {
            Console.WriteLine("Invalid choice or insufficient cookies.");
        }
    }

    static void ShowAchievements()
    {
        Console.WriteLine("-- ACHIEVEMENTS --");
        foreach (var achievement in achievements)
        {
            Console.WriteLine($"{achievement.Name} - {achievement.Type}: {achievement.Condition} - {(achievement.Achieved ? "Achieved" : "Not Achieved")}");
        }
    }

    static void ShowStatistics()
    {
        Console.WriteLine("-- STATISTICS --");
        Console.WriteLine($"Total cookies baked: {totalCookies:F2}");
        Console.WriteLine($"Elapsed time: {elapsedTime} seconds");
        Console.WriteLine($"Cookies per second: {cookiesPerSecond:F2}");
        Console.WriteLine($"Current multiplier: {multiplier}");
        Console.WriteLine($"Total purchases: {totalPurchases}");
        Console.WriteLine($"Total power-ups used: {totalPowerUpsUsed}");
    }

    static void CheckAchievements()
    {
        foreach (var achievement in achievements)
        {
            if (!achievement.Achieved)
            {
                bool achieved = false;

                switch (achievement.Type)
                {
                    case "Cookies":
                        if (totalCookies >= achievement.Condition)
                            achieved = true;
                        break;
                    case "Time":
                        if (elapsedTime >= achievement.Condition)
                            achieved = true;
                        break;
                    case "Purchases":
                        if (totalPurchases >= achievement.Condition)
                            achieved = true;
                        break;
                    case "PowerUps":
                        if (totalPowerUpsUsed >= achievement.Condition)
                            achieved = true;
                        break;
                }

                if (achieved)
                {
                    achievement.Achieved = true;
                    Console.WriteLine($"Achievement unlocked: {achievement.Name}!");
                }
            }
        }
    }
}

class Achievement
{
    public string Name { get; set; }
    public string Type { get; set; }
    public float Condition { get; set; }
    public bool Achieved { get; set; }

    public Achievement(string name, string type, float condition)
    {
        Name = name;
        Type = type;
        Condition = condition;
        Achieved = false;
    }
}

class Event
{
    public string Description { get; set; }
    public string Type { get; set; } 
    public Action StartEffect { get; set; } 
    public Action EndEffect { get; set; } 
    public int Duration { get; set; } 

    public Event(string description, string type, Action startEffect, Action endEffect, int duration)
    {
        Description = description;
        Type = type;
        StartEffect = startEffect;
        EndEffect = endEffect;
        Duration = duration;
    }
}

class DataManager
{
    private static string saveFilePath = "cookie_clicker_save.json";

    public static void SaveGame(float cookies, float cookiesPerSecond, double multiplier, float totalCookies,
                                int elapsedTime, int totalPurchases, int totalPowerUpsUsed, List<string> itemsBought)
    {
        var saveData = new SaveData
        {
            Cookies = cookies,
            CookiesPerSecond = cookiesPerSecond,
            Multiplier = multiplier,
            TotalCookies = totalCookies,
            ElapsedTime = elapsedTime,
            TotalPurchases = totalPurchases,
            TotalPowerUpsUsed = totalPowerUpsUsed,
            ItemsBought = itemsBought
        };

        string json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(saveFilePath, json);
        //Console.WriteLine("Game saved successfully!");
    }

    public static SaveData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData saveData = JsonSerializer.Deserialize<SaveData>(json);
            Console.WriteLine("Game loaded successfully!");
            return saveData;
        }
        else
        {

            var defaultSave = new SaveData
            {
                Cookies = 0,
                CookiesPerSecond = 1,
                Multiplier = 1.0,
                TotalCookies = 0,
                ElapsedTime = 0,
                TotalPurchases = 0,
                TotalPowerUpsUsed = 0,
                ItemsBought = new List<string>()
            };

            Console.WriteLine("No save file found. Starting a new game.");

            SaveGame(defaultSave.Cookies, defaultSave.CookiesPerSecond, defaultSave.Multiplier, defaultSave.TotalCookies,
                     defaultSave.ElapsedTime, defaultSave.TotalPurchases, defaultSave.TotalPowerUpsUsed, defaultSave.ItemsBought);

            return defaultSave;
        }
    }
}

class SaveData
{
    public float Cookies { get; set; }
    public float CookiesPerSecond { get; set; }
    public double Multiplier { get; set; }
    public float TotalCookies { get; set; }
    public int ElapsedTime { get; set; }
    public int TotalPurchases { get; set; }
    public int TotalPowerUpsUsed { get; set; }
    public List<string> ItemsBought { get; set; } = new List<string>();
}