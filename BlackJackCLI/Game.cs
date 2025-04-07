using System;
using System.Collections.Generic;

public class BlackjackGame
{
    private Deck _deck;
    private List<Player> _players;
    private Player _dealer;
    private int DealerPenaltyPoints = 0;

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

        CollectBets();
        DealInitialCards();
        Play();
    }

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
            Console.WriteLine($"Balance: ${player.Money}");
            Console.WriteLine("-- " + player.Name + "'s Hand --");
        }
    }

    public void Play()
    {
        BlackjackCheck();

        foreach (var player in _players)
        {
            Console.WriteLine($"\n{player.Name}'s turn:");
            PlayerTurn(player);

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

        ShowHands();
        EvaluateWinner();

        Console.WriteLine($"\nDealer has {DealerPenaltyPoints} penalty point(s).");

        ResetGame();
    }

    private void CollectBets()
    {
        Console.Clear();
        Console.WriteLine("Dealer, collect bets from all players.\n");

        foreach (var player in _players)
        {
            if (player.Money <= 0)
            {
                Console.WriteLine($"{player.Name} is broke and can't bet.");
                continue;
            }

            Console.WriteLine($"{player.Name}, you have ${player.Money}. Enter your bet:");
            int bet;

            while (true)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out bet) && bet > 0 && bet <= player.Money)
                {
                    player.CurrentBet = bet;
                    player.Money -= bet;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid bet. Try again.");
                }
            }
        }
    }

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
                if (AskForConfirmation($"Dealer, give {player.Name} a card?"))
                {
                    player.AddCard(_deck.DealCard());
                    ShowHands();

                    if (player.IsBusted())
                    {
                        DealerPenaltyPoints++;
                        Console.WriteLine($"Dealer penalty! {player.Name} busted. Total penalties: {DealerPenaltyPoints}");
                    }
                }
                else
                {
                    Console.WriteLine("Dealer denied the hit. Skipping player’s turn.");
                    break;
                }
            }
        }
    }

    private void DealerTurn()
    {
        while (_dealer.CalculateHandValue() < 17)
        {
            if (AskForConfirmation("Dealer, draw a card?"))
            {
                Console.WriteLine("\nDealer hits");
                _dealer.AddCard(_deck.DealCard());
                ShowHands();

                if (_dealer.IsBusted())
                {
                    DealerPenaltyPoints++;
                    Console.WriteLine($"Dealer busted! Penalty point given. Total penalties: {DealerPenaltyPoints}");
                }
            }
            else
            {
                Console.WriteLine("Dealer passed on drawing a card.");
                break;
            }
        }

        if (!_dealer.IsBusted())
        {
            Console.WriteLine("Dealer passes \n");
        }
    }

    private void ResetGame()
    {
        _deck = new Deck();
        foreach (var player in _players)
        {
            player.Hand.Clear();
            player.CurrentBet = 0;
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

    private void EvaluateWinner()
    {
        // Prompt for confirmation before showing results
        if (!AskForConfirmation("Dealer, do you want to show the results?"))
        {
            Console.WriteLine("Dealer declined to show results.");
            return;
        }

        Console.Clear(); // Clear the console before showing the results

        if (_dealer.IsBusted())
        {
            Console.WriteLine("\nDealer busted! Players who didn’t bust win:");
            foreach (var player in _players)
            {
                if (!player.IsBusted())
                {
                    int winnings = player.CurrentBet * 2;
                    Console.WriteLine($"{player.Name} wins and earns ${winnings}!");
                    player.Money += winnings;
                }
                else
                {
                    Console.WriteLine($"{player.Name} busted and lost ${player.CurrentBet}.");
                }

                player.CurrentBet = 0;
            }
            return;
        }

        foreach (var player in _players)
        {
            if (player.IsBusted())
            {
                Console.WriteLine($"{player.Name} busted and lost ${player.CurrentBet}.");
            }
            else
            {
                int playerValue = player.CalculateHandValue();
                int dealerValue = _dealer.CalculateHandValue();

                if (playerValue > dealerValue)
                {
                    int winnings = player.CurrentBet * 2;
                    Console.WriteLine($"{player.Name} wins and earns ${winnings}!");
                    player.Money += winnings;
                }
                else if (playerValue < dealerValue)
                {
                    Console.WriteLine($"{player.Name} loses their bet of ${player.CurrentBet}.");
                }
                else
                {
                    Console.WriteLine($"{player.Name} ties with dealer. Gets back ${player.CurrentBet}.");
                    player.Money += player.CurrentBet;
                }
            }

            player.CurrentBet = 0;
        }
    }

}
