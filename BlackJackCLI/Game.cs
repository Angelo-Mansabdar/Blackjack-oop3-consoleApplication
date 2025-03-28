using System;
using System.Collections.Generic;

public class BlackjackGame
{
    private Deck _deck;
    private List<Player> _players;
    private Player _dealer;

    //constructor
    public BlackjackGame()
    {
        _deck = new Deck();
        _players = new List<Player>();
        _dealer = new Player("Dealer");
    }

    public void StartGame()
    {
        Console.Clear();
        Console.WriteLine("Welcome to blackjack");

        int numberOfPlayers = GetNumberOfPlayers();
        AddPlayers(numberOfPlayers);

        DealInitialCards();
        Play();
    }

    // aantal spelers vragen
    private int GetNumberOfPlayers()
    {
        int numberOfPlayers;
        while (true)
        {
            Console.WriteLine("Dealer, how many players will join? { 1 - 4 }: ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out numberOfPlayers) && numberOfPlayers >= 1 && numberOfPlayers <= 4)
            {
                return numberOfPlayers;
            }
            else
            {
                Console.WriteLine("choose players { 1 - 4 }");
            }
        }
    }

    // spelers toevoegen aan array
    private void AddPlayers(int numberOfPlayers)
    {
        for (int i = 1; i <= numberOfPlayers; i++)
        {
            Console.WriteLine($"Enter player name {i}: ");
            string playerName = Console.ReadLine();
            _players.Add(new Player(playerName));
        }
    }

    public void DealInitialCards()
    {
        if (!AskForConfirmation("Dealer, deal the cards?"))
        {
            Console.WriteLine(" error ff huilen nu :(");
            return;
        }

        Console.WriteLine("\nDealer gives out the cards");

        foreach (var player in _players)
        {
            player.AddCard(_deck.DealCard());
            player.AddCard(_deck.DealCard());
        }

        _dealer.AddCard(_deck.DealCard());
        _dealer.AddCard(_deck.DealCard());

        ShowHands();
    }

    // print handen van dealer en spelers
    public void ShowHands()
    {
        Console.Clear();
        Console.WriteLine("\n-- Dealer's Hand --");
        Console.WriteLine(_dealer.ShowHand());
        Console.WriteLine("-- Dealer's Hand --");

        foreach (var player in _players)
        {
            Console.WriteLine("\n-- " + player.Name + "'s Hand --");
            Console.WriteLine(player.ShowHand());
            Console.WriteLine("-- " + player.Name + "'s Hand --");
        }
    }

    // main game loop
    public void Play()
    {
        // check voor blackjacks bij eerste 2 kaarten
        BlackjackCheck();

        foreach (var player in _players)
        {
            Console.WriteLine($"\n{player.Name}'s turn:");
            PlayerTurn(player);

            // busted en blackjack check
            if (player.HasBlackjack())
            {
                Console.WriteLine($"{player.Name} has Blackjack");
                continue;
            }
            else if (player.IsBusted())
            {
                Console.WriteLine($"{player.Name} busted");
                continue;
            }
        }

        Console.WriteLine("\nDealer's turn:");
        DealerTurn();
        Console.Clear();

        //handen van spelers laten zien
        ShowHands();

        // winaars bekent maken
        EvaluateWinner();

        // spell restarten
        ResetGame();
    }


    // speler hit of pass
    private void PlayerTurn(Player player)
    {
        string action;
        while (!player.IsBusted() && !player.HasBlackjack())
        {
            action = player.ChooseAction();

            if (action == "pass")
            {
                break;
            }
            else if (action == "hit")
            {
                player.AddCard(_deck.DealCard());
                ShowHands();
            }
        }
    }

    // dealer hit tot 17 anders pass
    private void DealerTurn()
    {
        while (_dealer.CalculateHandValue() < 17)
        {
            Console.WriteLine("\nDealer hits");
            _dealer.AddCard(_deck.DealCard());
            ShowHands();
        }

        if (_dealer.IsBusted())
        {
            Console.WriteLine("\nDealer busted, all players not busted win");
        }
        else
        {
            Console.WriteLine("Dealer passes \n");
        }
    }

    // game restart
    private void ResetGame()
    {
        _deck = new Deck();
        foreach (var player in _players)
        {
            player.Hand.Clear();
        }
        _dealer.Hand.Clear();
        _players.Clear();

        if (!AskForConfirmation("Dealer, restart game?"))
        {
            Console.WriteLine("buh byee-");
            return;
        }

        StartGame();
    }

    // dealer permissie vragen
    private bool AskForConfirmation(string message)
    {
        string response;
        while (true)
        {
            Console.WriteLine(message);
            Console.WriteLine("{ yes } { no }");

            response = Console.ReadLine().ToLower();

            if (response == "yes")
            {
                return true;
            }
            else if (response == "no")
            {
                return false;
            }
            else
            {
                Console.WriteLine(" error ff huilen nu :( ");
            }
        }
    }

    // check voor blackjacks bij eerste 2 kaarten
    public void BlackjackCheck()
    {
        if (_dealer.HasBlackjack())
        {
            Console.WriteLine("\nDealer has blackjack");

            bool playerHasBlackjack = false;
            foreach (var player in _players)
            {
                if (player.HasBlackjack())
                {
                    Console.WriteLine($"{player.Name} has blackjack! It's a tie with the dealer");
                    playerHasBlackjack = true;
                }
            }

            if (!playerHasBlackjack)
            {
                Console.WriteLine("All players lose");
            }

            ResetGame();
        }

        foreach (var player in _players)
        {
            if (player.HasBlackjack())
            {
                Console.WriteLine($"\n{player.Name} has Blackjack {player.Name} wins");
                return;
            }
        }
    }


    // winnaars bekent maken
    private void EvaluateWinner()
    {
        if (_dealer.IsBusted())
        {
            Console.WriteLine("\nDealer bustesd, players who did not bust win");
            foreach (var player in _players)
            {
                if (!player.IsBusted())
                {
                    Console.WriteLine($"{player.Name} wins");
                }
            }
            return;
        }

        foreach (var player in _players)
        {
            if (player.IsBusted())
            {
                Console.WriteLine($"{player.Name} busted");
            }
            else if (player.CalculateHandValue() > _dealer.CalculateHandValue())
            {
                Console.WriteLine($"{player.Name} wins");
            }
            else if (player.CalculateHandValue() < _dealer.CalculateHandValue())
            {
                Console.WriteLine($"{player.Name} loses");
            }
            else
            {
                Console.WriteLine($"{player.Name} tie with dealer");
            }
        }
    }
}
