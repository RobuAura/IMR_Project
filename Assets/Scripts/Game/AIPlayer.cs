using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPlayer : MonoBehaviour
{
    [Header("Settings")]
    public float thinkDelay = 0.8f;
    public int memorySize = 3; // Medium = 3, Hard = 4

    private List<Card> memory = new List<Card>();
    private MemoryGameManager gameManager;

    private Card lastCardA = null;
    private Card lastCardB = null;

    void Start()
    {
        gameManager = MemoryGameManager.Instance;
    }

    public void StartTurn()
    {
        StartCoroutine(PlayTurn());
    }

    IEnumerator PlayTurn()
    {
        yield return new WaitForSeconds(thinkDelay);

        CleanMemory();

        // Pasul 1: verifica daca are o pereche in memorie
        Card firstOfPair = FindPairInMemory(null);
        
        if (firstOfPair != null)
        {
            Card secondOfPair = FindPartner(firstOfPair);
            
            if (secondOfPair == null) yield break;

            if (!firstOfPair.IsMatched && !firstOfPair.IsFlipped)
                gameManager.AIFlipCard(firstOfPair);
            else yield break;

            yield return new WaitForSeconds(thinkDelay);

            if (!secondOfPair.IsMatched && !secondOfPair.IsFlipped)
                gameManager.AIFlipCard(secondOfPair);
        }
        else
        {
            Card cardA = GetRandomUnflippedCard();
            
            if (cardA == null)
            {
                yield break;
            }

            gameManager.AIFlipCard(cardA);
            yield return new WaitForSeconds(thinkDelay);

            Card pairForA = FindPairInMemory(cardA);

            if (pairForA != null && !pairForA.IsMatched && !pairForA.IsFlipped)
            {
                memory.Remove(pairForA);
                gameManager.AIFlipCard(pairForA);
            }
            else
            {
                Card cardB = GetRandomUnflippedCard(cardA);
                if (cardB == null) yield break;

                gameManager.AIFlipCard(cardB);
                lastCardA = cardA;
                lastCardB = cardB;
                yield return new WaitForSeconds(0.1f);
                UpdateMemory(cardA, cardB);
            }
        }
    }

    public void UpdateMemory(Card cardA, Card cardB)
    {
        int freeSlots = memorySize - memory.Count;

        if (freeSlots >= 2)
        {
            // Cel putin 2 locuri libere — adauga A si B direct
            memory.Add(cardA);
            memory.Add(cardB);
        }
        else if (freeSlots == 1)
        {
            // 1 loc liber — memoria are memorySize-1 carti (ex: X Y pentru memorySize=3)
            // Verifica daca B are pereche in memorie
            Card pairOfB = FindPairInMemory(cardB);

            if (pairOfB != null)
            {
                // B are pereche (sa zicem Y) — nu sterge Y, sterge X (cea care NU e perechea lui B)
                Card toRemove = FindOldestWithoutPairOf(cardB);
                if (toRemove != null) memory.Remove(toRemove);
            }
            else
            {
                // B nu are pereche — sterge cea mai veche (X), pastreaza Y
                memory.RemoveAt(0);
            }

            memory.Add(cardA);
            memory.Add(cardB);
        }
        else
        {
            // 0 locuri libere — memoria e plina (ex: X Y Z pentru memorySize=3)
            // Verifica daca B are pereche in memorie
            Card pairOfB = FindPairInMemory(cardB);

            if (pairOfB != null)
            {
                // B are pereche (sa zicem Z) — sterge celelalte doua, pastreaza Z
                List<Card> toRemove = new List<Card>();
                foreach (Card c in memory)
                {
                    if (c != pairOfB)
                        toRemove.Add(c);
                }
                // Sterge primele doua care nu sunt perechea lui B
                int removed = 0;
                foreach (Card c in toRemove)
                {
                    if (removed >= 2) break;
                    memory.Remove(c);
                    removed++;
                }
            }
            else
            {
                // B nu are pereche — sterge cele mai vechi doua (X si Y), pastreaza Z
                memory.RemoveAt(0);
                memory.RemoveAt(0);
            }

            memory.Add(cardA);
            memory.Add(cardB);
        }
    }

    // Gaseste cea mai veche carte din memorie care NU e perechea cartii date
    Card FindOldestWithoutPairOf(Card card)
    {
        foreach (Card c in memory)
        {
            if (c.CardId != card.CardId)
                return c;
        }
        return memory[0]; // fallback
    }

    // Sterge din memorie cartile care au fost matched
    public void CleanMemory()
    {
        memory.RemoveAll(c => c == null || c.IsMatched);
    }

    // Gaseste orice pereche in memorie (card == null) sau perechea unui card specific
    Card FindPairInMemory(Card card)
    {
        if (card == null)
        {
            for (int i = 0; i < memory.Count; i++)
            {
                for (int j = i + 1; j < memory.Count; j++)
                {
                    if (memory[i] != memory[j] &&
                        memory[i].CardId == memory[j].CardId &&
                        !memory[i].IsMatched && !memory[j].IsMatched &&
                        !memory[i].IsFlipped && !memory[j].IsFlipped)
                        return memory[i];
                }
            }
            return null;
        }
        else
        {
            foreach (Card c in memory)
            {
                if (c != card && c.CardId == card.CardId &&
                    !c.IsMatched && !c.IsFlipped)
                    return c;
            }
            return null;
        }
    }

    // Gaseste partenerul unei carti (stim deja ca exista pereche)
    Card FindPartner(Card card)
    {
        foreach (Card c in memory)
        {
            if (c != card && c.CardId == card.CardId &&
                !c.IsMatched && !c.IsFlipped)
                return c;
        }
        return null;
    }

    Card GetRandomUnflippedCard(Card exclude = null)
    {
        List<Card> available = new List<Card>();

        foreach (Transform child in gameManager.gridLayoutGroup.transform)
        {
            Card c = child.GetComponent<Card>();
            if (c != null && !c.IsFlipped && !c.IsMatched
                && c != exclude
                && c != lastCardA
                && c != lastCardB)
                available.Add(c);
        }

        // Daca nu mai sunt carti disponibile dupa excludere, ignora excluderea
        if (available.Count == 0)
        {
            return GetRandomUnflippedCardNoExclusion(exclude);
        }

        return available[Random.Range(0, available.Count)];
    }

    Card GetRandomUnflippedCardNoExclusion(Card exclude = null)
    {
        List<Card> available = new List<Card>();

        foreach (Transform child in gameManager.gridLayoutGroup.transform)
        {
            Card c = child.GetComponent<Card>();
            if (c != null && !c.IsFlipped && !c.IsMatched && c != exclude)
                available.Add(c);
        }

        if (available.Count == 0) return null;
        return available[Random.Range(0, available.Count)];
    }
}