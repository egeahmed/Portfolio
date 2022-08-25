using System;

using System.Collections.Generic;



[Serializable]
public class PlayerData 
{

    //struct for player data
    //string user_id;

    public string playerName;

    public int totalScore;

    //total games as instructor
    public int ins;

    //total games as solver
    public int sol;

    //win games as instructor
    public int winIns;

    //win games as solver
    public int winSol;

    List<Dictionary<string, float>> tie_strengths;


    //List<string> history_gameIds;

    public PlayerData(string playerName)
    {
        //this.user_id = user_id;

        this.playerName = playerName;

        this.totalScore = 0;

        this.ins = 0;

        this.sol = 0;

        this.winIns = 0;

        this.winSol = 0;

        //this.loseIns = 0;

        //this.loseSol = 0;

        this.tie_strengths = new List<Dictionary<string, float>>();

        //history_gameIds = new List<string>();
    }

}
