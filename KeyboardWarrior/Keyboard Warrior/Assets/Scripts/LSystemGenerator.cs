using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LSystemGenerator 
{
    private Dictionary<char, string[]> rules;
    private Dictionary<char, float[]> probabilities; // chances of rules/replacement strings
    private string axiom;

    public LSystemGenerator(string axiom, Dictionary<char, string[]> rules, Dictionary<char, float[]> probabilities)
    {
        this.rules = rules;
        this.probabilities = probabilities;
        this.axiom = axiom;
    }

    public string Generate(int iterations)
    {
        string current = axiom;
        for(int i = 0; i < iterations; i++)
        {
            current = ApplyRules(current);
        }

        return current;
    }


    private string ApplyRules(string input)
    {
        StringBuilder output = new StringBuilder();
        foreach(char c in input)
        {
            output.Append(GetReplacement(c));
        }

        return output.ToString();
    }


    private string GetReplacement(char c)
    {
        if (!rules.ContainsKey(c)) return c.ToString();

        float roll = Random.value;
        float cumulative = 0f;
        string[] options = rules[c];
        float[] probs = probabilities[c];

        for(int i = 0; i < options.Length; i++)
        {
            cumulative += probs[i];
            if (roll <= cumulative) return options[i];
        }

        return options[options.Length - 1];
    }
}
