using System.Collections;
using System.Linq;
using UnityEngine;
using Managers;
using TMPro;
using UnityEngine.SceneManagement;

public class GameLoopBehaviour : MonoBehaviour
{
    TouchPhase touchPhase = TouchPhase.Ended;

    BombProperties bombProperties;

    CounterBehaviour counterBehaviour;

    private GameObject[] bombs;


    int difficulty;

    GameInfo gameInfo;

    GameRecords gameRecords = new GameRecords();

    [SerializeField]
    private TMP_Text tmpT;

    [SerializeField]
    private TMP_Text instruction;
    /*
     *  IF the player is the solver, this value will be true
     */
    bool solver;

    [SerializeField]
    private GameObject generatePosition;

    [SerializeField]
    private GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        InstantiateBomb();
        gameInfo = MainManager.Instance.gameManager.currentGameInfo;
        difficulty = MainManager.Instance.gameManager.currentGameInfo.difficulty;
        solver = gameInfo.localPlayerId == gameInfo.solverId ? true : false;
        gameRecords.gameInfo = gameInfo;
        if (difficulty > 2)

            difficulty = 2;
        if (solver)
        {
            //For solver, it is noly needed to update the material once 
            bombProperties.UpdateMaterial(solver);
            tmpT.text = "Player type: Solver";
            instruction.text = "Instruction:\n- click the Buttom  to Solve The Bomb\n- Do NOT click the wrong one";
        }
        else
        {
            bombProperties.UpdateMaterial(solver);
            tmpT.text = "Player type: Instructor";
            instruction.text = "Instruction:\n- green is clickable, red is not clickable\n- Tell the information to the solver";

            bombProperties.ListenForButton(gameInfo.gameId, () =>
            {
                //check if current button solve the bomb

                //checkWinState();

            }, Debug.Log);


            bombProperties.ListenForLose(gameInfo.gameId, () => { Explode(); }, Debug.Log);


        }
    }

    void Update()
    {
        if (solver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                selectObject(Input.mousePosition);
            }
            else if (Input.touchCount == 1 && Input.GetTouch(0).phase == touchPhase)
            {
                selectObject(Input.GetTouch(0).position);
            }
        }
        else
        {
            bombProperties.UpdateMaterial(solver);
        }
        if (CounterBehaviour.timeup == true)
        {
            Explode();
        }
    }

    private void InstantiateBomb()
    {
        //Load from Resouces
        bombs = Resources.LoadAll<GameObject>("Bombs");
        if (!bombs.Any())
            throw new ExitGUIException();

        // Choose a bomb from difficulty
        GameObject bomb = bombs[difficulty];
        bombProperties = bomb.GetComponent<BombProperties>();
        bombProperties.LoadButtons();
        Transform counter = bomb.transform.Find("Counter");
        counterBehaviour = counter.gameObject.GetComponent<CounterBehaviour>();
        // Instantiate Bomb
        Instantiate(bomb, generatePosition.transform);

    }

    private void selectObject(Vector3 position)
    {
        if (Camera.main is { })
        {
            Ray ray = Camera.main.ScreenPointToRay(position);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow, 100f);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    GameObject gameObject = hit.transform.gameObject;
                    if (gameObject.transform.name.Contains("Button") || gameObject.transform.name.Contains("Rope"))
                    {
                        Debug.Log("Touched " + gameObject.transform.name);
                        if (gameObject.transform.name.Contains("Button"))
                            StartCoroutine(buttonMovement(gameObject));
                        else if (gameObject.transform.name.Contains("Rope"))
                            RopeMovemnet(gameObject);
                        if (buttonReaction(gameObject))
                        {
                            Debug.Log("You clicked the right button");

                            bombProperties.UpdataButton(gameInfo.gameId, () =>
                            {
                                Debug.Log("update button input!");
                            }, Debug.Log);

                            // Wins
                            if (checkWinState())
                            {
                                Debug.Log("Change Scene.");
                                gameRecords.win_state = "true";
                                gameRecords.time_left = counterBehaviour.timeRemaining;
                                //Change Scene to 3.
                                if (!solver)
                                {
                                    bombProperties.StopListenForButton();
                                }


                            }

                            gameRecords.score += 10;
                            StartCoroutine(changeScene(2));

                        }
                        else
                        {

                            bombProperties.exploded = true;
                            bombProperties.UpdateExplosion(gameInfo.gameId, Debug.Log);

                            Explode();
                            /*
                            Debug.Log("Boom!!!");
                            gameRecords.time_left = counterBehaviour.timeRemaining;
                            gameRecords.win_state = "false";
                            //TODO: explode effect
                            bombProperties.StopListenForButton();
                            bombProperties.StopListenForLose();
                            
                            StartCoroutine(changeScene(3));
                            */
                        }
                    }
                }
            }
        }
    }

    IEnumerator buttonMovement(GameObject button)
    {
        var position = button.transform.position;
        position = new Vector3(position.x, position.y - 0.02f, position.z);
        yield return new WaitForSeconds(0.3f);
        position = new Vector3(position.x, position.y + 0.02f, position.z);
        button.transform.position = position;
    }

    void RopeMovemnet(GameObject rope)
    {
        rope.GetComponent<Renderer>().enabled = false;
        rope.GetComponent<MeshCollider>().enabled = false;
        rope.transform.GetChild(0).gameObject.SetActive(true);
        rope.transform.GetChild(1).gameObject.SetActive(true);
    }

    IEnumerator changeScene(int seconds)
    {
        yield return new WaitForSeconds(seconds);

        MainManager.Instance.gameManager.onEndGame(gameRecords, () =>
        {
            Debug.Log("End Game!");
        }, Debug.Log);

        SceneManager.LoadScene(3);
    }

    bool buttonReaction(GameObject button)
    {
        bool res = false;
        for (int i = 0; i < bombProperties.clickable_indexes.Length; i++)
        {
            if (bombProperties.clickable_indexes[i] == true)
            {
                res = true;
                bombProperties.clickable_indexes[i] = false;
                break;
            }
        }

        return res;
    }

    private bool checkWinState()
    {
        int counter = 0;
        for (int i = 0; i < bombProperties.clickable_indexes.Length; i++)
        {
            if (bombProperties.clickable_indexes[i] == false)
                counter++;
        }
        return counter == bombProperties.clickable_indexes.Length;
    }

    private void Explode()
    {
        Debug.Log("Boom!!!");
        gameRecords.time_left = counterBehaviour.timeRemaining;
        gameRecords.win_state = "false";
        generatePosition.SetActive(false);
        explosion.SetActive(true);
        if (!solver)
        {
            bombProperties.StopListenForButton();
            bombProperties.StopListenForLose();
        }
        StartCoroutine(changeScene(3));
    }
}

