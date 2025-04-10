using System;
using System.Collections.Generic;

public class Player
{
    public string Name { get; set; }
    public List<Card> Hand { get; set; }

    // 💰 New fields for betting
    public int Money { get; set; }
    public int CurrentBet { get; set; }

    // 🏗️ Constructor initializes hand and starting money
    public Player(string name)
    {
        Name = name;
        Hand = new List<Card>();
        Money = 1000;         // starting cash
        CurrentBet = 0;
    }

    public void AddCard(Card card)
    {
        Hand.Add(card);
    }

    public string ChooseAction()
    {
        string action;
        Console.WriteLine($"{Name}, Choose hit, pass, or double down");
        action = Console.ReadLine().ToLower();

        while (action != "hit" && action != "pass" && action != "double down")
        {
            Console.WriteLine("Invalid action. Please choose 'hit', 'pass', or 'double down'.");
            action = Console.ReadLine().ToLower();
        }

        return action;
    }

    public bool HasBlackjack()
    {
        return Hand.Count == 2 && CalculateHandValue() == 21;
    }

    public bool IsBusted()
    {
        return CalculateHandValue() > 21;
    }

    public int CalculateHandValue()
    {
        int totalValue = 0;
        int aceCount = 0;

        // sum of card values
        foreach (var card in Hand)
        {
            totalValue += card.Value;
            if (card.Rank == "Ace")
                aceCount++;
        }

        // ace 1 or 10 check
        while (totalValue > 21 && aceCount > 0)
        {
            totalValue -= 10;
            aceCount--;
        }

        return totalValue;
    }

    public string ShowHand()
    {
        string handString = "";
        foreach (var card in Hand)
        {
            handString += card.ToString() + ", ";
        }

        int handValue = CalculateHandValue();
        handString += $" Score: {handValue}";

        // Remove the last comma
        return handString.TrimEnd(',', ' ');
    }

    // Double Down: doubles the bet, and player can only take one more card
    public bool DoubleDown()
    {
        if (Money >= CurrentBet)
        {
            CurrentBet *= 2; // Double the bet
            Console.WriteLine($"{Name} doubles down and the new bet is ${CurrentBet}.");
            return true; // Player can now only take one card
        }
        else
        {
            Console.WriteLine($"{Name} doesn't have enough money to double down.");
            return false; // Not enough money
        }
    }
}
