using System;
using System.Collections.Generic;

public class Deck
{
    private List<Card> _cards;
    private Random _random;

    //constructor
    public Deck()
    {
        _cards = new List<Card>();
        _random = new Random();
        CreateDeck();
        ShuffleDeck();
    }

    private void CreateDeck()
    {
        string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };

        foreach (var suit in suits)
        {
            foreach (var rank in ranks)
            {
                int cardValue;
                if (rank == "Ace")
                {
                    cardValue = 11;
                }
                else if (rank == "Jack" || rank == "Queen" || rank == "King")
                {
                    cardValue = 10;
                }
                else
                {
                    cardValue = int.Parse(rank);
                }
                _cards.Add(new Card(rank, suit, cardValue));
            }
        }
    }

    // Shuffle met Fisher-Yates algoritme
    private void ShuffleDeck()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            int j = _random.Next(i, _cards.Count);
            var temp = _cards[i];
            _cards[i] = _cards[j];
            _cards[j] = temp;
        }
    }

    public Card DealCard()
    {
        if (_cards.Count == 0)
        {
            CreateDeck();
            ShuffleDeck();
        }

        Card dealtCard = _cards[0];
        _cards.RemoveAt(0);
        return dealtCard;
    }
}
