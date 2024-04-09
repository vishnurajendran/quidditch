using Agents;
using System.Collections.Generic;
using Teams;
using UnityEngine;
using Utils;

public class FuzzySystem : SingletonBehaviour<FuzzySystem>
{
    [SerializeField] private float Team1Scores = 0.0f;
    [SerializeField] private float Team2Scores = 0.0f;
    [SerializeField] private float Team1fuzzyRate = 1.0f;
    [SerializeField] private float Team2fuzzyRate = 1.0f;

    private const float df_aggressiveRate = 1.5f;
    private const float df_averageSpeedRate = 1.0f;
    private const float df_calmlySpeedRate = 0.8f;


    public void OnGameScoreChanged(int team1Score, int team2Score)
    {
        Team1Scores = team1Score;
        Team2Scores = team2Score;
        //update team1 fuzzy rate
        {
            //calculate fuzzy rate
            Team1fuzzyRate = RunFuzzyLogic(team1Score, team2Score);
            List<Transform> team1Players = TeamManager.GetPlayersOfTeam(Team.Team_1);

            //Assign fuzzy rate to each agent
            foreach (Transform unit in team1Players)
            {
                unit.GetComponent<Agent>().SetFuzzyRate(Team1fuzzyRate);
            }
        }
        
        //update team2 fuzzy rate
        {
            //calculate fuzzy rate
            Team2fuzzyRate = RunFuzzyLogic(team2Score, team1Score);
            List<Transform> team2Players = TeamManager.GetPlayersOfTeam(Team.Team_2);

            //Assign fuzzy rate to each agent
            foreach (Transform unit in team2Players)
            {
                unit.GetComponent<Agent>().SetFuzzyRate(Team2fuzzyRate);
            }
        }
    }

    private float RunFuzzyLogic(int friendScore, int enemyScore)
    {
        //First, fuzzyification. Here, we convert unit numbers values between 1 -0 and find membership values
        float[] degreesOfMemberships = Fuzzyification(friendScore, enemyScore);
        //Then, using the degrreso of membership, we evaluate rules. You should get (2x2x2=8 trules) 
        float[] FuzzyOutput = RuleEvaluation(degreesOfMemberships);
        // finally, Defuzzification process, generates one output


        float crispOutputHighest = DefuzzificationHighest(FuzzyOutput);
        Debug.Log("The crisp Output: " + crispOutputHighest);
        return crispOutputHighest;
    }

    // fill the codes below
    private float[] Fuzzyification(int friendScore, int enemyScore)
    {
        int scoreDifference = friendScore - enemyScore;
        float[] membershipFunctionResults = {
            Fuzzyification_friendScore(friendScore),
            Fuzzyification_enemyScore(enemyScore),
            Fuzzyification_differenceScore(scoreDifference)
        };

        return membershipFunctionResults;
    }

    private float LinearFunction(float x1, float y1, float x2, float y2, float x)
    {
        if (x < x1)
            return y1;
        else if (x > x2)
            return y2;
        else
        {
            float rate = (x - x1) / (x2 - x1);
            return (y2 - y1) * rate + y1;
        }
    }

    private float LinearPeakFunction(float x1, float y1, float x2, float y2, float x3, float y3, float x)
    {
        if (x < x1)
            return y1;
        else if (x > x3)
            return y3;
        else if(x >= x1 && x < x2)
        {
            float rate = (x - x1) / (x2 - x1);
            return (y2 - y1) * rate + y1;
        }
        else //(x >= x2 && x<= x3)
        {
            float rate = (x - x2) / (x3 - x2);
            return (y3 - y2) * rate + y2;
        }
    }

    private float ScoreFuzzyFunctionForFriend(float friendsScore)
    {
        return LinearPeakFunction(30, 0.5f, 100, 1.0f, 150, 0.0f, friendsScore);
    }

    private float ScoreFuzzyFunctionForEnemy(float enemyScore)
    {
        return LinearFunction(30, 0.0f, 100, 1.0f, enemyScore);
    }

    private float ScoreFuzzyFunctionForDifference(float differenceScore)
    {
        return LinearFunction(-50, 0.0f, 50, 1.0f, differenceScore);
    }


    private float Fuzzyification_friendScore(int score)
    {
        float fuzzyInputFriendly = ScoreFuzzyFunctionForFriend(score);
        return fuzzyInputFriendly;
    }

    private float Fuzzyification_enemyScore(float score)
    {
        float fuzzyInputEnemy = ScoreFuzzyFunctionForEnemy(score);
        return fuzzyInputEnemy;
    }

    private float Fuzzyification_differenceScore(float score)
    {
        float fuzzyInputDifference = ScoreFuzzyFunctionForDifference(score);
        return fuzzyInputDifference;
    }

    private float[] RuleEvaluation(float[] fuzzyInput)
    {
        float HighFriendScores = fuzzyInput[0];
        float LowFriendScores = 1.0f - HighFriendScores;
        float HighEnemyScores = fuzzyInput[1];
        float LowEnemyScores = 1.0f - HighEnemyScores;
        float HighDifferenceScores = fuzzyInput[2];
        float LowDifferenceScores = 1.0f - HighDifferenceScores;

        //for calm velocity
        float calmVelocityRule1 = Mathf.Max(LowEnemyScores, HighFriendScores);

        //for normal velocity
        float normalVelocityRule1 = 1.0f - Mathf.Abs(HighDifferenceScores - 0.5f);

        //for aggressive velocity
        float agressiveVelocityRule1 = LowDifferenceScores;


        float averageValue = Mathf.Max(new float[] { calmVelocityRule1 });
        float aggresive = Mathf.Max(new float[] { normalVelocityRule1 });
        float moveCalmly = Mathf.Max(new float[] { agressiveVelocityRule1 });

        //collect fuzzy outputs
        float[] outputVariable = { averageValue, aggresive, moveCalmly };
        return outputVariable;
    }

    //choose the maximum fuzzy outputs as the crisp output
    private float DefuzzificationHighest(float[] fuzzyResults)
    {
        if (fuzzyResults[1] > fuzzyResults[2] && fuzzyResults[1] > fuzzyResults[0])
            return df_aggressiveRate;
        else if (fuzzyResults[0] >= fuzzyResults[1] && fuzzyResults[0] >= fuzzyResults[2])
            return df_averageSpeedRate;
        else
            return df_calmlySpeedRate;
    }

}
