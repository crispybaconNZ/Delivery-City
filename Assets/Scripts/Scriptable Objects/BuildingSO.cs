using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Building", fileName = "New Building")]
public class BuildingSO : ScriptableObject {
    public string buildingName;
    public string address;
    public Sprite sprite;
    public CustomerSO customer;

    public override string ToString() {
        string result = "";
        if (buildingName == "") {
            result = $"{address}";
        } else {
            result = $"{buildingName}, {address}";
        }

        if (customer != null) {
            result += $", home to {customer.name}";
        }

        return result;
    }
}
