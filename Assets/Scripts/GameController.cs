using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public BoardController currentBoard;
    public GameObject tilePlain, cardPlain, background;
    public Sprite card, _0, _1, _2, _3, _5, _7, _11, _13, _17, _19, _23, _29, _31, _37,
                      _41, _43, _47, _53, _59, _61, _67, _73, _79, _83, _89, _97, _x;
    Dictionary<string, Sprite> tileSprites;
    public InputField number;
    public int[,] results;
    public ParticleSystem midParticles, startParticles, endParticles;
    public TMPro.TextMeshPro winText;

    bool dragging, particlesOn;
    int currentRow, currentCol, currentPrime;
    int[] factors, primes;
    GameObject selected;
    List<CardController> cards;
    string[][] eqArray;
    Vector3 lineStart, lineEnd, mousePosition;
    ParticleSystem.ShapeModule shape;
    float particlesLength;
    public CardController cardPrefab;

    // Use this for initialization
    void Start()
    {
        LoadTileSprites();
        cards = new List<CardController>();
        currentBoard = Instantiate(currentBoard, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        particlesOn = false;
        shape = midParticles.shape;
        TurnParticlesOn(false);
        background.SetActive(true);
    }

    void LoadTileSprites()
    {
        tileSprites = new Dictionary<string, Sprite>();
        tileSprites.Add("0",_0);
        tileSprites.Add("1", _1);
        tileSprites.Add("2", _2);
        tileSprites.Add("3", _3);
        tileSprites.Add("5", _5);
        tileSprites.Add("7", _7);
        tileSprites.Add("11", _11);
        tileSprites.Add("13", _13);
        tileSprites.Add("17", _17);
        tileSprites.Add("19", _19);
        tileSprites.Add("23", _23);
        tileSprites.Add("29", _29);
        tileSprites.Add("31", _31);
        tileSprites.Add("37", _37);
        tileSprites.Add("41", _41);
        tileSprites.Add("43", _43);
        tileSprites.Add("47", _47);
        tileSprites.Add("53", _53);
        tileSprites.Add("59", _59);
        tileSprites.Add("61", _61);
        tileSprites.Add("67", _67);
        tileSprites.Add("73", _73);
        tileSprites.Add("79", _79);
        tileSprites.Add("83", _83);
        tileSprites.Add("89", _89);
        tileSprites.Add("97", _97);
        tileSprites.Add("x", _x);
}

private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selected = GetHit();
            
            if (selected != null && selected.GetComponent<TileController>() != null)
            {
                //Debug.Log("Playing " + selected);
                //selected.GetComponent<TileController>().particles.Play();
            }
            else
            {
                //Debug.Log("Not playing anything because selected is " + selected);
            }

            if (selected != null)
            {
                HandleParticles(true);
            }
            //Debug.Log("lineStart = " + lineStart);
        }
        else if (Input.GetMouseButton(0))
        {
            if (particlesOn)
            {
                HandleParticles(false);
            }
        }
        else if (dragging)
        {
            dragging = false;
            
            TurnParticlesOn(false);
            GameObject newObject = GetHit();
            //Debug.Log("selected = " + selected);
            if (newObject)
            {
                if (newObject.GetComponent<ParticleSystem>())
                {
                    newObject.GetComponent<ParticleSystem>().Stop();
                }

                if (selected.transform.position.x < newObject.transform.position.x)
                {
                    OnTilesLinked(selected, newObject);
                }
                else
                {
                    OnTilesLinked(newObject, selected);
                }
                //Debug.Log("Now to distribute sides");
                currentBoard.DistributeSides();
            }
            else if(selected.GetComponent<CardController>() 
                && !SameSide(selected.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                if (selected.transform.position.y > currentBoard.divider.transform.position.y)
                {
                    MoveCard(selected.GetComponent<CardController>(), 0, 1);
                }
                else
                {
                    MoveCard(selected.GetComponent<CardController>(), 1, 0);
                }
                currentBoard.DistributeSides();
            }

            selected = null;

            //Debug.Log("num columns = " + currentBoard.GetSide(0).NumColumns());
            //Debug.Log("num child cards = " + currentBoard.GetSide(0).GetColumn(0).GetCard().GetChildCards().Count);
            //Debug.Log("num tiles = " + currentBoard.GetSide(0).GetColumn(0).GetCard().GetTiles().Count);
            //Debug.Log("number = " + currentBoard.GetSide(0).GetColumn(0).GetCard().GetTile(0).number);

            if (currentBoard.GetSide(0).NumColumns() == 1
               && currentBoard.GetSide(0).GetColumn(0).GetCard()
               && currentBoard.GetSide(0).GetColumn(0).GetCard().GetChildCards().Count == 0
               && currentBoard.GetSide(0).GetColumn(0).GetCard().GetTiles().Count == 1
               && currentBoard.GetSide(0).GetColumn(0).GetCard().GetTile(0).number == int.MinValue)
            {
                //Debug.Log("Win!");
                winText.transform.Translate(0f, 100f, 0f);
            }
            else
            {
                //Debug.Log("No Win!");
            }
            
        }
    }

    void MoveCard(CardController card, int currentSide, int newSide)
    {
        if (!card.GetParentCard())
        {
            currentBoard.GetSide(newSide).AddColumn(card.GetColumn(), false);
            card.FlipSign();
            card.DistributeCards();
            card.DistributeTiles();
        }
    }

    bool SameSide(Vector3 pos0, Vector3 pos1)
    {
        float dividerY = currentBoard.divider.transform.position.y;

        if ((pos0.y > dividerY && pos1.y > dividerY)
             ||
            (pos0.y < dividerY && pos1.y < dividerY))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void TurnParticlesOn(bool val)
    {
        if (val)
        {
            startParticles.Play();
            endParticles.Play();
            midParticles.Play();
            particlesOn = true;
        }
        else
        {
            startParticles.Stop();
            endParticles.Stop();
            midParticles.Stop();
            particlesOn = false;
        }
    }

    void HandleParticles(bool newLine)
    {
        if (newLine)
        {
            lineStart = GetMousePositionWorld(0);
            //Debug.Log("lineStart = " + lineStart);
            lineEnd = lineStart;
            dragging = true;
            TurnParticlesOn(true);
        }
        else
        {
            lineEnd = GetMousePositionWorld(0);
        }

        particlesLength = Vector2.Distance(lineStart, lineEnd);
        shape.radius = particlesLength / 2f;

        startParticles.transform.position = lineStart;
        endParticles.transform.position = lineEnd;
        midParticles.transform.position = (lineStart + lineEnd) / 2f;
        midParticles.transform.LookAt(lineStart);

        if (lineStart.x < lineEnd.x)
        {
            midParticles.transform.localEulerAngles = new Vector3(0f,
                                                            0f,
                                                            midParticles.transform.localEulerAngles.x);
        }
        else
        {
            midParticles.transform.localEulerAngles = new Vector3(0f,
                                                            0f,
                                                            -midParticles.transform.localEulerAngles.x);
        }
    }
    
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTilesLinked(GameObject one, GameObject two)
    {
        /* Purpose: called whenever the user links two tiles
         * Parameters: one and two, should be game objects with TileController scripts
         */

        //Debug.Log("OnTilesLinked: Start");

        //We need to make sure these two objects are actually tiles
        //So, try to access their TileControllers
        TileController thisTile = one.GetComponent<TileController>();
        TileController otherTile = two.GetComponent<TileController>();

        //Debug.Log("Linking two tiles " + thisTile + " and " + otherTile);

        //If both turned out to be tiles . . .
        if (thisTile != null && otherTile != null)
        {
            //We now need to check which cards the two tiles belong to
            //since you can't combine or cancel tiles on the same card
            CardController otherCard = otherTile.GetCard();
            CardController thisCard = thisTile.GetCard();
            
            //If both tiles belong to cards
            if (otherCard != null && thisCard != null)
            {
                //Next, we need to find out if the cards that the two tiles belong
                //to themselves belong to even larger cards.
                CardController thisParentCard = thisCard.GetParentCard();
                CardController otherParentCard = otherCard.GetParentCard();

                //If the two cards are different
                //and if the two tiles represent the same prime factor
                if (!otherCard.Equals(thisCard)
                    && otherTile.number == thisTile.number)
                {
                    //If the numbers are on the same side of the equation
                    //we'll need to factor out the common factor but not 
                    //simply "delete it" from the equation
                    //Debug.Log(thisTile.GetSide());
                    //Debug.Log(otherTile.GetSide());
                    if (thisTile.GetSide().Equals(otherTile.GetSide()))
                    {
                        //If either the two cards do not themselves belong to 
                        //an even larger card or they both belong to the same card
                        //Debug.Log("Here we are");
                        //Debug.Log("thisParentCard = " + thisParentCard);
                        //Debug.Log("otherParentCard = " + otherParentCard);
                        if ((thisParentCard == null
                             && otherParentCard == null)
                            ||
                            (thisParentCard != null
                             && thisParentCard.Equals(otherParentCard)))
                        {
                            //Debug.Log("Removing tile other " + otherTile.name);

                            //Then we've found a common factor and can extract it!
                            //We need to remove the common tile from both the original cards
                            //Debug.Log(name + " need to remove other card " + otherCard + " and otherTile = " + otherTile);
                            otherCard.RemoveTile(otherTile, false);
                            //if(thisCard.GetChildCards().Count == 0)
                            //{
                                //Debug.Log("Removing tile this " + thisTile);
                                thisCard.RemoveTile(thisTile, true);
                            //}

                            //Debug.Log("Our two parent cards are " + thisParentCard + " and " + otherParentCard);
                            //If the larger card doesn't exist, we need to create it
                            if (thisParentCard == null
                                && otherParentCard == null)
                            {
                                //if (thisCard.GetChildCards().Count == 0)
                                //{
                                    //We need to add the two cards to a new common card
                                    //Debug.Log("Creating a new card");
                                    List<TileController> commonTiles = new List<TileController>();
                                    commonTiles.Add(otherTile);
                                    List<CardController> childCards = new List<CardController>();
                                    foreach(CardController c in thisCard.GetChildCards())
                                    {
                                        childCards.Add(c);
                                    }
                                    thisCard.MergeWith(otherCard, childCards, commonTiles);
                                //}
                                /*else
                                {
                                    thisCard.AddChildCard(otherCard);
                                    thisCard.ResizeTiles();
                                }
                                */
                            }
                            else if (thisParentCard != null
                                     && thisParentCard.Equals(otherParentCard))
                            {
                                //We just need to add the common factor to the tiles in
                                //the larger card to which the two cards belong.
                                Debug.Log("Adding extra tile");
                                thisParentCard.AddTile(otherTile);
                                thisParentCard.AdjustCardSizeAndContents();
                                if (thisParentCard.GetParentCard())
                                {
                                    thisParentCard.GetParentCard().AdjustCardSizeAndContents();
                                }
                            }
                            //Debug.Log("Now to distribute Columns");
                            thisTile.GetSide().DistributeColumns();
                        }
                    }
                    else if(thisTile.GetSide().NumColumns() == 1
                            && otherTile.GetSide().NumColumns() == 1)
                    {
                        //If either the two cards do not themselves belong to 
                        //an even larger card or they both belong to the same card
                        if (thisParentCard == null
                             && otherParentCard == null)
                        {
                            //Debug.Log("Removing tile other");

                            //Then we've found a common factor and can extract it!
                            //We need to remove the common tile from both the original cards
                            //Debug.Log("Removing tiles");
                            otherCard.RemoveTile(otherTile, true);
                            //Debug.Log("Removing tile this");
                            thisCard.RemoveTile(thisTile, true);
                            if(otherCard.GetTiles().Count > 0)
                            {
                                //Debug.Log("Adjusts other");
                                otherCard.AdjustCardSizeAndContents();
                            }
                            else if(otherCard.GetChildCards().Count > 0)
                            {
                                otherCard.GetColumn().FreeChildCards();
                            }
                            if (thisCard.GetTiles().Count > 0)
                            {
                                //Debug.Log("Adjusts this");
                                thisCard.AdjustCardSizeAndContents();
                            }
                            else if (thisCard.GetChildCards().Count > 0)
                            {
                                thisCard.GetColumn().FreeChildCards();
                            }
                        }
                    }

                }   
            }
            currentBoard.GetSide(0).DistributeColumns();
            currentBoard.GetSide(1).DistributeColumns();
        }
        else //Now to check if these are cards
        {
            //We need to make sure these two objects are actually tiles
            //So, try to access their TileControllers
            CardController thisCard = one.GetComponent<CardController>();
            CardController otherCard = two.GetComponent<CardController>();

            if(thisCard && otherCard)
            {
                int newCardValue = thisCard.GetValue() + otherCard.GetValue();
                Debug.Log(thisCard.GetValue() + " + " + otherCard.GetValue() + " = " + newCardValue);
                if(thisCard.transform.position.x < otherCard.transform.position.x)
                {
                    otherCard.GetSide().RemoveCard(otherCard, true);
                    thisCard.RemoveTiles();
                    AddTilesToThisCard(thisCard, newCardValue);
                }
                else
                {
                    thisCard.GetSide().RemoveCard(thisCard, true);
                    otherCard.RemoveTiles();
                    AddTilesToThisCard(otherCard, newCardValue);
                }
            }
        }
    }

    private GameObject GetHit()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit){
            //Debug.Log(Time.frameCount + " Hit " + hit.collider);
            return hit.collider.gameObject;
        }
        else
        {
            //Debug.Log("Hit nothing");
        }
        return null;
    }

    public void CallFindFactors()
    {
        //Debug.Log("CallFindFactors: Start");
        ParseText(number.text);
        GenerateOutput();
        currentBoard.DistributeSides();
    }

    void ParseText(string text)
    {
        string[] eqParsed = text.Split(' ');
        eqArray = new string[eqParsed.Length][];
        int num;
        for(int i = 0; i < eqParsed.Length; i++)
        {
            if (int.TryParse(eqParsed[i], out num))
            {
                //Debug.Log(Time.frameCount + "factoring " + eqParsed[i]);
                eqArray[i] = FindFactors(int.Parse(eqParsed[i]), true);
            }
            else
            {
                //Debug.Log(Time.frameCount + "Can't factor " + eqParsed[i]);
                eqArray[i] = new string[] { eqParsed[i] };
            }
        }

        for(int i = 0; i < eqArray.Length; i++)
        {
            //Debug.Log("Checking: i = " + i);
            string result = "";
            foreach(string s in eqArray[i])
            {
                result += s + ",";
            }
            //Debug.Log(result);
            if (int.TryParse(eqArray[i][0], out num))
            {
                //Debug.Log(Time.frameCount + "Integer! = " + num);
                eqArray[i] = GetFactorMultiples(ConvertStringToInt(eqArray[i]));
            }     
        }

    }

    private int[] ConvertStringToInt(string[] stringArray)
    {
        //Debug.Log(Time.frameCount + "ConvertStringToInt: " + stringArray);
        int[] result = new int[stringArray.GetLength(0)];
        int num;
        for(int i = 0; i < stringArray.GetLength(0); i++)
        {
            //Debug.Log(Time.frameCount + "Trying to convert " + stringArray[i]);
            if (int.TryParse(stringArray[i], out num))
            {
                result[i] = int.Parse(stringArray[i]);
            }
        }

        return result;
    }

    public void LoadPrimes(int primesnumber)
    {
        primes = new int[0];

        for (int i = 2; i <= primesnumber; i++)
        {
            if (IsPrime(i))
            {
                int[] temp = new int[primes.Length + 1];
                System.Array.Copy(primes, temp, primes.Length);
                primes = temp;
                primes[primes.Length - 1] = i;
            }
        }
    }

    public bool IsPrime(int n)
    {
        //Debug.Log(Time.frameCount + "IsPrime: checking " + n);
        if ((n == 0) || (n == 1))
        {
            return false;
        }
        else if (n == 2)
        {
            //Debug.Log(Time.frameCount + "IsPrime: returning true");
            return true;
        }
        else if (n % 2 == 0)
        {
            //Debug.Log(Time.frameCount + "IsPrime: returning false");
            return false;
        }
        //if not, then just check the odds
        for (int i = 3; i * i <= n; i += 2)
        {
            if (n % i == 0)
            {
                //Debug.Log(Time.frameCount + "IsPrime: returning false");
                return false;
            }
        }
        //Debug.Log(Time.frameCount + "IsPrime: returning true");
        return true;
    }

    public string[] FindFactors(int currentNum, bool increaseColumn)
    {
        //Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        //Debug.Log(Time.frameCount + "FindFactors: Start with number = " + currentNum);
        LoadPrimes(currentNum);
        factors = new int[currentNum];

        string output = "";
        currentCol = 0;
        bool go = true;

        int result = currentNum;

        while (go)
        {
            result = DivideXbyNextPrime(result);
            if (result <= 1)
            {
                go = false;
            }
        }

        for (int i = 0; i < factors.Length; i++)
        {
            output += factors[i] + " ";
        }

        //Debug.Log(Time.frameCount + "Returning " + output);
        //Debug.Log(Time.frameCount + "");
        if (increaseColumn)
        {
            currentRow++;
        }
        
        return output.Split(' ');
    }

    public int DivideXbyNextPrime(int x)
    {
        //Debug.Log(Time.frameCount + "DivideXbyNextPrime: on " + x + " with currentRow = " + currentRow + " and currentCol = " + currentCol);
        currentPrime = 0;

        while (true)
        {
            //Debug.Log(Time.frameCount + "DivideXbyNextPrime: current prime = " + primes[currentPrime]);
            if (x % primes[currentPrime] == 0)
            {
                //Debug.Log(Time.frameCount + "DivideXbyNextPrime: " + x + " was divisible by " + primes[currentPrime]);
                //Debug.Log(Time.frameCount + "DivideXbyNextPrime: setting factors[" + currentCol + "] to " + primes[currentPrime]);
                factors[currentCol] = primes[currentPrime];
                currentCol++;

                int result = x / primes[currentPrime];
                //Debug.Log(Time.frameCount + "DivideXbyNextPrime: returning " + result);
                return result;
            }
            else
            {
                currentPrime++;
                //Debug.Log(Time.frameCount + "DivideXbyNextPrime: currentPrime = " + currentPrime + ", while primes.Length = " + primes.Length);
                if (currentPrime >= primes.Length || primes[currentPrime] > x)
                {
                    currentCol = 0;
                    int result = -1;
                    //Debug.Log(Time.frameCount + "DivideXbyNextPrime: returning -1");
                    return result;
                }
            }
        }
    }

    public string[] GetFactorMultiples(int[] tempFactors)
    {
        //Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        //Debug.Log("GetFactorMultiples: Start with primes length = " + primes.Length);
        int[,] primesAndExponents = new int[primes.Length, 2];

        currentPrime = 0;

        for (int i = 0; i < factors.Length; i++)
        {
            //Debug.Log(Time.frameCount + "GetFactorMultiples: i = " + i);
            //Debug.Log(Time.frameCount + "currentPrime = " + currentPrime);
            //Debug.Log(Time.frameCount + "while there are " + primes.Length + " primes");
            //Debug.Log(Time.frameCount + "which is " + primes[currentPrime]);
            //Debug.Log(Time.frameCount + "and factors[" + i + "] = " + factors[i]);
            if (primes[currentPrime] == tempFactors[i])
            {
                primesAndExponents[currentPrime, 0] = primes[currentPrime];
                primesAndExponents[currentPrime, 1]++;
            }
            else if (tempFactors[i] == 0)
            {
                break;
            }
            else
            {
                currentPrime++;
                i--;
            }
        }

        results = new int[0, 2];
        string exponents = "";

        for (int i = 0; i < primesAndExponents.GetLength(0); i++)
        {
            exponents += primesAndExponents[i, 1] + ",";
            if (primesAndExponents[i, 1] != 0)
            {
                int[,] temp = new int[results.GetLength(0) + 1, 2];
                for (int j = 0; j < results.GetLength(0); j++)
                {
                    temp[j, 0] = results[j, 0];
                    temp[j, 1] = results[j, 1];
                }
                results = temp;
                results[(results.GetLength(0) - 1), 0] = primesAndExponents[i, 0];
                results[(results.GetLength(0) - 1), 1] = primesAndExponents[i, 1];
            }
        }
        //Debug.Log("Exponents: " + exponents);
        string[] stringResults = new string[results.GetLength(0) * 2];
        int index = 0;
        foreach(int entry in results)
        {
            stringResults[index] = entry.ToString();
            index++;
        }
        
        return stringResults;
    }

    public void GenerateOutput()
    {
        GenerateTextOutput();

        int counter = 0;
        int side = 0;

        for(int i = 0; i < eqArray.Length; i++)
        {
            //Debug.Log("GenerateOutput: i = " + i);
            int tryInt;
            for (int j = 0; j < eqArray[i].Length; j+=2)
            {
                //Debug.Log("GenerateOutput: j = " + j);
                GameObject tempTile = Instantiate(tilePlain);
                if(eqArray[i][j] == "x")
                {
                    //Debug.Log("We have an x");
                    tempTile.GetComponent<TileController>().number = int.MinValue;
                }
                else if (int.TryParse(eqArray[i][j], out tryInt))
                {
                    //Debug.Log("We have " + eqArray[i][j]);
                    tempTile.GetComponent<TileController>().number = tryInt;
                }

                //Debug.Log("Interpreting " + eqArray[i][j]);

                switch (eqArray[i][j])
                {
                    case "2":
                    case "3":
                    case "5":
                    case "7":
                    case "11":
                    case "13":
                    case "17":
                    case "19":
                    case "23":
                    case "29":
                    case "31":
                    case "37":
                    case "41":
                    case "43":
                    case "47":
                    case "53":
                    case "59":
                    case "61":
                    case "67":
                    case "73":
                    case "79":
                    case "83":
                    case "89":
                    case "97":
                        tempTile.GetComponent<SpriteRenderer>().sprite = tileSprites[eqArray[i][j]];
                        AddTileColumn(tempTile, counter, int.Parse(eqArray[i][j + 1]), side);
                        break;
                    case "x":
                        counter--;
                        tempTile.GetComponent<SpriteRenderer>().sprite = tileSprites[eqArray[i][j]];
                        AddTileColumn(tempTile, counter, 1, side);
                        break;
                    case "=":
                        counter = -1;
                        side = 1;
                        Destroy(tempTile);
                        break;
                    default:
                        counter--;
                        Destroy(tempTile);
                        break;
                }
                
                //Debug.Log("Done adding " + tempTile.name);
            }
            counter++;
        }

        //Debug.Log("Now to get cards");
        GetCards();

        foreach(CardController card in cards)
        {
            //Debug.Log("Now to distribute Tiles on " + card);
            card.DistributeTiles();
            card.ResizeX();
            card.ResizeY();
        }
        currentBoard.DistributeSides();
    }

    void GetCards()
    {
        CardController[] tempCards = GetComponentsInChildren<CardController>();
        foreach (CardController card in tempCards)
        {
            //Debug.Log("Checking " + card + " to see if it's a card");
            cards.Add(card);
        }
    }

    void AddTileColumn(GameObject tile, int column, int times, int side)
    {
        //Debug.Log(Time.frameCount + "AddTile: " + tile.name + ", column = " + column + ", " + times + " times" + " side = " + side);
        SideController currentSide = currentBoard.GetSide(side);

        //Debug.Log("Current side = " + currentSide.name);

        if(column >= currentSide.NumColumns())
        {
            //Debug.Log("Adding a new column");
            currentSide.AddColumn();
        }

        ColumnController currentColumn = currentSide.GetCurrentColumn();

        //Debug.Log(Time.frameCount + "AddTile: currentColumn = " + currentColumn);
        CardController currentCard = currentColumn.GetCard();

        for (int i = 0; i < times; i++)
        {
            if(i != 0)
            {
                tile = Instantiate(tile);
            }
            tile.name = tile.GetComponent<TileController>().number + " " + Random.Range(0.1f, 100f);
            //Debug.Log(Time.frameCount + "AddTile: adding tile " + tile + " " + tile.transform.lossyScale);
            currentCard.AddTile(tile.GetComponent<TileController>());
        }
    }

    bool HasCommonFactors(int[] array0, int[] array1)
    {
        bool result = false;

        foreach(int i in array0)
        {
            for(int j = 0; j < array1.Length; j++)
            {
                if(i == array1[j])
                {
                    return true;
                }
            }
        }

        return result;
    }

    int[] ListCommonFactors(int[] array0, int[] array1)
    {
        int[] result = new int[0];

        foreach (int i in array0)
        {
            for (int j = 0; j < array1.Length; j++)
            {
                if (i == array1[j])
                {
                    int[] temp = new int[result.Length + 1];
                    System.Array.Copy(result, temp, result.Length);
                    result = temp;
                    result[result.Length - 1] = i;
                }
            }
        }

        return result;
    }

    int[] ListCommonFactorsUnique(int[] array0, int[] array1)
    {
        int[] result = new int[0];

        foreach (int i in array0)
        {
            for (int j = 0; j < array1.Length; j++)
            {
                if (i == array1[j] && System.Array.IndexOf(result, i) == -1)
                {
                    int[] temp = new int[result.Length + 1];
                    System.Array.Copy(result, temp, result.Length);
                    result = temp;
                    result[result.Length - 1] = i;
                }
            }
        }

        return result;
    }

    void GenerateTextOutput()
    {
        string textOutput = "";

        for (int i = 0; i < results.GetLength(0); i++)
        {
            if (textOutput != "")
            {
                textOutput += "*";
            }
            textOutput += results[i, 0];
            textOutput += "^";
            textOutput += results[i, 1];
        }

        //Debug.Log(Time.frameCount + textOutput);
    }

    void CreateNewCardWithNumber(int newNumber)
    {
        string[] tempFactors = FindFactors(newNumber, false);
        CardController newCard = Instantiate(cardPrefab,
                                              new Vector3(0f, 0f, 0f),
                                              Quaternion.identity,
                                              transform);
        AddTilesToCard(newCard, tempFactors);
    }

    void AddTilesToThisCard(CardController thisCard, int newNumber)
    {
        string[] tempFactors = FindFactors(newNumber, false);
        AddTilesToCard(thisCard, tempFactors);
    }

    void AddTilesToCard(CardController thisCard, string[] tempFactors)
    {
        int tempInt;
        foreach (string s in tempFactors)
        {
            //Debug.Log("AddTilesToCard: Adding " + s);
            if (int.TryParse(s, out tempInt) && tempInt > 0)
            {
                GameObject tempTile = Instantiate(tilePlain);
                tempTile.name = tempInt + " " + Random.Range(0.1f, 100f);
                tempTile.GetComponent<SpriteRenderer>().sprite = tileSprites[s];

                thisCard.AddTile(tempTile.GetComponent<TileController>());
            }
            
        }
        thisCard.DistributeTiles();
        thisCard.ResizeY();
    }

    Vector3 GetMousePositionWorld(int option)
    {
        switch (option)
        {
            case 0: //The normal case, where we don't need to adjust things
                return Camera.main.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x,
                                Input.mousePosition.y,
                                Camera.main.nearClipPlane
                                ));
            case 1:
                /* The case where we have stopped dragging the mouse and need to adjust
			        * the destination so that the piece appears to end up where our mouse appeard
			        * to be */
                //Get the center of the screen
                //float centerX = Camera.main.pixelWidth / 2;
                //float centerY = Camera.main.pixelHeight / 2;

                //Find out how far off the mouse was from the center in both the x and y directions
                //float xOffset = (centerX - Input.mousePosition.x) / centerX;
                //float yOffset = (centerY - Input.mousePosition.y) / centerY;

                /*	The closer to an edge of the screen we are, the more perspective effects are
                 *	noticeable. So, when close to the edge, we need to get a point on the ray
                 *	from the camera to the mouse point that is closer to the camera (and therefore
                 *	appears to be closer to the center of the screen). Otherwise, pieces that are
                 *	"on screen" when x and y are taken into account will end up offscreen due to 
                 *	their z placement. Multiplying the distance between the camera and the object
                 *	by 1 would keep it the same; multiplying by 1 - offset would shorten the
                 *	distance more and more as the offset grows (which is what we want). However,
                 *	we divide the offset by 4.5 simply because subtracting the full offset ends
                 *	up being too dramatic, and 4.5 turned out produce the most visually accurate
                 *	results.
                 */
                //Use the larger of the two offsets

                return Camera.main.ScreenToWorldPoint(
                                new Vector3(Input.mousePosition.x,
                                Input.mousePosition.y,
                                transform.position.y));
            case 2:
                return Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            default:
                return Camera.main.WorldToScreenPoint(transform.position);
        }
    }
}
